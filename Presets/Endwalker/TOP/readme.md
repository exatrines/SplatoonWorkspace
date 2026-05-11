# The Omega Protocol
## P3 Transition
```
https://raw.githubusercontent.com/exatrines/SplatoonPresets/refs/heads/main/Scripts/Endwalker/TOP/P3_Transition.cs
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
https://raw.githubusercontent.com/exatrines/SplatoonPresets/refs/heads/main/Scripts/Endwalker/TOP/P5_Dynamis_Sigma_Relative_Tower_Finder.cs
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
https://raw.githubusercontent.com/exatrines/SplatoonPresets/refs/heads/main/Scripts/Endwalker/TOP/P5_Dynamis_Sigma_Hello_World.cs
```

P5のコードデュナミス・シグマのハロワ処理のスクリプトです。マーカーとデバフをもとに散開先にガイドします。また、回転ビームとオメガFのブレードアクションも回避するようガイドされるので、関連するレイアウトは無効化して問題ありません。

### Configration

各担当がどのポジションに行くのかを細かく設定できます。ギミックの都合でエレメントの編集で位置を変更することができないので、テーブル上で編集してください。

#### Resolve BaitNear
ニア誘導をマーカー依存ではなくどのロールにも割当たっていない残りの２名とします。チェックを押すとBaitNear1, 2の選択肢が非表示になり、誘導位置両方にテザー表示されます。`Attack me`マクロを各自押すタイプだとAttack5,6が付与されないことがあるので、チェックを推奨します。

### Configration Sample
りりどマクロの場合は「Import Japanese Strat」を押下してください。

- Macro: https://jp.finalfantasyxiv.com/lodestone/character/34120564/blog/5178791/
- RaidPlan: https://raidplan.io/plan/u98293e225836jcy

## P5 Dynamis Omega Safe Guide
```
https://raw.githubusercontent.com/exatrines/SplatoonPresets/refs/heads/main/Scripts/Endwalker/TOP/P5_Dynamis_Omega_Safe_Guide.cs
```

P5のコードデュナミス・オメガの激狭安置にナビします。1回目、2回目の安置を表示し、1回目にテザーを表示します。1回目着弾後テザーが2回目の安置に引き直されます。

視認性を上げるために公式レイアウトの以下レイアウトを非表示、もしくは透明度を下げることを推奨します。

- 「P5 D3 - M/F 1st clones attacks」
- 「P5 D3 - M/F 2nd clones attacks」
- 「P5 D3 - Diffuse WaveCannon」

### Configration
設定不要

### Sample
![](https://media.discordapp.net/attachments/1489216812674715708/1502283862804271265/image.png?ex=6a02721f&is=6a01209f&hm=31cd8a1a161775a5e66644d1e572bd3e4ad1fe8c19b6d4857d4641d81049cd6c&=&format=webp&quality=lossless&width=1234&height=602)

## P5 Dynamis Omega Hello World
```
https://raw.githubusercontent.com/exatrines/SplatoonPresets/refs/heads/main/Scripts/Endwalker/TOP/P5_Dynamis_Omega_Hello_World.cs
```

デバフとマーカーでハロワの散開先をナビします。必ずマーカー付与が必要です。

初期設定時の各エレメントの初期表示位置はRaidPlanを参考にしてください。

- [RaidPlan](https://raidplan.io/plan/fbxgrh8z7z6x8kvu)

### Configration
Spread1, Spread2にそれぞれマーカーと担当を指定してください。

#### Resolve BaitNear
ニア誘導をマーカー依存ではなくどのロールにも割当たっていない残りの２名とします。チェックを押すとBaitNear1, 2の選択肢が非表示になり、誘導位置両方にテザー表示されます。

#### Resolve BaitTether
オメガ紐の誘導をマーカー依存ではなくデュナミスが３の２名とします。チェックを押すとBaitTether1, 2の選択肢が非表示になり、誘導位置両方にテザー表示されます。

