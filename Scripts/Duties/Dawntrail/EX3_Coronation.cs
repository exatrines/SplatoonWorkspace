using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using Splatoon.Memory;
using Splatoon.SplatoonScripting;
using Splatoon.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static Splatoon.Splatoon;

namespace SplatoonScriptsOfficial.Duties.Dawntrail;

public unsafe class EX3_Coronation : SplatoonScript
{
    public override Metadata Metadata { get; } = new(2, "mirage");
    public override HashSet<uint>? ValidTerritories => [1243];

    private const uint CoronationBitDataId = 18043;
    private const double RainbowHueCycleSeconds = 4d;
    private const float ElementOffset = 1f;

    private BitDirection _bitDirection = BitDirection.None;
    private BitSide _bitSide = BitSide.None;

    public Config C => Controller.GetConfig<Config>();

    public enum BitDirection
    {
        None,
        North,
        East,
        South,
        West,
    }

    public enum BitSide
    {
        None,
        Left,
        Right,
    }

    public enum FieldDirection
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest,
    }

    public static Vector3 SpreadPositionFromDirection(FieldDirection dir)
        => dir switch
        {
            FieldDirection.North => new Vector3(100f, 0f, 80f + ElementOffset),
            FieldDirection.NorthEast => new Vector3(120f - ElementOffset, 0f, 80f + ElementOffset),
            FieldDirection.East => new Vector3(120f - ElementOffset, 0f, 100f),
            FieldDirection.SouthEast => new Vector3(120f - ElementOffset, 0f, 120f - ElementOffset),
            FieldDirection.South => new Vector3(100f, 0f, 120f - ElementOffset),
            FieldDirection.SouthWest => new Vector3(80f + ElementOffset, 0f, 120f - ElementOffset),
            FieldDirection.West => new Vector3(80f + ElementOffset, 0f, 100f),
            FieldDirection.NorthWest => new Vector3(80f + ElementOffset, 0f, 80f + ElementOffset),
        };

    public Vector3 GetSpreadPositionFromBit(BitDirection dir, BitSide side)
        => (dir, side) switch
        {
            (BitDirection.North, BitSide.Right) => SpreadPositionFromDirection(C.BasisIsNorth ? FieldDirection.North : FieldDirection.NorthWest),
            (BitDirection.North, BitSide.Left) => SpreadPositionFromDirection(C.BasisIsNorth ? FieldDirection.NorthEast : FieldDirection.North),
            (BitDirection.East, BitSide.Right) => SpreadPositionFromDirection(C.BasisIsNorth ? FieldDirection.East : FieldDirection.NorthEast),
            (BitDirection.East, BitSide.Left) => SpreadPositionFromDirection(C.BasisIsNorth ? FieldDirection.SouthEast : FieldDirection.East),
            (BitDirection.South, BitSide.Right) => SpreadPositionFromDirection(C.BasisIsNorth ? FieldDirection.South : FieldDirection.SouthEast),
            (BitDirection.South, BitSide.Left) => SpreadPositionFromDirection(C.BasisIsNorth ? FieldDirection.SouthWest : FieldDirection.South),
            (BitDirection.West, BitSide.Right) => SpreadPositionFromDirection(C.BasisIsNorth ? FieldDirection.West : FieldDirection.SouthWest),
            (BitDirection.West, BitSide.Left) => SpreadPositionFromDirection(C.BasisIsNorth ? FieldDirection.NorthWest : FieldDirection.West),
        };


    public override void OnSetup()
    {
        Controller.RegisterElementFromCode("navigation_spread",
            """{"Name":"navigation_spread","Enabled":false,"radius":1.0,"thicc":6.0,"fillIntensity":0.25,"tether":true}""",
            overwrite: true);
    }

    public override void OnSettingsDraw()
    {
        ImGui.Text("Spread Basis");
        ImGui.RadioButton("North (e.g. If North Bit tethered you then spread to N or NE.)", ref C.BasisIsNorth, true);
        ImGui.RadioButton("NorthWest (e.g. If North Bit tethered you then spread to NW or N.)", ref C.BasisIsNorth, false);

        ImGui.Separator();

        var bitCount = CountVisibleCoronationBits();
        ImGui.Text($"Bit Count: {bitCount} (required: 4)");
        ImGui.Text($"Bit Direction: {_bitDirection} (required: North, East, South, West)");
        ImGui.Text($"Bit Side: {_bitSide} (required: Right, Left)");
    }

    public override void OnUpdate()
    {
        if(!Controller.TryGetElementByName("navigation_spread", out var nav)) return;

        if(CountVisibleCoronationBits() != 4)
        {
            nav.Enabled = false;
            return;
        }

        _bitDirection = InferDirectionFromTethers(BasePlayer);
        _bitSide = InferSideFromPlayerVfx(BasePlayer);

        if(_bitDirection == BitDirection.None || _bitSide == BitSide.None)
        {
            nav.Enabled = false;
            return;
        }

        nav.Enabled = true;
        nav.color = GetRainbowColor(RainbowHueCycleSeconds).ToUint();
        nav.SetRefPosition(GetSpreadPositionFromBit(_bitDirection, _bitSide));
    }

    public override void OnReset()
    {
        _bitDirection = BitDirection.None;
        _bitSide = BitSide.None;
        if(Controller.TryGetElementByName("navigation_spread", out var nav)) nav.Enabled = false;
    }

    private static int CountVisibleCoronationBits()
        => Svc.Objects.OfType<IBattleNpc>().Count(x => x.DataId == CoronationBitDataId && x.IsCharacterVisible());

    private static IGameObject? ResolveTetherTarget(uint raw)
    {
        var byEntity = raw.GetObject();
        if(byEntity != null) return byEntity;
        return Svc.Objects.FirstOrDefault(x => x.GameObjectId == raw);
    }

    private BitDirection InferDirectionFromTethers(IPlayerCharacter me)
    {
        if(!AttachedInfo.TetherInfos.TryGetValue(me.Address, out var list) || list.Count == 0)
            return BitDirection.None;

        foreach(var t in list)
        {
            var targetObj = ResolveTetherTarget(t.Target);
            if(targetObj == null) continue;

            var z = FloatToInt(targetObj.Position.Z);
            var x = FloatToInt(targetObj.Position.X);
            if(z == 80) return BitDirection.North;
            if(x == 120) return BitDirection.East;
            if(z == 120) return BitDirection.South;
            if(x == 80) return BitDirection.West;
        }

        return BitDirection.None;
    }

    private static BitSide InferSideFromPlayerVfx(IPlayerCharacter me)
    {
        if(!me.TryGetVfx(out var fx) || fx == null) return BitSide.None;

        foreach(var kv in fx)
        {
            var path = kv.Key;
            if(path.Contains("chn_m0903track_c0w.avfx", StringComparison.OrdinalIgnoreCase))
                return BitSide.Right;
            if(path.Contains("chn_m0903track_c1w.avfx", StringComparison.OrdinalIgnoreCase))
                return BitSide.Left;
        }

        return BitSide.None;
    }

    private static int FloatToInt(float value) => (int)Math.Round(value);

    private Vector4 GetRainbowColor(double cycleSeconds)
    {
        if(cycleSeconds <= 0d) cycleSeconds = 1d;
        var normalizedTime = Environment.TickCount64 / 1000d / cycleSeconds;
        var hue = normalizedTime % 1d;
        return HsvToVector4(hue, 1d, 1d);
    }

    private static Vector4 HsvToVector4(double h, double s, double v)
    {
        double r = 0d, g = 0d, b = 0d;
        var i = (int)(h * 6d);
        var f = h * 6d - i;
        var p = v * (1d - s);
        var q = v * (1d - f * s);
        var t = v * (1d - (1d - f) * s);

        switch(i % 6)
        {
            case 0: r = v; g = t; b = p; break;
            case 1: r = q; g = v; b = p; break;
            case 2: r = p; g = v; b = t; break;
            case 3: r = p; g = q; b = v; break;
            case 4: r = t; g = p; b = v; break;
            case 5: r = v; g = p; b = q; break;
        }

        return new Vector4((float)r, (float)g, (float)b, 1f);
    }

    public sealed class Config : IEzConfig {
        public bool BasisIsNorth = true;
    }
}
