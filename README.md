# WingspanFreeBird

Wingspan (展翅翱翔) MOD：本地人类玩家**免食物 + 免蛋**打鸟，并可通过配置文件**指定第 1 位玩家的起手牌**。

仅作用于本地人类，AI（Automa / MCTS）完全不受影响，对局挑战感保持。

## 功能

- **免食物**：本地人类回合，任意鸟卡可直接打出，不消耗食物。
- **免蛋**：第 2 列及更后栖息地的弃蛋成本归零。
- **指定起手牌**：通过配置文件给座位顺位第 1 位玩家强制安排起手牌（保持 5 张、去重）。

## 兼容性

- Wingspan (Steam / GOG)，Unity IL2CPP **x86 (32-bit)** 后端。
- 依赖 [BepInEx-Unity.IL2CPP-win-x86 **6.0.0-be.755**](https://builds.bepinex.dev/projects/bepinex_be) 或兼容版本。
- 已在 Wingspan + Unity 6000.0.58f2 上验证。

## 安装（终端用户）

1. 解压 BepInEx IL2CPP **x86** 包到 Wingspan 游戏根目录（含 `Wingspan.exe` 的那一层）。
2. 启动游戏一次，等 BepInEx 生成 `BepInEx\interop\`、`BepInEx\unity-libs\` 后退出。
3. 把本插件的 `WingspanFreeBird.dll` 和 `bird_ids.txt` 放到：
   `<游戏目录>\BepInEx\plugins\WingspanFreeBird\`
4. 再启动游戏一次，BepInEx 会生成配置文件：
   `<游戏目录>\BepInEx\config\com.ncn.wingspanfreebird.cfg`
5. 编辑配置文件（见下），**重启游戏**生效。

## 配置

```ini
[StartingHand]
## 强制第 1 位玩家(座位顺位 Players[0])的起手牌包含这些鸟卡。
## 填鸟卡 ID, 逗号分隔(ID 对照见插件目录的 bird_ids.txt)。
## 指定的牌放到起手牌最前面, 其余用本来发到的牌补满 5 张并去重。
## 最多取前 5 个; 留空则关闭此功能(起手牌完全随机)。
## 改完需重启游戏生效。例: 255,12,40   (255=Greater Flamingo)
Player1Cards = 255
```

注意：
- 填的鸟若属于**未启用的扩展**，可能数据缺失导致 UI 异常。
- 配置改动需要**重启游戏**，不支持热加载。
- 留空 `Player1Cards = ` 关闭起手牌注入；食物/蛋的免费是常开、没有开关。

## 从源码构建

需要：.NET 6 SDK + 已装好 BepInEx 并启动过一次游戏的 Wingspan 安装。

1. `git clone` 本仓库。
2. 复制 `Directory.Build.props.user.example` 为 `Directory.Build.props.user`，编辑其中 `<GameDir>` 指向你的 Wingspan 安装路径。该文件已被 `.gitignore`，本地路径不会被提交。
3. `dotnet build`。构建产物会自动复制到 `<GameDir>\BepInEx\plugins\WingspanFreeBird\`。

## 实现要点

Harmony patches on：

- `Rulebook.CalcutateFood` / `CanPayFoodForBird` — 食物可付性判断。
- `BirdPayment.get_EnoughEggs`、`EggsPaymentStateController.HasEnoughEggs` / `GetEggsCost` — 蛋的支付门。
- `BasePlayerController.InternalPlayBirdAndPay` / `InternalPlayBirdOnBird` — 落子时清付费。
- `LocalPlayer.PickStartingCardsAndPay`（含 base 兜底）— 起手牌注入。

守卫：通过 `GameStateDriver.CurrentPlayerController` + `TryCast<LocalPlayer>() && !TryCast<RemotePlayer>()` 判定"本地人类回合"，AI 回合永不进入。起手牌注入通过 `Il2CppObjectBase.Pointer` 与 `Players[0]` 比对。

## 致谢

- [BepInEx](https://github.com/BepInEx/BepInEx) — IL2CPP 加载器
- [HarmonyX](https://github.com/BepInEx/HarmonyX) — 运行时 patch 框架
- [Il2CppInterop](https://github.com/BepInEx/Il2CppInterop) — IL2CPP 互操作

## License

[MIT](LICENSE)
