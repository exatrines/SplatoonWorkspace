using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.Hooks.ActionEffectTypes;
using FFXIVClientStructs.FFXIV.Client.Game;
using Splatoon;
using Splatoon.SplatoonScripting;
using Splatoon.SplatoonScripting.Priority;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static Splatoon.Splatoon;

namespace SplatoonScriptsOfficial.Duties.Endwalker.The_Omega_Protocol;

public class P3_Transition : SplatoonScript
{
    private const uint TerritoryTop = 1122;
    private const uint StatusSniperCannon = 3426;
    private const uint StatusSniperWave = 3425;
    private const uint EndTransitionCastId = 31566;
    private const int MinPartySize = 8;
    private const int ExpectedSniperCannonCount = 2;
    private const int ExpectedSniperWaveCount = 4;
    private const string GuideElementName = "P3TransitionGuide";

    private bool _onTransition;
    private GroupAssignment? _debugMyGroup;
    private int _debugPartyCountObjects;

    public override HashSet<uint>? ValidTerritories => [TerritoryTop];
    public override Metadata? Metadata => new(1, "mirage");

    private Config C => Controller.GetConfig<Config>();

    public override void OnSetup()
    {
        Controller.RegisterElementFromCode(
            GuideElementName,
            """{"Name":"","radius":2.0,"Donut":0.2,"color":3355508538,"fillIntensity":0.17,"tether":true}""");
        DisableGuide();
    }

    public override void OnStartingCast(uint source, uint castId)
    {
        if(castId == EndTransitionCastId)
        {
            _onTransition = false;
            DisableGuide();
        }
    }

    public override void OnGainBuffEffect(uint sourceId, Status status)
    {
        if(status.StatusId != StatusSniperCannon && status.StatusId != StatusSniperWave) return;
        if(_onTransition) return;

        TryBeginTransitionByDebuffs();
    }

    public override void OnUpdate()
    {
        UpdateDebugMyGroup();
        ApplyGuideVisibility();
    }

    public override void OnReset()
    {
        _onTransition = false;
        _debugMyGroup = null;
        DisableGuide();
    }

    public override void OnSettingsDraw()
    {
        C.PriorityData.Draw();
        DrawDirectionSelector("Stack1 (High 1 + None 1)", GroupAssignment.Stack1);
        DrawDirectionSelector("Stack2 (High 2 + None 2)", GroupAssignment.Stack2);
        DrawDirectionSelector("Spread1", GroupAssignment.Spread1);
        DrawDirectionSelector("Spread2", GroupAssignment.Spread2);
        DrawDirectionSelector("Spread3", GroupAssignment.Spread3);
        DrawDirectionSelector("Spread4", GroupAssignment.Spread4);
        ImGui.Separator();
        if(ImGui.CollapsingHeader("Debug"))
        {
            ImGui.Text($"OnTransition: {_onTransition}");
            ImGui.Text($"BasePlayer: {BasePlayer?.Name.ToString() ?? "Unknown"}");
            ImGui.Text($"My Group: {_debugMyGroup?.ToString() ?? "Unknown"}");
        }
    }

