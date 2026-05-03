using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
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
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Splatoon.SplatoonScripting;
using Splatoon.SplatoonScripting.Priority;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SplatoonScriptsOfficial.Duties.Endwalker.The_Omega_Protocol;

public class P5_Marking_Helper : SplatoonScript
{
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

    private PendingResolve _pendingResolve;
    private bool _waitingNearAfterCast;
    private List<IPlayerCharacter> _lastOmega1Bind = new();
    private AssignmentSnapshot? _sigmaSnapshot;
    private AssignmentSnapshot? _omega1Snapshot;
    private AssignmentSnapshot? _omega2Snapshot;
    private OverlayWindow? _overlayWindow;

    public override HashSet<uint>? ValidTerritories => [TerritoryTop];
    public override Metadata? Metadata => new(1, "mirage");

    private Config C => Controller.GetConfig<Config>();
    private string OverlayWindowName => "P5 Marking Helper###P5_Marking_Helper";

    public override void OnSetup()
    {
        _overlayWindow ??= new OverlayWindow(this);
        _overlayWindow.IsOpen = C.ShowOverlay && Svc.ClientState.TerritoryType == TerritoryTop;
    }

    public override void OnDisable()
    {
        _overlayWindow?.Dispose();
        _overlayWindow = null;
    }

    public override void OnUpdate()
    {
        _overlayWindow ??= new OverlayWindow(this);
        _overlayWindow.IsOpen = C.ShowOverlay && Svc.ClientState.TerritoryType == TerritoryTop;
    }

    public override void OnStartingCast(uint source, uint castId)
    {
        // シグマまたはオメガキャスト開始時に、ハローワールド・ニア付与を待つ
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
        // ハローワールド・ニア付与時にマーキング計算を実行
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

    public override void OnReset()
    {
        ResetState();
        _sigmaSnapshot = null;
        _omega1Snapshot = null;
        _omega2Snapshot = null;
    }

    public override void OnSettingsDraw()
    {
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

    private void ResolveSigma(List<IPlayerCharacter> party)
    {
        var none = OrderByConfigPriority(party.Where(x => HasStatus(x, StatusHelloNear) || HasStatus(x, StatusHelloFar))).Take(NoneCount).ToList();
        var bind = OrderByConfigPriority(party.Except(none)).Where(x => GetDynamisStack(x) > 0).Take(BindCount).ToList();
        var attack = OrderByConfigPriority(party.Except(none).Except(bind)).Take(AttackCount).ToList();
        OutputAssignment("sigma", none, bind, attack);
    }

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

    private void ResolveOmega2(List<IPlayerCharacter> party)
    {
        var none = OrderByConfigPriority(party.Where(x => HasStatus(x, StatusSecondTarget))).Take(NoneCount).ToList();
        var bind = OrderByConfigPriority(party.Except(none).Except(_lastOmega1Bind).Where(x => GetDynamisStack(x) == 2)).Take(BindCount).ToList();
        var attack = OrderByConfigPriority(party.Except(none).Except(bind)).Take(AttackCount).ToList();
        OutputAssignment("omega-2", none, bind, attack);
    }

    private void BeginNearTriggerSequence(PendingResolve resolve)
    {
        _pendingResolve = resolve;
        _waitingNearAfterCast = true;
    }

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

    private void ResetState()
    {
        _pendingResolve = PendingResolve.None;
        _waitingNearAfterCast = false;
    }

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

        // フェーズに応じてスナップショットを保存
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

    private List<IPlayerCharacter> OrderByConfigPriority(IEnumerable<IPlayerCharacter> players)
        => players.OrderBy(GetPriorityIndex).ThenBy(x => x.EntityId).ToList();

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

    private int GetOmega1BindTier(IPlayerCharacter player)
    {
        var stack = GetDynamisStack(player);
        var secondTarget = HasStatus(player, StatusSecondTarget);

        if(secondTarget && stack == 2) return 0;
        if(stack == 2) return 1;
        if(stack == 1) return 2;
        return 99;
    }

    private int GetOmega1AttackTier(IPlayerCharacter player)
    {
        var stack = GetDynamisStack(player);
        if(stack == 2) return 0;
        if(stack == 1) return 1;
        return 99;
    }

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

    private class Config : IEzConfig
    {
        public PriorityData PriorityData = new();
        public bool ShowOverlay = true;
        public bool ShowSigma = true;
        public bool OverlayLockPosition;
        public bool OverlayTransparent;
        public float OverlayBgAlpha = 0.25f;
    }
}