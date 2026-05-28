# WingspanFreeBird

> [中文版 / Chinese →](README.md)

A Wingspan MOD that **waives food + egg costs** for the local human player, and lets you **force specific birds into the first player's starting hand** via a config file.

> Warning: this MOD significantly breaks game balance. Use with caution, at your own risk.

## Background and Purpose

The main reason this MOD was built is to complete the achievement **Birdnado** — score over 180 points in a single game.

<img width="236" height="77" alt="image" src="https://github.com/user-attachments/assets/aa4bf5d2-834a-45bf-be85-169d90da3f53" />

A normal game averages around 60–80 points; even with the T0-tier "Big Four" birds you typically top out at 120–140. So to hit 180+ you essentially need 4 or more human-controlled players cooperating.

After testing, I found the most reliable approach is the **Wetland Tuck build** (there are walkthroughs on Bilibili and YouTube — look them up). My recommended core hand:

**Greater Flamingo** + **Gray Catbird / Northern Mockingbird** (one or both) + **Common Chiffchaff / Mute Swan** (one, prefer Common Chiffchaff) + **Audouin's Gull**

<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/5ea86182-0875-4c40-ac4c-1d30e54f65da" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/e729918f-825b-4c9f-a26d-8c9998a73869" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/53d6f08e-d3d6-46d4-9bc6-1a75ba704ed6" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/938652c8-57e2-4dc7-8880-93cfe24ebe74" />

- **Greater Flamingo**: core end-of-round cycling engine. Reliably tucks 4–5 cards at the end of each round; combined with Gray Catbird / Northern Mockingbird, worth 8–10 pts.
- **Common Chiffchaff / Mute Swan**: early end-of-round cycling. Reliably tucks 3–5 cards at the end of each round; combined with Gray Catbird / Northern Mockingbird, worth 6–10 pts.
- **Audouin's Gull**: draws cards + reliably tucks 1 per turn. Combined with Gray Catbird / Northern Mockingbird, every turn nets you a stable 5 cards in hand + 2 tucks (3 cards from the full-wetland draw, plus 2 extra from Audouin's Gull).
- **Gray Catbird / Northern Mockingbird**: pick which bird to copy based on context — when you need to draw cards → copy Audouin's Gull; start of a round → copy Common Chiffchaff / Mute Swan; end of a round → copy Greater Flamingo.

Beyond those 4 core cards: if you only have one of Gray Catbird / Northern Mockingbird, your 5th card can be:

