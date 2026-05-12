# P1 オメガ
## レイアウト

```
~Lv2~{"Name":"P1 Basic Mulipreset / 基本繪制","Group":"絶オメガ","ZoneLockH":[1122],"Scenes":[2],"ElementsL":[{"Name":"Tower Finder / 塔 ","type":1,"Enabled":false,"radius":2.5,"Donut":0.5,"color":4278255612,"thicc":3.0,"refActorNPCID":2013245,"refActorObjectLife":true,"refActorLifetimeMin":0.0,"refActorLifetimeMax":9.0,"refActorComparisonType":4,"tether":true},{"Name":"Tower Reminder / 進塔提醒","type":1,"Enabled":false,"overlayBGColor":2684354560,"overlayTextColor":4278253567,"overlayVOffset":2.0,"overlayFScale":2.0,"thicc":0.0,"overlayText":">>> !!! TOWER !!! <<<","refActorRequireBuff":true,"refActorBuffId":[3456],"refActorUseBuffTime":true,"refActorBuffTimeMax":11.0,"refActorComparisonType":1,"onlyVisible":true},{"Name":"Laser / 集合提醒 (激光)","type":1,"radius":4.52,"color":4278190335,"Filled":false,"fillIntensity":0.39215687,"overlayBGColor":3355443200,"overlayTextColor":4294940160,"overlayVOffset":2.0,"overlayFScale":2.0,"thicc":5.0,"refActorPlaceholder":["<1>","<2>","<3>","<4>","<5>","<6>","<7>","<8>"],"refActorRequireBuff":true,"refActorBuffId":[3507,3508,3509,3510],"refActorUseBuffTime":true,"refActorBuffTimeMax":5.0,"refActorComparisonType":5,"onlyVisible":true,"FillStep":0.778},{"Name":"Induced AOE / 靠近AOE","type":1,"radius":3.0,"color":4278255612,"overlayBGColor":3472883712,"overlayTextColor":4278255615,"overlayVOffset":2.0,"overlayFScale":2.0,"thicc":3.0,"refActorComparisonType":7,"includeRotation":true,"FaceMe":true,"refActorVFXPath":"vfx/lockon/eff/lockon5_t0h.avfx","refActorVFXMax":3000},{"Name":"Missile / 分散提醒 (射弾)","type":1,"radius":5.0,"color":3355507967,"Filled":false,"fillIntensity":0.39215687,"overlayBGColor":3355443200,"overlayTextColor":4278255600,"overlayVOffset":2.0,"overlayFScale":2.0,"thicc":4.9,"refActorRequireBuff":true,"refActorBuffId":[3424,3495,3496,3497],"refActorUseBuffTime":true,"refActorBuffTimeMax":5.0,"refActorComparisonType":1,"onlyVisible":true}]}
~Lv2~{"Name":"◆サークルプログラム","Group":"絶オメガ","ZoneLockH":[1122],"ElementsL":[{"Name":"1st","type":1,"offZ":2.76,"radius":0.0,"color":4294965504,"overlayBGColor":4294965504,"overlayTextColor":3355443200,"thicc":5.0,"overlayText":"1st","refActorRequireBuff":true,"refActorBuffId":[3004],"refActorComparisonType":1,"refActorType":1,"onlyVisible":true},{"Name":"2nd","type":1,"offZ":2.76,"radius":0.0,"color":3364749567,"overlayBGColor":3364749567,"thicc":5.0,"overlayText":"2nd","refActorRequireBuff":true,"refActorBuffId":[3005],"refActorComparisonType":1,"refActorType":1,"onlyVisible":true},{"Name":"3rd","type":1,"offZ":2.76,"radius":0.0,"color":3372156928,"overlayBGColor":3372156928,"thicc":5.0,"overlayText":"3rd","refActorRequireBuff":true,"refActorBuffId":[3006],"refActorComparisonType":1,"refActorType":1,"onlyVisible":true},{"Name":"4th","type":1,"offZ":2.76,"radius":0.0,"color":3359113471,"overlayBGColor":3359113471,"thicc":5.0,"overlayText":"4th","refActorRequireBuff":true,"refActorBuffId":[3451],"refActorComparisonType":1,"refActorType":1,"onlyVisible":true}]}
```

## スクリプト
### P1 サークルプログラム
デバフと優先度をもとにどの塔を踏むか、テザーを取ってどの方角に行くかがガイドされます。

#### URL
```
https://github.com/PunishXIV/Splatoon/raw/refs/heads/main/SplatoonScripts/Duties/Endwalker/The%20Omega%20Protocol/Program%20Loop%20Priority.cs
```
#### Configuration
![](../../../../assets/endwalker/p1_loop_setting.png)

### P1 パンクラ
直線頭割りとAOEの範囲が表示されます。優先度によるグループ分けは行われないので注意が必要です。

#### URL
```
https://github.com/PunishXIV/Splatoon/raw/main/SplatoonScripts/Duties/Endwalker/The%20Omega%20Protocol/Pantokrator.cs
```
#### Configuration
デフォルトから設定を変更する必要はありませんが、視認性を向上させるために色の調整をすることができます。

### P1 パンクラ無敵タイミングアナウンス
無敵を押すタイミングが頭上に表示されます。

#### URL
```
https://raw.githubusercontent.com/PunishXIV/Splatoon/refs/heads/main/SplatoonScripts/Duties/Endwalker/The%20Omega%20Protocol/Pantokrator%20invincible%20Reminder.cs
```
#### Configuration
設定不要
