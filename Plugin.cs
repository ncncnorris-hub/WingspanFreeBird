using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime;   // TryCast<T>, Il2CppObjectBase.Pointer

// Scope strategy: the game's model types (Player, GameState, Food, BirdData) are
// duplicated ~5x across rule-variant images, so Il2CppInterop renamed them and we
// can't name `Player`. But GameStateDriver / BasePlayerController / LocalPlayer /
// RemotePlayer are single types we CAN name. We use
// GameStateDriver.CurrentPlayerController to ask "is it the local human's turn?"
// and only override food/egg affordability then — accurate from any call path
// (UI validate or the async play-commit), AI turns untouched.
namespace WingspanFreeBird
{
    [BepInPlugin(Guid, "Wingspan Free Bird", "1.2.0")]
    public class Plugin : BasePlugin
    {
        public const string Guid = "com.ncn.wingspanfreebird";
        internal static ManualLogSource Logger;
        internal static GameStateDriver Driver;   // captured once; stable for the match

        // Configured (via BepInEx .cfg) bird IDs forced into seat-1's starting hand.
        // Empty = feature off. Parsed once at Load(); edit the cfg + restart to change.
        internal static readonly List<int> StartingCards = new List<int>();

        public override void Load()
        {
            Logger = Log;
            Logger.LogInfo("WingspanFreeBird v1.2.0 starting...");

            var cfgCards = Config.Bind(
                "StartingHand",
                "Player1Cards",
                "255",
                "强制第 1 位玩家(座位顺位 Players[0])的起手牌包含这些鸟卡。\n" +
                "填鸟卡 ID, 逗号分隔(ID 对照见插件目录的 bird_ids.txt)。\n" +
                "指定的牌放到起手牌最前面, 其余用本来发到的牌补满 5 张并去重。\n" +
                "最多取前 5 个; 留空则关闭此功能(起手牌完全随机)。\n" +
                "改完需重启游戏生效。例: 255,12,40   (255=Greater Flamingo)");
            ParseStartingCards(cfgCards.Value);

            var h = new Harmony(Guid);
            ApplyPatch(h, typeof(HB_CaptureDriver));
            ApplyPatch(h, typeof(HB_CapturePlayBird));
            ApplyPatch(h, typeof(HB_ValidateBirdToPlay));
            ApplyPatch(h, typeof(HB_CalcutateFood));
            ApplyPatch(h, typeof(HB_CanPayFoodForBird));
            ApplyPatch(h, typeof(HB_EnoughEggs));
            PatchEggController(h);
            ApplyPatch(h, typeof(HB_InternalPlayBirdAndPay));
            ApplyPatch(h, typeof(HB_InternalPlayBirdOnBird));
            ApplyPatch(h, typeof(HB_StartingCards_Base));
            ApplyPatch(h, typeof(HB_StartingCards_Local));
            Logger.LogInfo("[FreeBird] Load() finished.");
        }

        // Parse the comma-separated config string into StartingCards (deduped, capped at 5).
        static void ParseStartingCards(string raw)
        {
            StartingCards.Clear();
            if (string.IsNullOrWhiteSpace(raw)) { Logger.LogWarning("[StartHand] config empty -> feature OFF"); return; }
            foreach (var tok in raw.Split(','))
            {
                var s = tok.Trim();
                if (s.Length == 0) continue;
                if (int.TryParse(s, out int id))
                {
                    if (StartingCards.Contains(id)) continue;
                    StartingCards.Add(id);
                    if (StartingCards.Count >= 5) break;
                }
                else Logger.LogWarning($"[StartHand] ignored non-integer token: '{s}'");
            }
            Logger.LogWarning($"[StartHand] configured cards = [{string.Join(",", StartingCards)}]");
        }

        static void ApplyPatch(Harmony h, Type t)
        {
            try { h.CreateClassProcessor(t).Patch(); Logger.LogWarning($"[FreeBird] patched OK: {t.Name}"); }
            catch (Exception e) { Logger.LogError($"[FreeBird] patch FAILED: {t.Name}: {e}"); }
        }

