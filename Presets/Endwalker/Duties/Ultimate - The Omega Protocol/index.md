# The Omega Protocol
## P3 Transition
```
https://raw.githubusercontent.com/exatrines/Splatoon/refs/heads/main/Scripts/Duties/Endwalker/The%20Omega%20Protocol/P3%20Transition.cs
```

P3の「コロッサスブロー」をフルガイドするスクリプトです。パルス（ドーナツ範囲）のAOEは表示されないので、任意のレイアウトを導入してください。

### Configration
Priority設定が必要です。

#### Group direction settings
グループごとにどの方角を使用するかを選択してください。

### Configration Sample
りりどマクロの場合は以下のような設定となります。
- Group direction settings: 上から NorthWest, NorthEast, West, SouthWest, SouthEast, East

## P5 Dynamis Sigma Relative Tower Finder
```
https://raw.githubusercontent.com/exatrines/Splatoon/refs/heads/main/Scripts/Duties/Endwalker/The%20Omega%20Protocol/P5_Dynamis_Sigma_Relative_Tower_Finder.cs
```

P5のコードデュナミス・シグマの「マクロ押さない式」のスクリプトです。波動砲の散開位置を基準にどの塔に入るべきかナビゲーションされます。

注意：このスクリプトではプレステ整列および波動砲散開はナビゲーションされません。公式スクリプトの[「P5 Dynamis Sigma」](https://github.com/PunishXIV/Splatoon/blob/main/Presets/Endwalker/Duties/Ultimate%20-%20The%20Omega%20Protocol/Phase%205%20-%20Dynamis%20Omega%20-%202.%20Sigma.md)を導入し、適切な設定をしてください。

### Strategy 
for EN
- [Raidplan](https://raidplan.io/plan/cdq6r6x2sffs53kf)

for JP
- [Zattou Inugami 日記「絶オメガP5シグマ塔踏みのコツ：マクロは押さない式が簡単！」 | FINAL FANTASY XIV, The Lodestone ](https://jp.finalfantasyxiv.com/lodestone/character/36478083/blog/5319315/)
- [【FF14】絶オメガ検証戦　コード：＊＊＊ミ＊【シグマ】　塔踏みでマクロを使わない方法 | YouTube ](https://www.youtube.com/watch?v=F9xi4EyUrbA)

### Configration
設定不要


## P5 Marking Helper
```
https://raw.githubusercontent.com/exatrines/Splatoon/refs/heads/main/Scripts/Duties/Endwalker/The%20Omega%20Protocol/P5%20Marking%20Helper.cs
```

P5の「コードデュナミス・シグマ」と「コードデュナミス・オメガ」のマーカー付与先をオーバーレイで表示するスクリプトです。自動マーカー付与が使えないときや、リプレイでのマーカー付与のイメージトレーニングに使用してください。

注意：実際のマーカーの付与は行われません。

<details>
<summary>Overlay Sample</summary>

![](https://media.discordapp.net/attachments/1489216812674715708/1490963787501535292/image.png?ex=69f795fb&is=69f6447b&hm=387ceb9f2203c9160f2c272f702593cfbe1d83775bb750fcd87fd2438459e1f4&=&format=webp&quality=lossless)

</details>

以下ルールにてマーカーの付与先を表示します。同じルールに複数名が該当する場合はポジション優先度に従います。

- コードデュナミス・シグマ(LDPU)
    1. ハローワールド・ニアとハローワールド・ファーのプレイヤー2名はマーカーを付与しない
    2. デュナミスの高揚が1のプレイヤーのうち、ポジション優先度の上から順に2名に `BIND` を付与する
    3. 残りのプレイヤー4名に対して、ポジション優先度の上から順に `ATTACK` を付与する
- コードデュナミス・オメガ 前半
    1. ファーストアタックの対象のプレイヤー2名はマーカーを付与しない
    2. 残りのプレイヤー6名の内、以下ルールで2名に `BIND` を付与する
        1. セカンドアタックの対象かつデュナミスの高揚が2のプレイヤー
        2. 残っているプレイヤーの内デュナミスの高揚が2のプレイヤー
        3. 残っているプレイヤーの内デュナミスの高揚が1のプレイヤー
    3. 残りのプレイヤー4名に対して、優先度の上から順に `ATTACK` を付与する
- コードデュナミス・オメガ 後半
    1. セカンドアタックの対象のプレイヤー2名はマーカーを付与しない
    2. デュナミスの高揚が3スタックのプレイヤー2名に `BIND` を付与する
    3. 残りのプレイヤー4名に対して、ポジション優先度の上から順に `ATTACK` を付与する

### Configration
Priority設定が必要です。
#### Show Sigma Helper（LDPU）
シグマのマーカー表示が必要な場合はチェックを付けてください。