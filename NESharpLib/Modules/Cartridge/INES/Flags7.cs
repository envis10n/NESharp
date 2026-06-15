namespace NESharpLib.Modules.Cartridge.INES;

using NESharpLib.Modules.Cartridge;

[Flags]
public enum INESFlag7 : byte
{
    Empty = 0b0,
    VSUnisystem = 0b1,
    Playchoice10 = 0b10,
    NES2 = 0b1100,
    UpperMapperNybble = 0b11110000,
}

public static class Flag7Ext
{
    public static bool IsNES2(this INESFlag7 flags)
    {
        return ((byte)flags & (byte)INESFlag7.NES2) >> 2 == 2;
    }
    public static byte GetUpperMapperNybble(this INESFlag7 flags)
    {
        return (byte)((byte)flags & (byte)INESFlag7.UpperMapperNybble);
    }
}