        // EggsPaymentStateController is namespaced (View.Decisions.PlayBird.States), so we
        // resolve it at runtime from the same interop assembly BirdPayment lives in (avoids
        // guessing the interop C# name at compile time) and patch its two egg-gate methods
        // manually. Candidate list covers both with/without the Il2Cpp namespace prefix.
        static void PatchEggController(Harmony h)
        {
            try
            {
                var asm = typeof(BirdPayment).Assembly;
                Type t = null;
                foreach (var name in new[]
                {
                    "View.Decisions.PlayBird.States.EggsPaymentStateController",
                    "Il2CppView.Decisions.PlayBird.States.EggsPaymentStateController",
                })
                {
                    t = asm.GetType(name);
                    if (t != null) { Logger.LogWarning($"[FreeBird] resolved egg controller as: {name}"); break; }
                }
                if (t == null) { Logger.LogError("[FreeBird] EggsPaymentStateController NOT found (egg gate hooks skipped)"); return; }

                var hasEnough = AccessTools.Method(t, "HasEnoughEggs");
                if (hasEnough != null)
                {
                    h.Patch(hasEnough, postfix: new HarmonyMethod(AccessTools.Method(typeof(EggHooks), nameof(EggHooks.HasEnoughEggs_Postfix))));
                    Logger.LogWarning("[FreeBird] patched OK: EggsPaymentStateController.HasEnoughEggs");
                }
                else Logger.LogError("[FreeBird] method NOT found: HasEnoughEggs");

                var getCost = AccessTools.Method(t, "GetEggsCost");
                if (getCost != null)
                {
                    h.Patch(getCost, postfix: new HarmonyMethod(AccessTools.Method(typeof(EggHooks), nameof(EggHooks.GetEggsCost_Postfix))));
                    Logger.LogWarning("[FreeBird] patched OK: EggsPaymentStateController.GetEggsCost");
                }
                else Logger.LogError("[FreeBird] method NOT found: GetEggsCost");
            }
            catch (Exception e) { Logger.LogError($"[FreeBird] PatchEggController FAILED: {e}"); }
        }

        // True only when the player whose turn it currently is, is the local human
        // (LocalPlayer but not a remote human). AI controllers never cast to LocalPlayer.
        internal static bool IsLocalHumanTurn()
        {
            try
            {
                var d = Driver;
                if (d == null) return false;
                var c = d.CurrentPlayerController;
                if (c == null) return false;
                if (c.TryCast<LocalPlayer>() == null) return false;
                if (c.TryCast<RemotePlayer>() != null) return false;
                return true;
            }
            catch { return false; }
        }

        static readonly Dictionary<string, DateTime> _lastLog = new Dictionary<string, DateTime>();
        internal static void Throttled(string key, int ms, string msg)
        {
            try
            {
                var now = DateTime.UtcNow;
                if (_lastLog.TryGetValue(key, out var t) && (now - t).TotalMilliseconds < ms) return;
                _lastLog[key] = now;
                Logger.LogWarning(msg);
            }
            catch { }
        }

        internal static void WaiveCost(BasePlayerController controller, BirdPayment pay)
        {
            try
            {
                if (controller == null || pay == null) return;
                if (controller.TryCast<LocalPlayer>() == null) return;
                if (controller.TryCast<RemotePlayer>() != null) return;
                pay.ClearFoodPayment();
                pay.EnoughFood = true;
                pay.EggsCost = 0;
                if (pay.EggPay != null) pay.EggPay.Clear();
                Logger.LogWarning("[FreeBird][Pay] waived food + egg cost for local human bird play");
            }
            catch (Exception e) { Logger.LogError($"[FreeBird] WaiveCost: {e}"); }
        }