    private void DrawDirectionSelector(string label, GroupAssignment group)
    {
        var value = C.GroupDirection[group];
        if(ImGui.BeginCombo(label, value.ToString()))
        {
            foreach(var spot in Enum.GetValues<DirectionSpot>())
            {
                var selected = value == spot;
                if(ImGui.Selectable(spot.ToString(), selected))
                {
                    C.GroupDirection[group] = spot;
                }

                if(selected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }
    }

    private void ShowGuideForLocalPlayer()
    {
        if(!_onTransition) return;

        if(!TryResolveCurrentAssignment(out var assignment)) return;
        if(assignment == null) return;

        var direction = C.GroupDirection[assignment.Value];
        ShowGuideAt(DirectionPositions[direction]);
    }

    private void TryBeginTransitionByDebuffs()
    {
        var party = GetPartyMembers();
        if(party.Count < MinPartySize) return;

        var highCount = party.Count(x => HasStatus(x, StatusSniperCannon));
        var waveCount = party.Count(x => HasStatus(x, StatusSniperWave));
        if(highCount != ExpectedSniperCannonCount || waveCount != ExpectedSniperWaveCount) return;

        _onTransition = true;
        ShowGuideForLocalPlayer();
    }

    private List<IPlayerCharacter> GetPartyMembers()
    {
        var objectParty = Svc.Objects
            .OfType<IPlayerCharacter>()
            .Where(x => !x.IsDead && x.CurrentHp > 0)
            .OrderBy(x => Vector3.Distance(x.Position, Player.Object?.Position ?? x.Position))
            .Take(MinPartySize)
            .ToList();
        _debugPartyCountObjects = objectParty.Count;
        return objectParty;
    }

    private bool TryResolveCurrentAssignment(out GroupAssignment? assignment)
    {
        assignment = null;
        var party = GetPartyMembers();
        if(party.Count < MinPartySize) return false;

        var targetPlayer = GetProcessingPlayer();
        if(targetPlayer == null) return false;

        assignment = ResolveGroupAssignment(targetPlayer.EntityId, party);
        return true;
    }

    private GroupAssignment? ResolveGroupAssignment(uint localEntityId, List<IPlayerCharacter> party)
    {
        var high = OrderByPriority(party.Where(x => HasStatus(x, StatusSniperCannon))).ToList();
        var spread = OrderByPriority(party.Where(x => HasStatus(x, StatusSniperWave))).ToList();
        var none = OrderByPriority(party.Where(x => !HasStatus(x, StatusSniperCannon) && !HasStatus(x, StatusSniperWave))).ToList();

        if(high.Count >= 1 && high[0].EntityId == localEntityId) return GroupAssignment.Stack1;
        if(high.Count >= 2 && high[1].EntityId == localEntityId) return GroupAssignment.Stack2;
        if(none.Count >= 1 && none[0].EntityId == localEntityId) return GroupAssignment.Stack1;
        if(none.Count >= 2 && none[1].EntityId == localEntityId) return GroupAssignment.Stack2;
        if(spread.Count >= 1 && spread[0].EntityId == localEntityId) return GroupAssignment.Spread1;
        if(spread.Count >= 2 && spread[1].EntityId == localEntityId) return GroupAssignment.Spread2;
        if(spread.Count >= 3 && spread[2].EntityId == localEntityId) return GroupAssignment.Spread3;
        if(spread.Count >= 4 && spread[3].EntityId == localEntityId) return GroupAssignment.Spread4;

        return null;
    }

    private void UpdateDebugMyGroup()
    {
        _debugMyGroup = TryResolveCurrentAssignment(out var assignment) ? assignment : null;
    }

    private IPlayerCharacter? GetProcessingPlayer() => BasePlayer;

    private IEnumerable<IPlayerCharacter> OrderByPriority(IEnumerable<IPlayerCharacter> players)
        => players.OrderBy(GetPriorityIndex).ThenBy(x => x.EntityId);

    private int GetPriorityIndex(IPlayerCharacter player)
    {
        var priority = C.PriorityData.GetPlayers(_ => true)?.ToList();
        if(priority == null) return int.MaxValue;

        var name = player.Name.ToString();
        for(var i = 0; i < priority.Count; i++)
        {
            if(priority[i].Name == name) return i;
        }

        return int.MaxValue;
    }

    private static bool HasStatus(IPlayerCharacter player, uint statusId)
        => player.StatusList.Any(x => x.StatusId == statusId);

    private void ApplyGuideVisibility()
    {
        if(_onTransition) ShowGuideForLocalPlayer();
        else DisableGuide();
    }

    private void ShowGuideAt(Vector2 position)
    {
        if(!Controller.TryGetElementByName(GuideElementName, out var element)) return;

        element.Enabled = true;
        element.tether = true;
        element.refX = position.X;
        element.refY = position.Y;
        element.refZ = 0f;
    }

    private void DisableGuide()
    {
        if(Controller.TryGetElementByName(GuideElementName, out var element))
        {
            element.Enabled = false;
        }
    }

    private static readonly Dictionary<DirectionSpot, Vector2> DirectionPositions = new()
    {
        [DirectionSpot.NorthEast] = new(107.0f, 83.0f),
        [DirectionSpot.East] = new(118.0f, 100.0f),
        [DirectionSpot.SouthEast] = new(107.0f, 116.5f),
        [DirectionSpot.SouthWest] = new(93.0f, 116.5f),
        [DirectionSpot.West] = new(82.0f, 100.0f),
        [DirectionSpot.NorthWest] = new(93.0f, 83.0f),
    };

    private enum GroupAssignment
    {
        Stack1,
        Stack2,
        Spread1,
        Spread2,
        Spread3,
        Spread4,
    }

    private enum DirectionSpot
    {
        NorthEast,
        East,
        SouthEast,
        SouthWest,
        West,
        NorthWest,
    }

    private class Config : IEzConfig
    {
        public PriorityData PriorityData = new();
        public Dictionary<GroupAssignment, DirectionSpot> GroupDirection = new()
        {
            [GroupAssignment.Stack1] = DirectionSpot.NorthEast,
            [GroupAssignment.Stack2] = DirectionSpot.NorthWest,
            [GroupAssignment.Spread1] = DirectionSpot.East,
            [GroupAssignment.Spread2] = DirectionSpot.SouthEast,
            [GroupAssignment.Spread3] = DirectionSpot.SouthWest,
            [GroupAssignment.Spread4] = DirectionSpot.West,
        };
    }
}
