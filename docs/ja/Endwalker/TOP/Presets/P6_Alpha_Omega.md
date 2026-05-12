# P6 アルファオメガ
## レイアウト

```
~Lv2~{"Name":"P6 Exaflares 1","Group":"絶オメガ","ZoneLockH":[1122],"Freezing":true,"FreezeFor":12.5,"IntervalBetweenFreezes":0.75,"ElementsL":[{"Name":"","type":1,"radius":8.0,"color":4278190335,"fillIntensity":0.5,"refActorName":"*","refActorRequireCast":true,"refActorCastId":[31661],"refActorUseCastTime":true,"refActorCastTimeMax":0.5}]}
~Lv2~{"Name":"P6 Exaflares 2","Group":"絶オメガ","ZoneLockH":[1122],"Freezing":true,"FreezeFor":13.5,"IntervalBetweenFreezes":0.75,"ElementsL":[{"Name":"","type":1,"offY":8.0,"radius":8.0,"color":4278190335,"fillIntensity":0.5,"refActorName":"*","refActorRequireCast":true,"refActorCastId":[31661],"refActorUseCastTime":true,"refActorCastTimeMax":0.5,"includeRotation":true}]}
~Lv2~{"Name":"P6 Exaflares 3","Group":"絶オメガ","ZoneLockH":[1122],"Freezing":true,"FreezeFor":14.5,"IntervalBetweenFreezes":0.75,"FreezeDisplayDelay":11.5,"ElementsL":[{"Name":"","type":1,"offY":16.0,"radius":8.0,"color":4278190335,"fillIntensity":0.5,"refActorName":"*","refActorRequireCast":true,"refActorCastId":[31661],"refActorUseCastTime":true,"refActorCastTimeMax":0.5,"includeRotation":true}]}
~Lv2~{"Name":"P6 Exaflares 4","Group":"絶オメガ","ZoneLockH":[1122],"Freezing":true,"FreezeFor":15.5,"IntervalBetweenFreezes":0.75,"FreezeDisplayDelay":12.54,"ElementsL":[{"Name":"","type":1,"offY":24.0,"radius":8.0,"color":4278190335,"fillIntensity":0.5,"refActorName":"*","refActorRequireCast":true,"refActorCastId":[31661],"refActorUseCastTime":true,"refActorCastTimeMax":0.5,"includeRotation":true}]}
~Lv2~{"Name":"P6 Exaflares 5","Group":"絶オメガ","ZoneLockH":[1122],"Freezing":true,"FreezeFor":16.5,"IntervalBetweenFreezes":0.75,"FreezeDisplayDelay":13.5,"ElementsL":[{"Name":"","type":1,"offY":32.0,"radius":8.0,"color":4278190335,"fillIntensity":0.5,"refActorName":"*","refActorRequireCast":true,"refActorCastId":[31661],"refActorUseCastTime":true,"refActorCastTimeMax":0.5,"includeRotation":true}]}
~Lv2~{"Name":"P6 Exaflares 6","Group":"絶オメガ","ZoneLockH":[1122],"Freezing":true,"FreezeFor":17.5,"IntervalBetweenFreezes":0.75,"FreezeDisplayDelay":14.5,"ElementsL":[{"Name":"","type":1,"offY":40.0,"radius":8.0,"color":4278190335,"fillIntensity":0.5,"refActorName":"*","refActorRequireCast":true,"refActorCastId":[31661],"refActorUseCastTime":true,"refActorCastTimeMax":0.5,"includeRotation":true}]}
~Lv2~{"Name":"P6 Exaflares 7","Group":"絶オメガ","ZoneLockH":[1122],"Freezing":true,"FreezeFor":18.5,"IntervalBetweenFreezes":0.75,"FreezeDisplayDelay":15.5,"ElementsL":[{"Name":"","type":1,"offY":48.0,"radius":8.0,"color":4278190335,"fillIntensity":0.5,"refActorName":"*","refActorRequireCast":true,"refActorCastId":[31661],"refActorUseCastTime":true,"refActorCastTimeMax":0.5,"includeRotation":true}]}
~Lv2~{"Name":"P6 Exaflares First flare indicator","Group":"絶オメガ","ZoneLockH":[1122],"Freezing":true,"FreezeFor":12.5,"IntervalBetweenFreezes":20.0,"ElementsL":[{"Name":"","type":1,"radius":8.0,"color":4278190335,"fillIntensity":0.5,"overlayBGColor":4278190080,"overlayTextColor":4278190335,"overlayFScale":2.0,"thicc":4.0,"overlayText":"!!! First Exaflare !!!","refActorName":"*","refActorRequireCast":true,"refActorCastId":[31661],"refActorUseCastTime":true,"refActorCastTimeMax":0.5}]}
```

## スクリプト
## P6 マルチスクリプト
AAの対象者とその範囲、コスモダイブの対象とその範囲、コスモメテオの小メテオの範囲などが表示されます。

#### URL
```
https://raw.githubusercontent.com/PunishXIV/Splatoon/refs/heads/main/SplatoonScripts/Duties/Endwalker/The%20Omega%20Protocol/P6%20MultiScript.cs
```
#### Configuration
`Wave Canon` / `Cosmo Dive Spread Marker` の設定が必要です。ロールによって設定が異なります。`Cosmo Dive` とありますが、コスモメテオの散開位置誘導設定なので注意してください。
![](../../../../assets/endwalker/p6_multi_setting.png)

### P6 コスモアロー
コスモアローのAOEが表示されます。誘導はされないので注意が必要です。

#### URL
```
https://github.com/PunishXIV/Splatoon/raw/main/SplatoonScripts/Duties/Endwalker/The%20Omega%20Protocol/Exasquares.cs
```
#### Configuration
設定不要。

### P6 コスモメテオ調整
コスモメテオ中のフレア時の移動先がガイドされます。

#### URL
```
https://raw.githubusercontent.com/PunishXIV/Splatoon/refs/heads/main/SplatoonScripts/Duties/Endwalker/The%20Omega%20Protocol/Cosmo%20Meteor%20Adjuster.cs
```
#### Configuration
このスクリプトは PriorityEditor からではなく名前を入力する必要があるので注意してください。
![](../../../../assets/endwalker/p6_meteor_setting.png)