- **Sandhill Crane / American White Pelican / Double-crested Cormorant / Canada Goose** etc.: spend food to tuck 2 cards (requires another player to consistently feed you the right food — e.g. via Red Crossbill / Anna's Hummingbird / Ruby-throated Hummingbird etc.).

<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/71334121-6b18-4e84-8e92-8f80fcd5eb0e" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/4d474691-6143-4311-b36d-dac91438286b" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/0167c974-6eed-4c33-a54d-62e5b1cd73c7" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/7381fbb3-9c4a-4de0-9a99-d7d08e9d432f" />

- **Bushtit / Common Grackle** etc.: tuck cards + lay eggs (very smooth early-game ramp-up; in the late game your eggs may overflow the per-bird cap).

<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/621ea7fb-23af-4cd3-afa0-b63495a9fb5f" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/92decc88-9224-4e0e-bc04-bad88e45c158" />

In the **Forest**, while cycling cards, watch for 2 types of cards:

- **Red-eyed Vireo / Downy Woodpecker / Ruby-crowned Kinglet** etc.: lets you play an extra bird — fill a whole forest row in one turn, and grab a bonus card (e.g. Spotted Owl) or a high-scoring bird (e.g. Bonelli's Eagle) on the last play.

<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/55e02967-8a7b-49bd-9927-c3ccfd4a03be" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/3f64a3a7-62e4-401d-87f2-04fa9628cae3" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/2d4bcb42-fcb9-4467-8a3a-e7609a336ae9" />


<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/d727e73a-fcb8-42dc-b31e-075faf4568de" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/49a568fb-34fa-4040-8a32-b155d8754ea5" />

Also in the **Forest**, watch for the following cards:

- **Mountain Bluebird / Eastern Bluebird / Savannah Sparrow** etc.: lets you play an extra bird — fill a whole forest row in one turn, and grab a bonus card (e.g. Chestnut-collared Longspur) or a high-scoring bird (e.g. Eastern Imperial Eagle) on the last play.

<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/f5555ef8-80a6-493e-bffa-97e9355d3b6f" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/923f3cb2-8078-4546-8f04-74c210edbb08" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/dcfb1383-4a1d-4162-8e06-53f7519de965" />

<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/8f8f731e-fbfc-40cb-b925-a670efb33d93" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/2f6f0938-249e-4145-b70d-67c08966959c" />

- **European Goldfinch / Snow Bunting** etc.: tuck + swap cards.

<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/286a33d6-d0c2-4b64-ace0-a1daadc07f78" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/e9460386-9613-4520-b567-ed55fc65b93a" />

- **Common Cuckoo / Loggerhead Shrike**: extra egg laying / tuck cards via food, for bonus points.

<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/acadae33-6920-41aa-84fb-28848b367cbd" />
<img width="188" height="378" alt="image" src="https://github.com/user-attachments/assets/634016f7-0faa-4b90-a111-7d01deb367e3" />



Only the local human is affected; AI (Automa / MCTS) is completely untouched, so the game keeps its sense of challenge.

## Features

- **Free food**: on the local human's turn, any bird card can be played without spending food.
- **Free eggs**: the egg cost when placing in column 2 or later is zeroed out.
- **Custom starting hand**: a config file lets you force specific bird cards into the first (seat-order) player's starting hand (still kept at 5, deduplicated).

## Compatibility

- Wingspan (Steam / GOG), Unity IL2CPP **x86 (32-bit)** backend.
- Requires [BepInEx-Unity.IL2CPP-win-x86 **6.0.0-be.755**](https://builds.bepinex.dev/projects/bepinex_be) or a compatible build.
- Verified on Wingspan + Unity 6000.0.58f2.

## Install (end users)

1. Extract the BepInEx IL2CPP **x86** archive into the Wingspan game root (the folder containing `Wingspan.exe`).
2. Launch the game once and wait for BepInEx to generate `BepInEx\interop\` and `BepInEx\unity-libs\`, then quit.
3. Drop this plugin's `WingspanFreeBird.dll` and `bird_ids.txt` into:
   `<game dir>\BepInEx\plugins\WingspanFreeBird\`
4. Launch the game once more — BepInEx will generate the config file at:
   `<game dir>\BepInEx\config\com.ncn.wingspanfreebird.cfg`
5. Edit the config (see below) and **restart the game** for changes to take effect.

## Configuration

```ini
[StartingHand]
## Force these bird cards into the first (seat-order Players[0]) player's starting hand.
## Bird card IDs, comma-separated (see bird_ids.txt next to the plugin for the ID table).
## Specified cards go to the front of the starting hand; the rest is padded from the
## originally-dealt cards, deduplicated. Up to 5 IDs total; leave empty to disable
## (the starting hand becomes fully random again).
## Restart the game for changes to apply. Example: 255,12,40   (255 = Greater Flamingo)
Player1Cards = 255
```

Notes:
- If a forced bird belongs to an **expansion that is not enabled**, missing card data may cause UI issues.
- Config changes require a **game restart**; hot-reload is not supported.
- Setting `Player1Cards = ` (empty) disables only the starting-hand injection. The food/egg waiver is always on; there is no toggle.

## Build from source

Requirements: .NET 6 SDK + a Wingspan install where BepInEx has been set up and the game has been launched at least once.

1. `git clone` this repo.
2. Copy `Directory.Build.props.user.example` to `Directory.Build.props.user` and edit `<GameDir>` to point at your Wingspan install. This file is gitignored, so your local path will not be committed.
3. `dotnet build`. The output DLL is auto-deployed to `<GameDir>\BepInEx\plugins\WingspanFreeBird\`.

## Implementation notes

Harmony patches on:

- `Rulebook.CalcutateFood` / `CanPayFoodForBird` — food affordability gates.
- `BirdPayment.get_EnoughEggs`, `EggsPaymentStateController.HasEnoughEggs` / `GetEggsCost` — egg-payment gates.
- `BasePlayerController.InternalPlayBirdAndPay` / `InternalPlayBirdOnBird` — clear payment when a bird is actually played.
- `LocalPlayer.PickStartingCardsAndPay` (with the base method as a fallback) — starting-hand injection.

Guard: each patch checks "is it the local human's turn?" via `GameStateDriver.CurrentPlayerController` + `TryCast<LocalPlayer>() && !TryCast<RemotePlayer>()`, so AI turns are never affected. The starting-hand injection matches `Players[0]` by `Il2CppObjectBase.Pointer`.

## Credits

- [BepInEx](https://github.com/BepInEx/BepInEx) — IL2CPP loader
- [HarmonyX](https://github.com/BepInEx/HarmonyX) — runtime patching framework
- [Il2CppInterop](https://github.com/BepInEx/Il2CppInterop) — IL2CPP interop

## License

[MIT](LICENSE)
