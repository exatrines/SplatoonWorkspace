using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Interface.Windowing;
using ECommons;
using ECommons.Automation;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.GameHelpers.LegacyPlayer;
using ECommons.Hooks.ActionEffectTypes;
using ECommons.Schedulers;
using ECommons.SimpleGui;
using FFXIVClientStructs.FFXIV.Client.Game;
using Splatoon.SplatoonScripting;
using Splatoon.SplatoonScripting.Priority;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SplatoonScriptsOfficial.Duties.Endwalker.The_Omega_Protocol;

public class P5_Marking_Helper : SplatoonScript
{
    #region Metadata
    public override Metadata? Metadata => new(1, "mirage");
    public override HashSet<uint>? ValidTerritories => [TerritoryTop];
    #endregion

    #region Constant
    private const uint TerritoryTop = 1122;

    private const uint StatusFirstTarget = 3004;
    private const uint StatusSecondTarget = 3005;
    private const uint StatusHelloNear = 3442;
    private const uint StatusHelloFar = 3443;
    private const uint StatusDynamis = 3444;

    private const uint CastCodeSigma = 32788;
    private const uint CastCodeOmega = 32789;
    private const int ResolveDelayMs = 1000;
    private const int NoneCount = 2;
    private const int BindCount = 2;
    private const int AttackCount = 4;
    private const int MinPartySize = 8;
    private const float OverlayMinWidth = 300f;
    private static readonly Vector2 OverlayDefaultOffset = new(20f, 80f);
    private static readonly Vector2 OverlayInitialSize = new(OverlayMinWidth, 0f);
    #endregion

    private Config C => Controller.GetConfig<Config>();

    #region State
    private PendingResolve _pendingResolve;
    private bool _waitingNearAfterCast;
    private List<IPlayerCharacter> _lastOmega1Bind = new();
    private AssignmentSnapshot? _sigmaSnapshot;
    private AssignmentSnapshot? _omega1Snapshot;
    private AssignmentSnapshot? _omega2Snapshot;
    private OverlayWindow? _overlayWindow;
    #endregion

    private string OverlayWindowName => "P5 Marking Helper###P5_Marking_Helper";

    #region Private Class
    private enum PendingResolve
    {
        None,
        Sigma,
        Omega1,
    }

    private sealed class OverlayWindow : Window, IDisposable
    {
        private const ImGuiWindowFlags BaseFlags = ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar |
                                                   ImGuiWindowFlags.NoCollapse;

        private readonly P5_Marking_Helper _owner;

        public OverlayWindow(P5_Marking_Helper owner)
            : base(owner.OverlayWindowName, BaseFlags, true)
        {
            _owner = owner;

            // Hot-reload safety: remove stale windows that share the same ID.
            foreach(var window in EzConfigGui.WindowSystem.Windows.Where(x => x.WindowName == owner.OverlayWindowName).ToArray())
            {
                EzConfigGui.WindowSystem.RemoveWindow(window);
            }

            EzConfigGui.WindowSystem.AddWindow(this);
            IsOpen = false;
            ShowCloseButton = false;
            RespectCloseHotkey = false;
            AllowClickthrough = false;
            AllowPinning = false;
        }

        public void Dispose() => EzConfigGui.WindowSystem.RemoveWindow(this);

        public override void PreDraw()
        {
            Flags = BaseFlags | (_owner.C.OverlayLockPosition ? ImGuiWindowFlags.NoMove : ImGuiWindowFlags.None);
            base.PreDraw();

            var vp = ImGui.GetMainViewport();
            if(_owner.C.OverlayTransparent)
                ImGui.SetNextWindowBgAlpha(_owner.C.OverlayBgAlpha);

            ImGui.SetNextWindowSizeConstraints(new Vector2(OverlayMinWidth, 0f), new Vector2(2000f, 2000f));
            ImGui.SetNextWindowPos(vp.WorkPos + OverlayDefaultOffset, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSize(OverlayInitialSize, ImGuiCond.FirstUseEver);
        }

        public override void Draw() => _owner.DrawOverlay();

        public override void PostDraw()
        {
            base.PostDraw();
            var vp = ImGui.GetMainViewport();
            var pos = ImGui.GetWindowPos();
            var size = ImGui.GetWindowSize();
            var min = vp.WorkPos;
            var max = vp.WorkPos + vp.WorkSize;
            var fullyOff = pos.X > max.X || pos.Y > max.Y || pos.X + size.X < min.X || pos.Y + size.Y < min.Y;
            if(fullyOff)
                ImGui.SetWindowPos(vp.WorkPos + OverlayDefaultOffset, ImGuiCond.Always);
        }
    }

    private sealed class AssignmentSnapshot
    {
        public string PhaseName = string.Empty;
        public string NoneText = string.Empty;
        public string BindText = string.Empty;
        public string AttackText = string.Empty;
    }
    #endregion

