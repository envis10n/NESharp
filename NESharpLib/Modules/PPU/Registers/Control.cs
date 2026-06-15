namespace NESharpLib.Modules.PPU.Registers;

using System;

[Flags]
public enum PPUCtrl : byte
{
    Empty = 0b0,
    Nametable1 = 0b1,
    Nametable2 = 0b10,
    VRAMIncrement = 0b100,
    SpriteTable = 0b1000,
    BGTable = 0b10000,
    SpriteSize = 0b100000,
    PPUSelect = 0b1000000,
    VBLEnable = 0b10000000,
}

public static class PPUCtrlExt
{
    public static ushort GetBaseNametable(this PPUCtrl ctrl)
    {
        byte t = (byte)(ctrl & (PPUCtrl.Nametable1 | PPUCtrl.Nametable2));
        return ((ushort)0x2000).WrappingAdd(0x400 * t);
    }
    public static ushort GetVRAMIncrement(this PPUCtrl ctrl)
    {
        return (ushort)(ctrl.HasFlag(PPUCtrl.VRAMIncrement) ? 32 : 1);
    }
    public static ushort GetSpriteTableAddress(this PPUCtrl ctrl)
    {
        return (ushort)(ctrl.HasFlag(PPUCtrl.SpriteTable) ? 0x1000 : 0x0000);
    }
    public static ushort GetBGTableAddress(this PPUCtrl ctrl)
    {
        return (ushort)(ctrl.HasFlag(PPUCtrl.BGTable) ? 0x1000 : 0x0000);
    }
    public static byte GetSpriteSize(this PPUCtrl ctrl)
    {
        return (byte)(ctrl.HasFlag(PPUCtrl.SpriteSize) ? 1 : 0);
    }
    public static bool GetVBLEnabled(this PPUCtrl ctrl)
    {
        return ctrl.HasFlag(PPUCtrl.VBLEnable);
    }
}