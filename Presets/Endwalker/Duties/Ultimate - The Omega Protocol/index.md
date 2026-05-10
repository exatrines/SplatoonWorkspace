# The Omega Protocol
## P3 Transition
```
https://raw.githubusercontent.com/exatrines/SplatoonPresets/refs/heads/main/Scripts/Duties/Endwalker/The%20Omega%20Protocol/P3_Transition.cs
```

P3のコロッサスブローをフルガイドするスクリプトです。パルス（ドーナツ範囲）のAOEは表示されないので、任意のレイアウトを導入してください。

### Configration
Priority設定が必要です。

#### Group direction settings
グループごとにどの方角を使用するかを選択してください。

### Configration Sample
りりどマクロの場合は以下のような設定となります。
- Group direction settings: 上から NorthWest, NorthEast, West, SouthWest, SouthEast, East

## P5 Dynamis Sigma Relative Tower Finder
```
https://raw.githubusercontent.com/exatrines/SplatoonPresets/refs/heads/main/Scripts/Duties/Endwalker/The%20Omega%20Protocol/P5_Dynamis_Sigma_Relative_Tower_Finder.cs
```

P5のコードデュナミス・シグマの「マクロ押さない式」のスクリプトです。波動砲の散開位置を基準にどの塔に入るべきかナビゲーションされます。

注意：このスクリプトではプレステ整列および波動砲散開はナビゲーションされません。公式スクリプトの[「Dynamis Sigma」](https://github.com/PunishXIV/Splatoon/blob/main/Presets/Endwalker/Duties/Ultimate%20-%20The%20Omega%20Protocol/Phase%205%20-%20Dynamis%20Omega%20-%202.%20Sigma.md)と組み合わせて使用することを推奨します。

### Strategy 
for EN
- [Raidplan](https://raidplan.io/plan/cdq6r6x2sffs53kf)

for JP
- [Zattou Inugami 日記「絶オメガP5シグマ塔踏みのコツ：マクロは押さない式が簡単！」 | FINAL FANTASY XIV, The Lodestone ](https://jp.finalfantasyxiv.com/lodestone/character/36478083/blog/5319315/)
- [【FF14】絶オメガ検証戦　コード：＊＊＊ミ＊【シグマ】　塔踏みでマクロを使わない方法 | YouTube ](https://www.youtube.com/watch?v=F9xi4EyUrbA)

### Configration
設定不要

## P5 Dynamis Sigma Hello World
```
https://raw.githubusercontent.com/exatrines/SplatoonPresets/refs/heads/main/Scripts/Duties/Endwalker/The%20Omega%20Protocol/P5_Dynamis_Sigma_Hello_World.cs
```

P5のコードデュナミス・シグマのハロワ処理のスクリプトです。マーカーとデバフをもとに散開先にガイドします。また、回転ビームとオメガFのブレードアクションも回避するようガイドされるので、関連するレイアウトは無効化して問題ありません。

### Configration
[シグマりょんめ](https://youtu.be/pNS1WBbs89w?t=1180)のプリセットを用意しています。これらの処理法以外の設定が必要な場合は手動で設定してください。

### Configration Sample
りりどマクロの場合は「Import Japanese Strat」で問題ありません。

## P5 Dynamis Omega Safe Guide
```
https://raw.githubusercontent.com/exatrines/SplatoonPresets/refs/heads/main/Scripts/Duties/Endwalker/The%20Omega%20Protocol/P5_Dynamis_Omega_Safe_Guide.cs
```

P5のコードデュナミス・オメガの激狭安置にナビします。

視認性を上げるために公式レイアウトの以下レイアウトを非表示、もしくは透明度を下げることを推奨します。

- 「P5 D3 - M/F 1st clones attacks」
- 「P5 D3 - M/F 2nd clones attacks」
- 「P5 D3 - Diffuse WaveCannon」

### Configration
設定不要
