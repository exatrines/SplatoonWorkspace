# The Omega Protocol
## P5 Marking Helper
```
https://raw.githubusercontent.com/exatrines/Splatoon/refs/heads/main/Scripts/Duties/Endwalker/The%20Omega%20Protocol/P5%20Marking%20Helper.cs
```

P5の「コードデュナミス・シグマ」と「コードデュナミス・オメガ」のマーカー付与先をオーバーレイで表示するスクリプトです。自動マーカー付与が使えないときや、リプレイでのマーカー付与のイメージトレーニングに使用してください。\

<details>
<summary>Overlay Sample</summary>

![](https://media.discordapp.net/attachments/1489216812674715708/1490963787501535292/image.png?ex=69f795fb&is=69f6447b&hm=387ceb9f2203c9160f2c272f702593cfbe1d83775bb750fcd87fd2438459e1f4&=&format=webp&quality=lossless)

</details><br>

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

注意：実際のマーカーの付与は行われません。

### Configration
Priority設定が必要です。
#### Show Sigma Helper（LDPU）
シグマのマーカー表示が必要な場合はチェックを付けてください。

## P5 Dynamis Sigma JP No Macro Strat
```
https://raw.githubusercontent.com/exatrines/Splatoon/refs/heads/main/Scripts/Duties/Endwalker/The%20Omega%20Protocol/P5%20Dynamis%20Sigma%20JP%20No%20macro%20Strat.cs
```

P5のコードデュナミス・シグマの「マクロ押さない式」のスクリプトです。波動砲およびアーム誘導の散開位置と、処理後にどの塔に行くべきかをガイドします。（ハロワ処理はガイドされません。）

### Configration
#### Marker Alignment
マーカーの整列の基準を設定します。マーカーの整列方法は以下の3種類があります。
- NorthToSouth: フィールド外周にいるオメガMの正面に縦に整列します。
- WestToEast: フィールド外周にいるオメガMの正面に横に整列します。
- WestToEast & Both Omega Arms: フィールドに出現している2つのオメガアームの間に整列します。
#### Marker Sort
マーカーの整列順を選択します。Marker Alignmentで選択した整列方法において、上または左を先頭として、どのマーカーがどの位置に来るかを選択します。
#### Wave Cannon Spread Configuration
整列の先頭から順に、それぞれの対象者がどこに散開するべきかを指定します。
#### Other Configurations
- Show Omega-M Tether: ギミック開始時にオメガMに対してテザーを表示します。
- Display tower type: 波動砲の対象となる塔の種類を表示するか。1人塔は「1」、2人塔は「2」と表示されます。

### Configration Sample
りりどマクロの散会かつ、塔は押さない式の場合は以下のような設定となります。
- Marker Alignment: NorthToSouth
- Marker Sort: 上から RedCircle, BlueCross, GreenTriangle, PurpleSquare
- Wave Cannon Spread Configuration: 上から Front, Bottom, Left, Right, BottomLeft, FrontRight, BottomRight, FrontLeft
