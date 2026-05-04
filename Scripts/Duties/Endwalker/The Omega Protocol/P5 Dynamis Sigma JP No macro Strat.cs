using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Colors;
using ECommons;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.Hooks.ActionEffectTypes;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Dalamud.Bindings.ImGui;
using Newtonsoft.Json;
using Splatoon.SplatoonScripting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace SplatoonScriptsOfficial.Duties.Endwalker.The_Omega_Protocol
{
    public unsafe class P5_Dynamis_Sigma_JP_No_Macro_Strat : SplatoonScript
    {
        public override HashSet<uint> ValidTerritories => [1122];

        public override Metadata? Metadata => new(1, "mirage");

        public const uint TowerSingle = 2013245;
        public const uint TowerDual = 2013246;
        public const uint TowerAny = 2013244;

        public const uint GlitchFar = 3428;
        public const uint GlitchClose = 3427;

        private static readonly Dictionary<string, float> MarkerAngles = new()
        {
            ["A"] = 0f,
            ["1"] = 45f,
            ["B"] = 90f,
            ["2"] = 135f,
            ["C"] = 180f,
            ["3"] = 225f,
            ["D"] = 270f,
            ["4"] = 315f,
        };

        public class Headmarkers
        {
            public const string Playstation = "vfx/lockon/eff/z3oz_firechain_";
            public const string BlueCross = "vfx/lockon/eff/z3oz_firechain_04c.avfx";
            public const string PurpleSquare = "vfx/lockon/eff/z3oz_firechain_03c.avfx";
            public const string RedCircle = "vfx/lockon/eff/z3oz_firechain_01c.avfx";
            public const string GreenTriangle = "vfx/lockon/eff/z3oz_firechain_02c.avfx";

            public const string Marker = "vfx/lockon/eff/lockon8_t0w.avfx";
        }

        private MarkingController* MKC;
        private Vector3 OmegaPos = Vector3.Zero;
        private Dictionary<uint, ChainMarker> Chains = [];
        private HashSet<uint> Markers = [];
        private List<string> State = [];
        private string MyMarker = "";
        private bool isLeft;
        private bool isUp;
        private long StopRegisteringAt;
        private bool announced = false;

        private IPlayerCharacter LocalPlayer => Svc.Objects.LocalPlayer!;

        private Config Conf => Controller.GetConfig<Config>();

        private IGameObject[] GetTowers() => Svc.Objects.Where(x => x.DataId.EqualsAny<uint>(TowerSingle, TowerDual)).ToArray();

        public override void OnSetup()
        {
            MKC = MarkingController.Instance();
            RegisterElements();
            base.OnSetup();
        }

        private void RegisterElements()
        {
            // Tower elements
            for (var i = 0; i < 6; i++)
            {
                Controller.RegisterElementFromCode($"{i}", "{\"Enabled\":false,\"Name\":\"\",\"radius\":2.5,\"Donut\":0.5,\"color\":4278255615,\"overlayBGColor\":4110417920,\"overlayTextColor\":4278255615,\"overlayFScale\":2.0,\"overlayPlaceholders\":true,\"thicc\":4.0,\"overlayText\":\"L1\\\\nL2\",\"refActorDataID\":2013244,\"refActorComparisonType\":3}");
            }

            // Marker elements
            RegisterMarkerElements();

            // Direction elements
            RegisterDirectionElements();

            // Other elements
            Controller.RegisterElementFromCode("MaleFinder", "{\"Name\":\"OmegaM\",\"type\":1,\"Enabled\":false,\"radius\":0.0,\"color\":4278255615,\"thicc\":5.0,\"refActorDataID\":15720,\"refActorComparisonType\":3,\"includeRotation\":true,\"tether\":true}");
            Controller.RegisterElementFromCode("player_angle", "{\"Name\":\"player_angle\",\"type\":1,\"radius\":0.0,\"Filled\":false,\"fillIntensity\":0.5,\"thicc\":0.0,\"overlayText\":\"\",\"refActorType\":1}");
        }

        private void RegisterMarkerElements()
        {
            var markers = new[] { "BlueCross", "PurpleSquare", "RedCircle", "GreenTriangle" };
            var positions = new[] { "R", "L", "U", "D" };

            foreach (var marker in markers)
            {
                foreach (var pos in positions)
                {
                    var offX = pos switch { "R" => -3.0, "L" => 3.0, "U" => 4.5, "D" => 4.5, _ => 0.0 };
                    var offY = pos switch { "R" => 1.0, "L" => 1.0, "U" => 3.5, "D" => 8.5, _ => 0.0 };
                    var color = marker switch
                    {
                        "BlueCross" => 4294967040,
                        "PurpleSquare" => 4294902015,
                        "RedCircle" => 4278190335,
                        "GreenTriangle" => 4278255360,
                        _ => 4278255615
                    };
                    var icon = marker switch
                    {
                        "BlueCross" => "",
                        "PurpleSquare" => "",
                        "RedCircle" => "",
                        "GreenTriangle" => "",
                        _ => ""
                    };

                    Controller.RegisterElementFromCode($"{marker}{pos}", $"{{\"Name\":\"{marker.ToLower()}{pos.ToLower()}\",\"type\":1,\"Enabled\":false,\"offX\":{offX},\"offY\":{offY},\"radius\":1.0,\"color\":{color},\"overlayBGColor\":0,\"overlayTextColor\":{color},\"overlayFScale\":2.0,\"thicc\":5.0,\"overlayText\":\"{icon}\",\"refActorDataID\":15720,\"refActorComparisonType\":3,\"includeRotation\":true}}");
                }
            }
        }

        private void RegisterDirectionElements()
        {
            var directions = new[] { "Front", "Bottom", "Left", "Right", "FrontLeft", "FrontRight", "BottomLeft", "BottomRight" };
            var glitches = new[] { "Far", "Close" };

            foreach (var glitch in glitches)
            {
                foreach (var dir in directions)
                {
                    var offX = dir switch
                    {
                        "Left" => glitch == "Far" ? 19.0 : 11.0,
                        "Right" => glitch == "Far" ? -19.0 : -11.0,
                        "FrontLeft" => glitch == "Far" ? 13.5 : 8.0,
                        "FrontRight" => glitch == "Far" ? -13.5 : -8.0,
                        "BottomLeft" => glitch == "Far" ? 13.5 : 8.0,
                        "BottomRight" => glitch == "Far" ? -13.5 : -8.0,
                        _ => 0.0
                    };
                    var offY = dir switch
                    {
                        "Front" => glitch == "Far" ? 1.0 : 8.0,
                        "Bottom" => glitch == "Far" ? 39.0 : 31.0,
                        "Left" => glitch == "Far" ? 20.0 : 20.0,
                        "Right" => glitch == "Far" ? 20.0 : 20.0,
                        "FrontLeft" => glitch == "Far" ? 6.5 : 12.0,
                        "FrontRight" => glitch == "Far" ? 6.5 : 12.0,
                        "BottomLeft" => glitch == "Far" ? 33.5 : 28.0,
                        "BottomRight" => glitch == "Far" ? 33.5 : 28.0,
                        _ => 0.0
                    };

                    Controller.RegisterElementFromCode($"{glitch}{dir}", $"{{\"Name\":\"\",\"type\":1,\"offY\":{offY},\"offX\":{offX},\"radius\":1.0,\"color\":4278255615,\"thicc\":4.0,\"refActorDataID\":15720,\"refActorComparisonType\":3,\"includeRotation\":true,\"tether\":true}}");
                }
            }
        }


        public override void OnVFXSpawn(uint target, string vfxPath)
        {
            if(Controller.Scene == 6)
            {
                if(vfxPath.StartsWith(Headmarkers.Playstation))
                {
                    Chains[target] = GetChainMarker(vfxPath);
                }
                else if(vfxPath == Headmarkers.Marker)
                {
                    Markers.Add(target);
                    StopRegisteringAt = Environment.TickCount64 + 1500;
                }
            }
        }

        internal void ApplyMarkerPhaseNorthToSouth(IGameObject omega, IPlayerCharacter partner, bool first, string glitch)
        {
            var rota = (MathHelper.GetRelativeAngle(omega.Position, LocalPlayer.Position) + omega.Rotation.RadToDeg()) % 360;
            var rotaPar = (MathHelper.GetRelativeAngle(omega.Position, partner.Position) + omega.Rotation.RadToDeg()) % 360;
            if(Environment.TickCount64 < StopRegisteringAt) isLeft = rota > rotaPar;
            if(isLeft)
            {
                //left
                if(first)
                {
                    Controller.GetElementByName($"{glitch}{Conf.DirectionsSpots[Position.DoubleMarkerFirstLeft]}").Enabled = true;
                }
                else
                {
                    Controller.GetElementByName($"{glitch}{Conf.DirectionsSpots[Position.DoubleMarkerSecondLeft]}").Enabled = true;
                }
            }
            else
            {
                //right
                if(first)
                {
                    Controller.GetElementByName($"{glitch}{Conf.DirectionsSpots[Position.DoubleMarkerFirstRight]}").Enabled = true;
                }
                else
                {
                    Controller.GetElementByName($"{glitch}{Conf.DirectionsSpots[Position.DoubleMarkerSecondRight]}").Enabled = true;
                }
            }
            State.Add($"Your rotation: {rota}, partner rotation: {rotaPar}");
        }

        internal void ApplyMarkerPhaseWestToEast(IGameObject omega, IPlayerCharacter partner, bool first, string glitch)
        {
            var relativeY = Dynamis_Sigma_Utils.GetRelativePosition(omega.Position, LocalPlayer.Position,
                omega.Rotation).Y;
            var partnerRelativeY =
                Dynamis_Sigma_Utils.GetRelativePosition(omega.Position, partner.Position, omega.Rotation).Y;
            if(Environment.TickCount64 < StopRegisteringAt) isUp = relativeY < partnerRelativeY;
            if(isUp)
            {
                // left
                if(first)
                {
                    Controller.GetElementByName($"{glitch}{Conf.DirectionsSpots[Position.DoubleMarkerFirstUp]}").Enabled = true;
                }
                else
                {
                    Controller.GetElementByName($"{glitch}{Conf.DirectionsSpots[Position.DoubleMarkerSecondUp]}").Enabled = true;
                }
            }
            else
            {
                // right
                if(first)
                {
                    Controller.GetElementByName($"{glitch}{Conf.DirectionsSpots[Position.DoubleMarkerFirstDown]}").Enabled = true;
                }
                else
                {
                    Controller.GetElementByName($"{glitch}{Conf.DirectionsSpots[Position.DoubleMarkerSecondDown]}").Enabled = true;
                }
            }
        }

        public override void OnUpdate()
        {
            Off();
            State.Clear();
            if (Controller.Scene == 6)
            {
                State.Add("In scene 6");
                if (Conf.DebugFindTowerBasis != "")
                {
                    MyMarker = Conf.DebugFindTowerBasis;
                }
                if (GetTowers().Length.EqualsAny(5, 6))
                {
                    HandleTowerPhase();
                }
                else if (Markers.Count == 6)
                {
                    HandleMarkerPhase();
                }
                else if (Chains.Count == 8 && Vector3.Distance(new(100, 0, 100), Svc.Objects.FirstOrDefault(x => x.DataId == 15720).Position) > 10)
                {
                    HandleOmegaPhase();
                }
            }
            else
            {
                Markers.Clear();
                Chains.Clear();
                announced = false;
            }
        }

        private void HandleTowerPhase()
        {
            State.Add($"Tower phase, yours is {MyMarker}");
            State.Add($"Player guidance marker: {MyMarker}");
            var towers = GetTowers().OrderBy(x => GetTowerAngle(x, IsInverted())).ToArray();
            var isFar = LocalPlayer.StatusList.Any(x => x.StatusId == GlitchFar);
            var targetTower = GetTargetTower(towers, isFar);
            var playerAngle = GetPlayerAngleFromMarker();
            var roundedAngle = RoundTo8Directions(playerAngle) + 22.5f;
            var range = 50f;
            State.Add($"Player display angle: {roundedAngle:F1}°, range: ±{range:F1}°");
            for (int i = 0; i < towers.Length; i++)
            {
                var tower = towers[i];
                var towerAngle = GetTowerDirectionAngle(tower);
                var diff = AngleDifference(towerAngle, roundedAngle);
                var inRange = diff <= range;
                State.Add($"Tower {i}: angle {towerAngle:F1}°, diff {diff:F1}°, in range: {inRange}");
            }

            if (Controller.TryGetElementByName("player_angle", out var playerAngleElement))
            {
                if (Conf.Angle)
                {
                    playerAngleElement.Enabled = true;
                    playerAngleElement.overlayText = $"Angle: {roundedAngle:F1}°";
                }
            }

            for (int i = 0; i < towers.Length; i++)
            {
                var tower = towers[i];
                var isTarget = tower == targetTower;
                SetTowerAs(i, tower, isTarget, "");
            }
        }

        private void HandleMarkerPhase()
        {
            var glitch = LocalPlayer.StatusList.Any(x => x.StatusId == GlitchFar) ? "Far" : "Close";
            State.Add("Markers phase");
            State.Add($"Player guidance marker: {MyMarker}");
            State.Add($"Markers: {Markers.Select(x => x.GetObject()).Print()}");
            var omega = Svc.Objects.FirstOrDefault(x => x.DataId == 15720);
            State.Add($"Omega-M is {omega}");
            var partner = GetPartner();
            if (Markers.Contains(LocalPlayer.EntityId) && Markers.Contains(partner.EntityId))
            {
                State.Add("You and your partners are markers");
                //both are markers
                var first = true;
                foreach (var x in Conf.MarkerOrder)
                {
                    State.Add($"Checking {x}...");
                    if (x == Chains[LocalPlayer.EntityId]) break;
                    State.Add(Chains.Where(c => c.Value == x).Select(z => $"{z.Key.GetObject()}/{Markers.Contains(z.Key)}").Print());
                    if (Chains.Where(c => c.Value == x).All(z => Markers.Contains(z.Key)))
                    {
                        State.Add($"Not first because {x} are same");
                        first = false;
                    }
                }
                State.Add($"You are {(first ? "first" : "second")}");
                switch (Conf.AlignmentDirection)
                {
                    case MarkerAlignmentDirection.NorthToSouth:
                        ApplyMarkerPhaseNorthToSouth(omega, partner, first, glitch);
                        break;
                    case MarkerAlignmentDirection.WestToEast:
                        ApplyMarkerPhaseWestToEast(omega, partner, first, glitch);
                        break;
                }
            }
            else
            {
                State.Add($"Only one of you or your partners are markers");
                //single marker
                var first = true;
                foreach (var x in Conf.MarkerOrder)
                {
                    State.Add($"Checking {x}...");
                    if (x == Chains[LocalPlayer.EntityId]) break;
                    if (Chains.Where(c => c.Value == x).Count(z => Markers.Contains(z.Key)) == 1)
                    {
                        State.Add($"Not first because {x} are same");
                        first = false;
                    }
                }
                State.Add($"You are {(first ? "first" : "second")}");
                var marked = Markers.Contains(LocalPlayer.EntityId);
                State.Add($"You are marked: {marked}");
                if (marked)
                {
                    if (first)
                    {
                        Controller.GetElementByName($"{glitch}{Conf.DirectionsSpots[Position.SingleMarkerFirstMarked]}").Enabled = true;
                    }
                    else
                    {
                        Controller.GetElementByName($"{glitch}{Conf.DirectionsSpots[Position.SingleMarkerSecondMarked]}").Enabled = true;
                    }
                }
                else
                {
                    if (first)
                    {
                        Controller.GetElementByName($"{glitch}{Conf.DirectionsSpots[Position.SingleMarkerFirstUnmarked]}").Enabled = true;
                    }
                    else
                    {
                        Controller.GetElementByName($"{glitch}{Conf.DirectionsSpots[Position.SingleMarkerSecondUnmarked]}").Enabled = true;
                    }
                }
            }
        }

        private void HandleOmegaPhase()
        {
            State.Add("Omega-M found");
            State.Add($"Player guidance marker: {MyMarker}");
            if (Conf.AlignmentDirection == MarkerAlignmentDirection.NorthToSouth)
            {
                var i = 1f;
                foreach (var x in Conf.MarkerOrder)
                {
                    if (Controller.TryGetElementByName($"{x}L", out var l) && Controller.TryGetElementByName($"{x}R", out var r))
                    {
                        l.offY = i;
                        r.offY = i;
                        l.Enabled = true;
                        r.Enabled = true;
                        //l.tether = Chains[LocalPlayer.EntityId] == x;
                        //r.tether = Chains[LocalPlayer.EntityId] == x;
                    }
                    i += 3f;
                }
            }
            else if (Conf.AlignmentDirection == MarkerAlignmentDirection.WestToEast)
            {
                var i = 4.5f;
                foreach (var x in Conf.MarkerOrder)
                {
                    if (Controller.TryGetElementByName($"{x}U", out var u) &&
                        Controller.TryGetElementByName($"{x}D", out var d))
                    {
                        u.offX = i;
                        d.offX = i;
                        if (Conf.StartBothOmegaArms)
                        {
                            u.offY = 28f;
                            d.offY = 30f;
                        }
                        u.Enabled = true;
                        d.Enabled = true;
                    }
                    i -= 3f;
                }
            }
            if (Conf.ShowOmegaFinder)
            {
                Controller.GetElementByName("MaleFinder").Enabled = true;
            }
        }

        private IPlayerCharacter GetPartner()
        {
            return FakeParty.Get().First(x => x.EntityId != LocalPlayer.EntityId && Chains[x.EntityId] == Chains[LocalPlayer.EntityId]);
        }

        public override void OnActionEffect(uint ActionID, ushort animationID, ActionEffectType type, uint sourceID, ulong targetOID, uint damage)
        {
            if(Controller.Scene == 6)
            {
                if(ActionID == 31603)
                {
                    OmegaPos = Svc.Objects.FirstOrDefault(x => x.DataId == 15720)?.Position ?? Vector3.Zero;
                    var distance = float.MaxValue;
                    var marker = "A";
                    for(var i = 0; i < MkNum.Length; i++)
                    {
                        var d = MKC->FieldMarkers[i].GetPositon().GetDistanceToWaymark();
                        if(d < distance)
                        {
                            marker = MkNum[i];
                            distance = d;
                        }
                    }
                    MyMarker = marker;
                    Chains.Clear();
                    Markers.Clear();
                }
                else if(ActionID == 32788 || ActionID == 31492)
                {
                    //DuoLog.Information($"Starting sigma");
                    Chains.Clear();
                    Markers.Clear();
                }
            }
        }

        private void Off()
        {
            Controller.GetRegisteredElements().Each(x => x.Value.Enabled = false);
        }

        private void SetTowerAs(int tower, IGameObject obj, bool isTarget, params string[] s)
        {
            if(Controller.TryGetElementByName($"{tower}", out var t))
            {
                t.Enabled = true;
                t.SetRefPosition(obj.Position);
                var overlayText = s.Join("\n");
                if (Conf.ShowTowerType)
                {
                    var towerTypeText = obj.DataId == TowerDual ? "2" : "1";
                    overlayText = string.IsNullOrEmpty(overlayText) ? towerTypeText : $"{overlayText}\n{towerTypeText}";
                }
                if (Conf.Angle)
                {
                    var angleText = $"Angle: {GetTowerDirectionAngle(obj):F1}°";
                    overlayText = string.IsNullOrEmpty(overlayText) ? angleText : $"{overlayText}\n{angleText}";
                }
                t.overlayText = overlayText;
                t.tether = isTarget;
                t.color = isTarget ? 0xFF00FF00 : 0xFF00FFFF;
            }
            else
            {
                DuoLog.Error($"Could not obtain element {tower}");
            }
        }

        private float NormalizeAngle(float angle)
        {
            return (angle % 360 + 360) % 360;
        }

        private float AngleDifference(float a, float b)
        {
            var diff = NormalizeAngle(a - b);
            return diff > 180 ? 360 - diff : diff;
        }

        private float GetTowerDirectionAngle(IGameObject tower)
        {
            var center = new Vector3(100, 0, 100);
            return NormalizeAngle(MathHelper.GetRelativeAngle(center, tower.Position));
        }

        private Vector3 GetPlayerMarkerPosition()
        {
            return GetMarker(MyMarker);
        }

        private float GetPlayerAngleFromMarker()
        {
            if (string.IsNullOrEmpty(MyMarker) || !MarkerAngles.TryGetValue(MyMarker, out var angle))
            {
                return 0;
            }
            return NormalizeAngle(angle);
        }

        private Vector3? GetClosestMarkerPositionFrom(Vector3 referencePosition)
        {
            var bestMarker = (position: (Vector3?)null, distance: float.MaxValue);
            foreach (var id in MkNum)
            {
                var markerPosition = GetMarker(id);
                var distance = Vector3.Distance(markerPosition, referencePosition);
                if (distance < bestMarker.distance)
                {
                    bestMarker = (markerPosition, distance);
                }
            }
            return bestMarker.position;
        }

        private float RoundTo8Directions(float angle)
        {
            return MathF.Round(angle / 45f) * 45f;
        }

        private float RoundTo16Directions(float angle)
        {
            return MathF.Round(angle / 22.5f) * 22.5f;
        }

        private bool IsAngleInRange(float angle, float center, float halfRange)
        {
            return AngleDifference(angle, center) <= halfRange;
        }

        private IGameObject? GetTargetTower(IGameObject[] towers, bool isFar)
        {
            if (string.IsNullOrEmpty(MyMarker))
            {
                return null;
            }
            var playerAngle = GetPlayerAngleFromMarker();
            var displayedAngle = RoundTo8Directions(playerAngle) + 22.5f;
            var range = 50f;
            var inRangeTowers = towers.Where(t => IsAngleInRange(GetTowerDirectionAngle(t), displayedAngle, range)).ToArray();
            State.Add($"Glitch {(isFar ? "far" : "cross")}, in-range towers = {string.Join(", ", inRangeTowers.Select(t => $"{t.DataId}({GetTowerDirectionAngle(t):F1})"))}");
            var candidates = inRangeTowers.AsEnumerable();
            if (!isFar)
            {
                candidates = candidates.Where(t => !IsAngleInRange(GetTowerDirectionAngle(t), displayedAngle, 0.1f));
                var excluded = inRangeTowers.Except(candidates).ToArray();
                if (excluded.Length > 0)
                {
                    State.Add($"Excluded same-angle towers: {string.Join(", ", excluded.Select(t => $"{t.DataId}({GetTowerDirectionAngle(t):F1})"))}");
                }
            }
            var finalCandidates = candidates.ToArray();
            if (finalCandidates.Length == 1)
            {
                return finalCandidates[0];
            }
            if (finalCandidates.Length == 2)
            {
                return finalCandidates.FirstOrDefault(t => t.DataId == TowerDual) ?? finalCandidates[0];
            }
            return finalCandidates.FirstOrDefault();
        }

        private float GetTowerAngle(IGameObject t, bool inverted = false)
        {
            var z = new Vector3(100, 0, 100);
            var angle = (MathHelper.GetRelativeAngle(z, t.Position) + (inverted ? 181 : 1) + 360 - MathHelper.GetRelativeAngle(z, OmegaPos)) % 360;
            return angle;
        }

        private bool IsInverted()
        {
            if(LocalPlayer.StatusList.Any(x => x.StatusId == GlitchFar))
            {
                return !GetTowers().Any(x => GetTowerAngle(x) < 3);
            }
            else
            {
                return !GetTowers().Any(x => GetTowerAngle(x).InRange(90, 90 + 45));
            }
        }

        private ChainMarker GetChainMarker(string s)
        {
            if(s == Headmarkers.RedCircle) return ChainMarker.RedCircle;
            if(s == Headmarkers.BlueCross) return ChainMarker.BlueCross;
            if(s == Headmarkers.GreenTriangle) return ChainMarker.GreenTriangle;
            if(s == Headmarkers.PurpleSquare) return ChainMarker.PurpleSquare;
            return default;
        }

        public override void OnSettingsDraw()
        {
            DrawSettings();
        }

        private void DrawSettings()
        {
            ImGuiEx.Text("# Marker alignment");
            ImGui.SetNextItemWidth(150f);
            var alignmentDirection = Conf.AlignmentDirection;
            if (ImGuiEx.EnumCombo("(Omega-M is true north)", ref alignmentDirection))
            {
                Conf.AlignmentDirection = alignmentDirection;
            }
            if (Conf.AlignmentDirection == MarkerAlignmentDirection.WestToEast)
            {
                ImGui.SameLine();
                ImGui.Checkbox("Both Omega Arms", ref Conf.StartBothOmegaArms);
            }

            ImGuiEx.Text("\n# Marker Sort");
            ImGuiEx.Text($"{(Conf.AlignmentDirection == MarkerAlignmentDirection.NorthToSouth ? "Top" : "Left")}");
            for (var i = 0; i < Conf.MarkerOrder.Length; i++)
            {
                ImGui.PushID($"num{i}");
                if (ImGuiEx.IconButton(Dalamud.Interface.FontAwesomeIcon.ArrowUp))
                {
                    if (i != 0)
                    {
                        (Conf.MarkerOrder[i], Conf.MarkerOrder[i - 1]) = (Conf.MarkerOrder[i - 1], Conf.MarkerOrder[i]);
                    }
                }
                ImGui.SameLine();
                if (ImGuiEx.IconButton(Dalamud.Interface.FontAwesomeIcon.ArrowDown))
                {
                    if (i != Conf.MarkerOrder.Length - 1)
                    {
                        (Conf.MarkerOrder[i], Conf.MarkerOrder[i + 1]) = (Conf.MarkerOrder[i + 1], Conf.MarkerOrder[i]);
                    }
                }
                ImGui.SameLine();
                ImGuiEx.Text($"{Conf.MarkerOrder[i]}");
                ImGui.PopID();
            }
            ImGuiEx.Text($"{(Conf.AlignmentDirection == MarkerAlignmentDirection.NorthToSouth ? "Bottom" : "Right")}");

            ImGuiEx.Text($"\n # Wave Cannon Spread configuration");
            List<Position> directionsSpots = [];
            switch (Conf.AlignmentDirection)
            {
                case MarkerAlignmentDirection.NorthToSouth:
                    directionsSpots = PositionsNorthToSouthOnly;
                    break;
                case MarkerAlignmentDirection.WestToEast:
                    directionsSpots = PositionsWestToEastOnly;
                    break;
            }
            foreach (var x in directionsSpots)
            {
                var z = Conf.DirectionsSpots[x];
                ImGui.SetNextItemWidth(200f);
                if (ImGuiEx.EnumCombo($"{x}", ref z))
                {
                    Conf.DirectionsSpots[x] = z;
                }
            }

            ImGuiEx.Text($"\n # Other Configurations");
            ImGui.Checkbox("Show Omega-M Tether", ref Conf.ShowOmegaFinder);
            ImGui.Checkbox("Display tower type (single = 1, dual = 2)", ref Conf.ShowTowerType);

            if (ImGui.CollapsingHeader("Debug"))
            {
            ImGui.Checkbox("Display tower angle and player angle", ref Conf.Angle);
                ImGui.SetNextItemWidth(150f);
                if (ImGui.BeginCombo("Find Tower Basis", string.IsNullOrEmpty(Conf.DebugFindTowerBasis) ? "" : Conf.DebugFindTowerBasis))
                {
                    if (ImGui.Selectable("", string.IsNullOrEmpty(Conf.DebugFindTowerBasis)))
                        Conf.DebugFindTowerBasis = "";

                    foreach (var marker in new char[] { 'A', '1', 'B', '2', 'C', '3', 'D', '4' })
                    {
                        var name = marker.ToString();
                        if (ImGui.Selectable(name, Conf.DebugFindTowerBasis == name))
                            Conf.DebugFindTowerBasis = name;
                    }

                    ImGui.EndCombo();
                }
                ImGui.Separator();
                foreach (var x in MkNum)
                {
                    ImGuiEx.Text($"Waymark {x}: {GetMarker(x)} distance {GetMarker(x).GetDistanceToWaymark()}");
                }
                ImGui.Separator();
                ImGuiEx.Text($"State:");
                ImGuiEx.TextWrapped(State.Join("\n"));
            }
            if (ImGui.CollapsingHeader("Howto"))
            {
                ImGui.Text("# About");
                ImGui.Text("このスクリプトは絶オメガ検証戦のP5のギミックであるコードデュナミス・シグマの「マクロ押さない式」のスクリプトです。");
                ImGui.Text("波動砲およびアーム誘導の散開位置と、処理後にどの塔に行くべきかをガイドします。（ハロワ処理はガイドされません。）");
                ImGui.Text("\n# Configration");
                ImGui.Text("以下はこのスクリプトの設定項目の説明です。");
                ImGui.Text("\n## Marker Alignment");
                ImGui.Text("マーカーの整列の基準を設定します。マーカーの整列方法は以下の3種類があります。");
                ImGui.Text("- NorthToSouth: フィールド外周にいるオメガMの正面に縦に整列します。");
                ImGui.Text("- WestToEast: フィールド外周にいるオメガMの正面に横に整列します。");
                ImGui.Text("- WestToEast & Both Omega Arms: フィールドに出現している2つのオメガアームの間に整列します。");
                ImGui.Text("\n## Marker Sort");
                ImGui.Text("マーカーの整列順を選択します。Marker Alignmentで選択した整列方法において、上または左を先頭として、どのマーカーがどの位置に来るかを選択します。");
                ImGui.Text("\n## Wave Cannon Spread Configuration");
                ImGui.Text("整列の先頭から順に、それぞれの対象者がどこに散開するべきかを指定します。");
                ImGui.Text("\n## Other Configurations");
                ImGui.Text("- Show Omega-M Tether: ギミック開始時にオメガMに対してテザーを表示します。");
                ImGui.Text("- Display tower type: 波動砲の対象となる塔の種類を表示するか。1人塔は「1」、2人塔は「2」と表示されます。");
                ImGui.Text("\n# Lilydoll Sample");
                ImGui.Text("りりどマクロの散会で塔は押さない式の場合は以下のような設定となります。");
                ImGui.Text("- Marker Alignment: NorthToSouth");
                ImGui.Text("- Marker Sort: 上から RedCircle, BlueCross, GreenTriangle, PurpleSquare");
                ImGui.Text("- Wave Cannon Spread Configuration: 上から Front, Bottom, Left, Right, BottomLeft, FrontRight, BottomRight, FrontLeft");
            }
        }

        public class Config : IEzConfig
        {
            public bool ShowOmegaFinder = true;
            public bool StartBothOmegaArms = false;
            public ChainMarker[] MarkerOrder = new ChainMarker[] { ChainMarker.RedCircle, ChainMarker.BlueCross, ChainMarker.GreenTriangle, ChainMarker.PurpleSquare };
            public bool Angle = false;
            public bool ShowTowerType = true;
            public string DebugFindTowerBasis = "";
            public string Strat = "";
            public MarkerAlignmentDirection AlignmentDirection = MarkerAlignmentDirection.NorthToSouth;
    
            public Dictionary<Position, Directions> DirectionsSpots = new()
            {
                {Position.DoubleMarkerFirstLeft, Directions.Front },
                {Position.DoubleMarkerFirstRight, Directions.Bottom},
                {Position.DoubleMarkerSecondLeft, Directions.Left },
                {Position.DoubleMarkerSecondRight, Directions.Right},
                {Position.DoubleMarkerFirstUp, Directions.Front},
                {Position.DoubleMarkerFirstDown, Directions.Bottom},
                {Position.DoubleMarkerSecondUp, Directions.Left},
                {Position.DoubleMarkerSecondDown, Directions.Right},
                { Position.SingleMarkerFirstMarked, Directions.BottomLeft },
                { Position.SingleMarkerFirstUnmarked, Directions.FrontRight },
                { Position.SingleMarkerSecondMarked, Directions.BottomRight },
                { Position.SingleMarkerSecondUnmarked, Directions.FrontLeft },
            };
        }

        public enum MarkerAlignmentDirection
        {
            NorthToSouth,
            WestToEast
        }

        public enum Position
        {
            DoubleMarkerFirstLeft,
            DoubleMarkerFirstRight,
            DoubleMarkerSecondLeft,
            DoubleMarkerSecondRight,
            SingleMarkerFirstMarked,
            SingleMarkerFirstUnmarked,
            SingleMarkerSecondMarked,
            SingleMarkerSecondUnmarked,
            DoubleMarkerFirstUp,
            DoubleMarkerFirstDown,
            DoubleMarkerSecondUp,
            DoubleMarkerSecondDown,
        }

        internal readonly List<Position> PositionsNorthToSouthOnly = [
            Position.DoubleMarkerFirstLeft,
            Position.DoubleMarkerFirstRight,
            Position.DoubleMarkerSecondLeft,
            Position.DoubleMarkerSecondRight,
            Position.SingleMarkerFirstMarked,
            Position.SingleMarkerFirstUnmarked,
            Position.SingleMarkerSecondMarked,
            Position.SingleMarkerSecondUnmarked,
        ];

        internal readonly List<Position> PositionsWestToEastOnly = [
            Position.DoubleMarkerFirstUp,
            Position.DoubleMarkerFirstDown,
            Position.DoubleMarkerSecondUp,
            Position.DoubleMarkerSecondDown,
            Position.SingleMarkerFirstMarked,
            Position.SingleMarkerFirstUnmarked,
            Position.SingleMarkerSecondMarked,
            Position.SingleMarkerSecondUnmarked,
        ];

        public enum Directions
        {
            Front, Bottom, Left, Right, BottomLeft, FrontRight, BottomRight, FrontLeft 
        }

        public enum ChainMarker
        {
            RedCircle, BlueCross, GreenTriangle, PurpleSquare
        }


        private static string[] MkNum = new string[] { "A", "B", "C", "D", "1", "2", "3", "4" };
        private Vector3 GetMarker(string s)
        {
            var index = 0;
            for(var i = 0; i < MkNum.Length; i++)
            {
                if(MkNum[i].EqualsIgnoreCase(s))
                {
                    index = i;
                    break;
                }
            }
            return new Vector3(MKC->FieldMarkers[index].X, MKC->FieldMarkers[index].Y, MKC->FieldMarkers[index].Z);
        }
    }

    public static unsafe class Dynamis_Sigma_Utils
    {
        public static string GetFirstLetter(this string s)
        {
            return s.Length > 0 ? s[0..1] : "";
        }

        public static float GetDistanceToWaymark(this Vector3 waymarkPos)
        {
            var d1 = Vector3.Distance(new(100, 0, 100), waymarkPos / 1000f);
            var d2 = Vector3.Distance(Svc.Objects.LocalPlayer?.Position ?? Vector3.Zero, waymarkPos / 1000f);
            return d1 + d2;
        }

        public static Vector3 GetPositon(this FieldMarker marker)
        {
            return new Vector3(marker.X, 0, marker.Z);
        }

        public static Vector2 GetRelativePosition(Vector2 origin, Vector2 target, float rotation)
        {
            var offset = target - origin;
            var relative = Vector2.Transform(offset, Quaternion.CreateFromYawPitchRoll(0, 0, rotation));
            return relative;
        }

        public static Vector2 GetRelativePosition(Vector3 origin, Vector3 target, float rotation)
        {
            return GetRelativePosition(origin.ToVector2(), target.ToVector2(), rotation);
        }
    }
}