        // ---- Configured cards into seat-1 player's starting hand ----
        // Called from PickStartingCardsAndPay Prefix(es). `cards` is the 5 starting birds
        // (reference type) — mutating its contents here is seen by the coroutine body later.
        // Seat 1 == gameStateDriver.Players[0]; compare by Il2Cpp pointer.
        // Forced cards go to the front; the rest are filled from the originally-dealt cards
        // (deduped) so the hand stays at its original size (normally 5).
        internal static void InjectStartingCards(string via, BasePlayerController controller, GameStateDriver gsd,
                                                 Il2CppSystem.Collections.Generic.List<int> cards)
        {
            try
            {
                if (StartingCards.Count == 0) return;   // feature off
                if (gsd == null || controller == null || cards == null) { Logger.LogWarning($"[StartHand][{via}] skip: null arg"); return; }
                var players = gsd.Players;
                bool isSeat0 = players != null && players.Count > 0 && players[0] != null && players[0].Pointer == controller.Pointer;
                Logger.LogWarning($"[StartHand][{via}] fired; seat0={isSeat0} count={cards.Count} cards=[{Dump(cards)}]");
                if (!isSeat0) return;
                if (cards.Count == 0) { Logger.LogWarning($"[StartHand][{via}] cards empty at hook time -> cannot inject"); return; }

                int n = cards.Count;
                var result = new List<int>();
                foreach (var id in StartingCards)
                {
                    if (result.Count >= n) break;
                    if (!result.Contains(id)) result.Add(id);
                }
                for (int i = 0; i < n && result.Count < n; i++)
                {
                    int id = cards[i];
                    if (!result.Contains(id)) result.Add(id);
                }
                for (int i = 0; i < n && result.Count < n; i++) result.Add(cards[i]); // safety pad

                for (int i = 0; i < n; i++) cards[i] = result[i];
                Logger.LogWarning($"[StartHand][{via}] injected; now=[{Dump(cards)}]");
            }
            catch (Exception e) { Logger.LogError($"[StartHand][{via}] {e}"); }
        }

        static string Dump(Il2CppSystem.Collections.Generic.List<int> cards)
        {
            try
            {
                var sb = new System.Text.StringBuilder();
                for (int i = 0; i < cards.Count; i++) { if (i > 0) sb.Append(','); sb.Append(cards[i]); }
                return sb.ToString();
            }
            catch { return "?"; }
        }
    }

    // Most reliable capture: ChangeCurrentPlayer runs on GameStateDriver every time
    // the active player changes (so it fires each turn, before any play check).
    [HarmonyPatch(typeof(GameStateDriver), "ChangeCurrentPlayer")]
    static class HB_CaptureDriver
    {
        static void Prefix(GameStateDriver __instance)
        {
            if (__instance != null) { Plugin.Driver = __instance; Plugin.Throttled("capdrv", 2000, "[FreeBird] captured GameStateDriver (ChangeCurrentPlayer)"); }
        }
    }

    // Backup capture (the human uses base PlayBird only if LocalPlayer doesn't override it).
    [HarmonyPatch(typeof(BasePlayerController), "PlayBird")]
    static class HB_CapturePlayBird
    {
        static void Prefix(GameStateDriver gameStateDriver)
        {
            if (gameStateDriver != null && Plugin.Driver == null)
            {
                Plugin.Driver = gameStateDriver;
                Plugin.Logger.LogWarning("[FreeBird] captured GameStateDriver");
            }
        }
    }

    // Logging only: confirm the UI gate now passes.
    [HarmonyPatch(typeof(PlayerHandUI), "ValidateBirdToPlay")]
    static class HB_ValidateBirdToPlay
    {
        static void Postfix(PlayerHandUI __instance, ref bool eggsOk, ref bool foodOk)
        { try { if (__instance != null && !__instance.IsAutoma) Plugin.Throttled("val", 250, $"[FreeBird][Validate] foodOk={foodOk} eggsOk={eggsOk}"); } catch { } }
    }

    // ---- Force affordability only during the local human's turn ----

    [HarmonyPatch(typeof(Rulebook), "CalcutateFood")]
    static class HB_CalcutateFood
    {
        static void Postfix(BirdPayment pay)
        {
            try
            {
                bool human = Plugin.IsLocalHumanTurn();
                if (human && pay != null)
                {
                    pay.EnoughFood = true;
                    pay.EggsCost = 0;
                    Plugin.Throttled("calc-h", 300, "[FreeBird][CalcutateFood] FORCED (local human turn)");
                }
                else Plugin.Throttled("calc-n", 1500, $"[FreeBird][CalcutateFood] not forced (human={human} driverNull={Plugin.Driver == null})");
            }
            catch (Exception e) { Plugin.Logger.LogError($"[FreeBird] CalcutateFood: {e}"); }
        }
    }

    [HarmonyPatch(typeof(Rulebook), "CanPayFoodForBird")]
    static class HB_CanPayFoodForBird
    {
        static void Postfix(ref CanPlayReturn __result)
        {
            try
            {
                if (Plugin.IsLocalHumanTurn()) { __result.Can = true; Plugin.Throttled("cpf-h", 300, "[FreeBird][CanPayFoodForBird] forced Can for local human"); }
            }
            catch (Exception e) { Plugin.Logger.LogError($"[FreeBird] CanPayFoodForBird: {e}"); }
        }
    }

