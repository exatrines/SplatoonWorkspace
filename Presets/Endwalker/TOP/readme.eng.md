# The Omega Protocol
## P3 Transition
This script provides full guidance for the Colossus Blow in P3. The pulse (donut wave) AOE is not displayed, so please implement your desired layout.

### Configuration
Priority setting is required.

#### Group direction settings
Select which direction to use for each group.

### Configuration Sample
For the Rirido macro, the settings would be as follows:

- Group direction settings: From top to bottom: NorthWest, NorthEast, West, SouthWest, SouthEast, East

## P5 Dynamis Sigma Relative Tower Finder
This is a "塔マクロ押さない式" script for P5's Code Dynamis Sigma Tower. It navigates you to which tower to enter based on the Wave Cannon's spread position. !! Not Field Marker Strat !!.

Note: This script does not navigate PlayStation alignment or Wave Cannon spread. It is recommended to use this in conjunction with the official script, [Dynamis Sigma](https://github.com/PunishXIV/Splatoon/blob/main/Presets/Endwalker/Duties/Ultimate%20-%20The%20Omega%20Protocol/Phase%205%20-%20Dynamis%20Omega%20-%202.%20Sigma.md).

### Strategy
for EN
- [Raidplan](https://raidplan.io/plan/cdq6r6x2sffs53kf)

for JP
- [Zattou Inugami 日記「絶オメガP5シグマ塔踏みのコツ：マクロは押さない式が簡単！」 | FINAL FANTASY XIV, The Lodestone ](https://jp.finalfantasyxiv.com/lodestone/character/36478083/blog/5319315/)
- [【FF14】絶オメガ検証戦　コード：＊＊＊ミ＊【シグマ】　塔踏みでマクロを使わない方法 | YouTube ](https://www.youtube.com/watch?v=F9xi4EyUrbA)

### Configuration
No Configuration Required

## P5 Dynamis Sigma Hello World
This is the script for handling Hello World in Code Dynamis Sigma of P5. It guides players to their spread locations based on markers and debuffs. It also guides players to avoid the rotating beam and Omega F's blade action, so you can disable the related layouts without any problems.

### Configuration

You can finely configure which position each player goes to. Due to the mechanics, you cannot change the position by editing the elements, so please edit it on the table.

#### Resolve BaitNear
This will make near guidance dependent on the remaining two players not assigned to any role, rather than being marker-dependent. Checking this box will hide the BaitNear1 and 2 options, and tethers will be displayed at both guidance positions. If you're using the `Attack me` macro and pressing it yourself, Attack 5 and 6 might not be applied, so checking is recommended.

### Configuration Sample
For Rirido macros, please press "Import Japanese Strat".

- Macro: https://jp.finalfantasyxiv.com/lodestone/character/34120564/blog/5178791/
- RaidPlan: https://raidplan.io/plan/u98293e225836jcy

## P5 Dynamis Omega Safe Guide
This guides you to the extremely narrow safe zone in Code Dynamis Omega in P5. The first and second safe zones are displayed, and the tether is shown during the first safe zone. After the first shot hits, the tether is redrawn to the second safe zone.

To improve visibility, it is recommended to hide or reduce the transparency of the following layouts in the official layout.

- "P5 D3 - M/F 1st clones attacks"
- "P5 D3 - M/F 2nd clones attacks"
- "P5 D3 - Diffuse WaveCannon"

### Configuration
No settings required

### Sample

![](https://media.discordapp.net/attachments/1489216812674715708/1502283862804271265/image.png?ex=6a02721f&is=6a01209f&hm=31cd8a1a161775a5e66644d1e572bd3e4ad1fe8c19b6d4857d4641d81049cd6c&=&format=webp&quality=lossless&width=1234&height=602)

## P5 Dynamis Omega Hello World
This script uses debuffs and markers to guide players to their Hello World spread locations. Marker placement is mandatory.

Please refer to the RaidPlan for the initial display positions of each element during initial setup.

- [RaidPlan](https://raidplan.io/plan/fbxgrh8z7z6x8kvu)

### Configuration
Specify markers and roles for Spread1 and Spread2 respectively.

#### Resolve BaitNear
This will make near-bait guidance dependent on the remaining two players not assigned to any role, rather than markers. Checking this box will hide the BaitNear1 and 2 options, and tethers will be displayed at both guidance locations.

#### Resolve BaitTether
This will change the Omega Tether guidance from marker-dependent to two players with a Dynamis of 3. Checking this box will hide the BaitTether1 and 2 options and display tethers at both guidance locations.