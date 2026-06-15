namespace NESharpLib.Modules.Cartridge.INES;

using NESharpLib.Modules.Cartridge;

[Flags]
public enum INESFlag6 : byte
{
    Empty = 0b0,
    NametableArrangement = 0b1,
    BatteryBackedRAM = 0b10,
    TrainerPresent = 0b100,
    AlternativeNametable = 0b1000,
    LowerMapperNybble = 0b11110000,
}

public static class Flag6Ext
{
    public static Mirroring GetMirroring(this INESFlag6 flags)
    {
        byte v = (byte)((((byte)flags & (byte)INESFlag6.AlternativeNametable) >> 2) | ((byte)flags & (byte)INESFlag6.NametableArrangement));
        if (v == 0) return Mirroring.Horizontal;
        else if (v == 1) return Mirroring.Vertical;
        else if (v == 2) return Mirroring.SingleScreen;
        else return Mirroring.FourScreen;
    }
    public static byte GetLowerMapperNybble(this INESFlag6 flags)
    {
        return (byte)(((byte)flags & (byte)INESFlag6.LowerMapperNybble) >> 4);
    }
    public static bool HasTrainer(this INESFlag6 flags)
    {
        return flags.HasFlag(INESFlag6.TrainerPresent);
    }
    public static bool HasBatteryRAM(this INESFlag6 flags)
    {
        return flags.HasFlag(INESFlag6.BatteryBackedRAM);
    }
}