    // ---- Hook (2): don't actually charge when the human plays ----

    [HarmonyPatch(typeof(BasePlayerController), "InternalPlayBirdAndPay")]
    static class HB_InternalPlayBirdAndPay
    {
        static void Prefix(BasePlayerController __instance, GameStateDriver gameStateDriver, BirdPayment birdPayment)
        {
            if (gameStateDriver != null && Plugin.Driver == null) Plugin.Driver = gameStateDriver;
            Plugin.WaiveCost(__instance, birdPayment);
        }
    }

    [HarmonyPatch(typeof(BasePlayerController), "InternalPlayBirdOnBird")]
    static class HB_InternalPlayBirdOnBird
    {
        static void Prefix(BasePlayerController __instance, GameStateDriver gameStateDriver, BirdPayment birdPayment)
        {
            if (gameStateDriver != null && Plugin.Driver == null) Plugin.Driver = gameStateDriver;
            Plugin.WaiveCost(__instance, birdPayment);
        }
    }

    // ---- Egg gate (the "not enough eggs" block when playing a 2nd+ bird in a row) ----
    // BirdPayment.EnoughEggs is a computed read-only property; the UI gate (ValidateBirdToPlay)
    // most likely reads it. Force it true on the local human's turn.
    [HarmonyPatch(typeof(BirdPayment), "get_EnoughEggs")]
    static class HB_EnoughEggs
    {
        static void Postfix(ref bool __result)
        {
            try { if (Plugin.IsLocalHumanTurn()) { __result = true; Plugin.Throttled("egg-prop", 300, "[FreeBird][EnoughEggs] forced true (local human)"); } }
            catch (Exception e) { Plugin.Logger.LogError($"[FreeBird] EnoughEggs: {e}"); }
        }
    }

    // Postfixes applied manually (see Plugin.PatchEggController) to the namespaced
    // EggsPaymentStateController. Forcing GetEggsCost to 0 makes the column cost-free and
    // HasEnoughEggs true covers any direct affordability check.
    internal static class EggHooks
    {
        public static void HasEnoughEggs_Postfix(ref bool __result)
        {
            try { if (Plugin.IsLocalHumanTurn()) { __result = true; Plugin.Throttled("egg-has", 300, "[FreeBird][HasEnoughEggs] forced true (local human)"); } }
            catch (Exception e) { Plugin.Logger.LogError($"[FreeBird] HasEnoughEggs: {e}"); }
        }

        public static void GetEggsCost_Postfix(ref int __result)
        {
            try { if (Plugin.IsLocalHumanTurn() && __result != 0) { Plugin.Throttled("egg-cost", 300, $"[FreeBird][GetEggsCost] {__result} -> 0 (local human)"); __result = 0; } }
            catch (Exception e) { Plugin.Logger.LogError($"[FreeBird] GetEggsCost: {e}"); }
        }
    }

    // ---- Force configured cards into the seat-1 player's starting hand ----
    // PickStartingCardsAndPay is a virtual iterator; the human runs LocalPlayer's override.
    // Patch both LocalPlayer and the base (catch-all for any non-overriding controller),
    // sharing one injector. The Prefix mutates the `cards` list in place before the
    // coroutine body uses it.
    [HarmonyPatch(typeof(LocalPlayer), "PickStartingCardsAndPay")]
    static class HB_StartingCards_Local
    {
        static void Prefix(BasePlayerController __instance, GameStateDriver gameStateDriver,
                           Il2CppSystem.Collections.Generic.List<int> cards)
        {
            Plugin.InjectStartingCards("LocalPlayer", __instance, gameStateDriver, cards);
        }
    }

    [HarmonyPatch(typeof(BasePlayerController), "PickStartingCardsAndPay")]
    static class HB_StartingCards_Base
    {
        static void Prefix(BasePlayerController __instance, GameStateDriver gameStateDriver,
                           Il2CppSystem.Collections.Generic.List<int> cards)
        {
            Plugin.InjectStartingCards("Base", __instance, gameStateDriver, cards);
        }
    }
}
