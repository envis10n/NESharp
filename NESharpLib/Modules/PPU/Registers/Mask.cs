namespace NESharpLib.Modules.PPU.Registers;

using System;

[Flags]
public enum PPUMask : byte
{
    Empty          = 0b0,
    Greyscale      = 0b1,
    BGLeftShow     = 0b10,
    SpriteLeftShow = 0b100,
    BGEnabled      = 0b1000,
    SpriteEnabled  = 0b10000,
    EmphasizeRed   = 0b100000,
    EmphasizeGreen = 0b1000000,
    EmphasizeBlue  = 0b10000000,
}

public static class PPUMaskExt
{
    public static byte GetGreyscale(this PPUMask mask)
    {
        return (byte)(mask & PPUMask.Greyscale);
    }
    public static bool GetBGLeft(this PPUMask mask)
    {
        return mask.HasFlag(PPUMask.BGLeftShow);
    }
    public static bool GetSpriteLeft(this PPUMask mask)
    {
        return mask.HasFlag(PPUMask.SpriteLeftShow);
    }
    public static bool GetBGRenderEnabled(this PPUMask mask)
    {
        return mask.HasFlag(PPUMask.BGEnabled);
    }
    public static bool GetSpriteRenderEnabled(this PPUMask mask)
    {
        return mask.HasFlag(PPUMask.SpriteEnabled);
    }
    public static byte GetEmphasis(this PPUMask mask)
    {
        return (byte)((byte)(mask & (PPUMask.EmphasizeRed | PPUMask.EmphasizeGreen | PPUMask.EmphasizeBlue)) >> 5);
    }
}