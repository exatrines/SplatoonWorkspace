# P5 Dynamis Sigma Hello World
## About
コード：＊＊＊ミ＊【シグマ】におけるハロワ処理をガイドするスクリプトです。**必ずマーカー付与が必要**です。

このスクリプトでは中央のリアユニットからのビーム、オメガFのソードアクションを回避し、役割に応じた最終的な散開位置へガイドします。

## Import URL

```
https://raw.githubusercontent.com/PunishXIV/Splatoon/refs/heads/main/SplatoonScripts/Duties/Endwalker/The%20Omega%20Protocol/P5_Dynamis_Sigma_Hello_World.cs
```

## Configuration

![](../../../../assets/endwalker/p5_dynamis_sigma_hello_world_setting.png)

### Spread settings
マーカーとデバフで役割を決定し、処理の開始位置と最終的なハロワ処理を行う位置を設定します。

- Group: 開始位置
- Spread Angle: フィールド中央から見てオメガFの位置を0度としたときの最終散開位置の角度
- Range from Center: 最終散開位置の中央からの距離（フィールド外周は20）

Cw: リアユニットの回転方向が時計回りの場合, Ccw: 反時計周りの場合

注意: ギミックの都合で Registered elements タブでは散開位置を調整できません。必ずテーブル上で編集してください。

### Resolve BaitNear
ニア誘導担当者を役割を割り当てられていない残り2名とします。有効にすると BaitNear1・BaitNear2 の選択肢が非表示になり、ニア誘導位置の両方にテザーが表示されます。

`/mk attack <me>` マクロを各自押す運用では Attack5・6 が付かないことがあるため、このオプションを有効にすることを推奨します。

## Sample configuration
LilyDollマクロの場合は **Import Japanese Strat** を押してください。

- [RaidPlan](https://raidplan.io/plan/u98293e225836jcy)
- [マクロ](https://jp.finalfantasyxiv.com/lodestone/character/34120564/blog/5178791/)

北グループ:
- `Attack1`: BaitArm（NW担当）
- `Attack2`: BaitArm（NE担当）
- `Attack3`: BaitFar（HelloFar から遠い側）

南グループ:
- `HelloNear`
- `HelloFar`
- `Attack4`: BaitFar（HelloFar から近い側）
- `Remaining`: BaitNear
