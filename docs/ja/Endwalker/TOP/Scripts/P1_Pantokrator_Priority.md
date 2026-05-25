# P1 Pantokrator Priority
## About
優先度に基づきパントクラトルをガイドするスクリプトです。以下が表示されます。

- 優先度に基づくグループ分け
- ターゲットデバフによるAOE処理、頭割り処理位置へのナビ
- 波動砲の散開位置へのナビ

## Import URL

```
https://raw.githubusercontent.com/PunishXIV/Splatoon/refs/heads/main/SplatoonScripts/Duties/Endwalker/The%20Omega%20Protocol/P1_Pantokrator_Priority.cs
```

## Configuration

![](../../../../assets/endwalker/p1_panto_prio_setting.png)

### Priority settings
プレイヤーに付与されるターゲットデバフと Priority 設定に基づき、2つのグループ ( High or Low ) に割り当てます。

### Wavecannnon spread direction
この波動砲の処理はタンクが北で無敵する処理法に基づいた散開となります。
ロールごと設定を変更する必要があります。

## Sample Configuration
LilyDollマクロの場合は以下のように設定してください。

- priority: `H1 > T1 > T2 > M1 > M2 > R1 > R2 > H2`
- direction: 以下画像の通り

![](../../../../assets/endwalker/p1_panto_wave.png)
