namespace NESharpLib.Modules.PPU.Registers;

using System;

[Flags]
public enum PPUStatus
{
    Empty          = 0b0,
    OpenBusID      = 0b11111,
    SpriteOverflow = 0b100000,
    SpriteZeroHit  = 0b1000000,
    VBlankFlag     = 0b10000000,
}

public static class PPUStatusExt
{
    public static bool GetSpriteOverflow(this PPUStatus status)
    {
        return status.HasFlag(PPUStatus.SpriteOverflow);
    }
    public static bool GetSpriteZeroHit(this PPUStatus status)
    {
        return status.HasFlag(PPUStatus.SpriteZeroHit);
    }
    public static bool VBlankFlag(this PPUStatus status)
    {
        return status.HasFlag(PPUStatus.VBlankFlag);
    }
}