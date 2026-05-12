# P4 リブート
## レイアウト
一部レイアウトがP3のものと重複しているのでエラーが出ますが、無視して問題ありません。

```
~Lv2~{"Name":"P3/P4 Wave Repeater 1","Group":"絶オメガ","ZoneLockH":[1122],"DCond":5,"UseTriggers":true,"Triggers":[{"Type":2,"Duration":5.2,"Match":">31567)"}],"ElementsL":[{"Name":"1","refX":100.0,"refY":100.0,"radius":6.0,"color":4278190335,"fillIntensity":0.5,"refActorNPCNameID":7636,"refActorUseCastTime":true,"refActorCastTimeMax":5.5,"refActorUseOvercast":true,"refActorComparisonType":6}]}
~Lv2~{"Name":"P3/P4 Wave Repeater 2","Group":"絶オメガ","ZoneLockH":[1122],"DCond":5,"UseTriggers":true,"Triggers":[{"Type":2,"Duration":2.0,"Match":">31567)","MatchDelay":5.2}],"ElementsL":[{"Name":"2","refX":100.0,"refY":100.0,"radius":6.0,"Donut":6.0,"color":4278190335,"fillIntensity":0.5,"thicc":5.0,"refActorNPCNameID":7636,"refActorRequireCast":true,"refActorCastId":[31567],"refActorUseCastTime":true,"refActorCastTimeMin":5.5,"refActorCastTimeMax":7.5,"refActorUseOvercast":true,"refActorComparisonType":6}]}
~Lv2~{"Name":"P3/P4 Wave Repeater 3","Group":"絶オメガ","ZoneLockH":[1122],"DCond":5,"UseTriggers":true,"Triggers":[{"Type":2,"Duration":2.0,"Match":">31567)","MatchDelay":7.2}],"ElementsL":[{"Name":"3","refX":100.0,"refY":100.0,"radius":12.0,"Donut":6.0,"color":4278190335,"fillIntensity":0.5,"thicc":5.0,"refActorNPCNameID":7636,"refActorRequireCast":true,"refActorCastId":[31567],"refActorUseCastTime":true,"refActorCastTimeMin":7.5,"refActorCastTimeMax":9.5,"refActorUseOvercast":true,"refActorComparisonType":6}]}
~Lv2~{"Name":"P3/P4 Wave Repeater 4","Group":"絶オメガ","ZoneLockH":[1122],"DCond":5,"UseTriggers":true,"Triggers":[{"Type":2,"Duration":2.0,"Match":">31567)","MatchDelay":9.2}],"ElementsL":[{"Name":"4","refX":100.0,"refY":100.0,"radius":18.0,"Donut":6.0,"color":4278190335,"fillIntensity":0.5,"thicc":5.0,"refActorNPCNameID":7636,"refActorRequireCast":true,"refActorCastId":[31567],"refActorUseCastTime":true,"refActorCastTimeMin":9.5,"refActorCastTimeMax":11.5,"refActorUseOvercast":true,"refActorComparisonType":6}]}
~Lv2~{"Name":"P4 Wave Cannon","Group":"絶オメガ","ZoneLockH":[1122],"ElementsL":[{"Name":"Snapshot","type":3,"refY":20.0,"radius":3.0,"color":4294966272,"fillIntensity":0.5,"refActorNPCNameID":7636,"refActorRequireCast":true,"refActorCastId":[31616],"refActorComparisonType":6,"includeRotation":true},{"Name":"Target you (1st)","type":3,"refY":20.0,"radius":3.0,"color":4278190335,"fillIntensity":0.5,"refActorNPCNameID":7636,"refActorRequireCast":true,"refActorCastId":[31617],"refActorComparisonType":6,"includeRotation":true,"FaceMe":true}]}
```

## スクリプト
### P4 波動砲調整
設定にもとづいて移動先がガイドされます。波動砲が片方に偏った場合の調整も行われます。

#### URL
```
https://raw.githubusercontent.com/PunishXIV/Splatoon/refs/heads/main/SplatoonScripts/Duties/Endwalker/The%20Omega%20Protocol/BSOD%20Adjuster.cs
```
#### Configuration
このスクリプトは PriorityEditor からではなく名前を入力する必要があるので注意してください。
![](../../../../assets/endwalker/p4_wave_setting.png)