    #region LifeCycle
    public override void OnSetup()
    {
        _overlayWindow ??= new OverlayWindow(this);
        _overlayWindow.IsOpen = C.ShowOverlay && Svc.ClientState.TerritoryType == TerritoryTop;
    }

    public override void OnUpdate()
    {
        _overlayWindow ??= new OverlayWindow(this);
        _overlayWindow.IsOpen = C.ShowOverlay && Svc.ClientState.TerritoryType == TerritoryTop;
    }

    public override void OnReset()
    {
        ResetState();
        _sigmaSnapshot = null;
        _omega1Snapshot = null;
        _omega2Snapshot = null;
    }

    public override void OnDisable()
    {
        _overlayWindow?.Dispose();
        _overlayWindow = null;
    }

    public override void OnStartingCast(uint source, uint castId)
    {
        // Wait for Hello World Near after sigma or omega cast.
        if(castId == CastCodeSigma)
        {
            BeginNearTriggerSequence(PendingResolve.Sigma);
        }
        else if(castId == CastCodeOmega)
        {
            BeginNearTriggerSequence(PendingResolve.Omega1);
        }
    }

    public override void OnGainBuffEffect(uint sourceId, Status status)
    {
        // Run assignment when Hello Near applies after the watched cast.
        if(!IsPartyMember(sourceId)) return;

        if(status.StatusId == StatusHelloNear && _waitingNearAfterCast)
        {
            _ = new TickScheduler(() =>
            {
                if(!_waitingNearAfterCast) return;
                ResolveNearTrigger();
            }, ResolveDelayMs);
        }
    }

    public override void OnSettingsDraw()
    {
        ImGui.Text($"BasePlayer: {Controller.BasePlayer?.Name.ToString() ?? "null"}");
        C.PriorityData.Draw();
        ImGui.Text("Basic Settings");
        ImGui.Checkbox("Show Sigma Helper (LDPU)", ref C.ShowSigma);
        ImGui.Text("Overlay Settings");
        ImGui.Checkbox("Show Overlay", ref C.ShowOverlay);
        ImGui.Checkbox("Lock Overlay Position", ref C.OverlayLockPosition);
        ImGui.Checkbox("Make Overlay Transparent", ref C.OverlayTransparent);
        if(C.OverlayTransparent)
        {
            ImGui.SetNextItemWidth(120);
            ImGui.SliderFloat("Overlay Transparency", ref C.OverlayBgAlpha, 0.05f, 1.0f, "%.2f");
        }
    }
    #endregion

    #region Private Method
    // Sigma LDPU: none from hello debuffs, bind from dynamis, rest attack.
    private void ResolveSigma(List<IPlayerCharacter> party)
    {
        var none = OrderByConfigPriority(party.Where(x => HasStatus(x, StatusHelloNear) || HasStatus(x, StatusHelloFar))).Take(NoneCount).ToList();
        var bind = OrderByConfigPriority(party.Except(none)).Where(x => GetDynamisStack(x) > 0).Take(BindCount).ToList();
        var attack = OrderByConfigPriority(party.Except(none).Except(bind)).Take(AttackCount).ToList();
        OutputAssignment("sigma", none, bind, attack);
    }

    // Omega-1: none from first target; bind tiered; rest attack.
    private void ResolveOmega1(List<IPlayerCharacter> party)
    {
        var none = OrderByConfigPriority(party.Where(x => HasStatus(x, StatusFirstTarget))).Take(NoneCount).ToList();

        var bind = party
            .Except(none)
            .OrderBy(x => GetOmega1BindTier(x))
            .ThenBy(GetPriorityIndex)
            .Take(BindCount)
            .ToList();

        _lastOmega1Bind = bind;

        var attack = party
            .Except(none)
            .Except(bind)
            .OrderBy(x => GetOmega1AttackTier(x))
            .ThenBy(GetPriorityIndex)
            .Take(AttackCount)
            .ToList();

        OutputAssignment("omega-1", none, bind, attack);
    }

    // Omega-2: none from second target; bind two-stack excluding omega1 binds.
    private void ResolveOmega2(List<IPlayerCharacter> party)
    {
        var none = OrderByConfigPriority(party.Where(x => HasStatus(x, StatusSecondTarget))).Take(NoneCount).ToList();
        var bind = OrderByConfigPriority(party.Except(none).Except(_lastOmega1Bind).Where(x => GetDynamisStack(x) == 2)).Take(BindCount).ToList();
        var attack = OrderByConfigPriority(party.Except(none).Except(bind)).Take(AttackCount).ToList();
        OutputAssignment("omega-2", none, bind, attack);
    }

    // Arms delayed resolve until Hello Near buff.
    private void BeginNearTriggerSequence(PendingResolve resolve)
    {
        _pendingResolve = resolve;
        _waitingNearAfterCast = true;
    }

    // Runs the correct resolver for the pending phase after party check.
    private void ResolveNearTrigger()
    {
        var party = GetPartyMembers();
        if(party.Count < MinPartySize) return;

        switch(_pendingResolve)
        {
            case PendingResolve.Sigma:
                _waitingNearAfterCast = false;
                _pendingResolve = PendingResolve.None;
                ResolveSigma(party);
                break;
            case PendingResolve.Omega1:
                _waitingNearAfterCast = false;
                _pendingResolve = PendingResolve.None;
                ResolveOmega1(party);
                ResolveOmega2(party);
                break;
        }
    }

    // Clears pending near-trigger flags.
    private void ResetState()
    {
        _pendingResolve = PendingResolve.None;
        _waitingNearAfterCast = false;
    }

    // Stores overlay snapshot text for a phase name.
    private void OutputAssignment(string phaseName, List<IPlayerCharacter> none, List<IPlayerCharacter> bind, List<IPlayerCharacter> attack)
    {
        var noneText = string.Join(" ", none.Select(GetJobName));
        var bindText = string.Join(" ", bind.Select(GetJobName));
        var attackText = string.Join(" ", attack.Select(GetJobName));

        var snapshot = new AssignmentSnapshot
        {
            PhaseName = phaseName,
            NoneText = noneText,
            BindText = bindText,
            AttackText = attackText,
        };

        switch(phaseName)
        {
            case "sigma":
                _sigmaSnapshot = snapshot;
                break;
            case "omega-1":
                _omega1Snapshot = snapshot;
                break;
            case "omega-2":
                _omega2Snapshot = snapshot;
                break;
        }
    }

    // Renders the floating overlay window body.
    private void DrawOverlay()
    {
        ImGui.Text("P5 Marking Helper");
        ImGui.Separator();
        ImGui.Separator();
        if(C.ShowSigma)
        {
            DrawPhaseSection("Sigma", _sigmaSnapshot);
            ImGui.Separator();
        }
        DrawPhaseSection("Omega1", _omega1Snapshot);
        ImGui.Separator();
        DrawPhaseSection("Omega2", _omega2Snapshot);
    }

    // One phase block in the overlay.
    private void DrawPhaseSection(string phaseName, AssignmentSnapshot? snapshot)
    {
        ImGui.Text($"▼ {phaseName}");
        if(snapshot == null)
        {
            ImGui.Text("  none: ");
            ImGui.Text("  bind: ");
            ImGui.Text("  attack: ");
        }
        else
        {
            ImGui.Text($"  none: {snapshot.NoneText}");
            ImGui.Text($"  bind: {snapshot.BindText}");
            ImGui.Text($"  attack: {snapshot.AttackText}");
        }
    }

    // Stable party ordering from priority config then entity id.
    private List<IPlayerCharacter> OrderByConfigPriority(IEnumerable<IPlayerCharacter> players)
        => players.OrderBy(GetPriorityIndex).ThenBy(x => x.EntityId).ToList();

    // Index in script priority list (fallback max).
    private int GetPriorityIndex(IPlayerCharacter player)
    {
        var name = player.Name.ToString();
        var priority = C.PriorityData.GetPlayers(_ => true)?.ToList();
        if(priority == null) return int.MaxValue;

        for(var i = 0; i < priority.Count; i++)
        {
            if(priority[i].Name == name) return i;
        }

        return int.MaxValue;
    }

    // Sort key for omega-1 bind (second target + stacks).
    private int GetOmega1BindTier(IPlayerCharacter player)
    {
        var stack = GetDynamisStack(player);
        var secondTarget = HasStatus(player, StatusSecondTarget);

        if(secondTarget && stack == 2) return 0;
        if(stack == 2) return 1;
        if(stack == 1) return 2;
        return 99;
    }

    // Sort key for omega-1 attack line.
    private int GetOmega1AttackTier(IPlayerCharacter player)
    {
        var stack = GetDynamisStack(player);
        if(stack == 2) return 0;
        if(stack == 1) return 1;
        return 99;
    }

    // Dynamis stack count from status param.
    private int GetDynamisStack(IPlayerCharacter player)
    {
        if(player == null || player.StatusList == null) return 0;

        foreach(var status in player.StatusList)
        {
            if(status.StatusId == StatusDynamis)
                return status.Param;
        }

        return 0;
    }

    private static bool HasStatus(IPlayerCharacter player, uint statusId)
        => player.StatusList.Any(x => x.StatusId == statusId);

    private static string GetJobName(IPlayerCharacter player)
        => player.GetJob().ToString();

    private static List<IPlayerCharacter> GetPartyMembers()
        => FakeParty.Get().ToList();

    private static bool IsPartyMember(uint entityId)
        => FakeParty.Get().Any(x => x.EntityId == entityId);
    #endregion

    #region Config
    private class Config : IEzConfig
    {
        public PriorityData PriorityData = new();
        public bool ShowOverlay = true;
        public bool ShowSigma = true;
        public bool OverlayLockPosition;
        public bool OverlayTransparent;
        public float OverlayBgAlpha = 0.25f;
    }
    #endregion
}
