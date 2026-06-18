using NESharpLib.Modules.Cartridge.INES;

namespace NESharpLib.Modules.Cartridge;

public enum Mirroring : byte
{
    Horizontal,
    Vertical,
    SingleScreen,
    FourScreen,
}

public enum ROMFormat : byte
{
    iNES,
    NES2
}

public class Cartridge
{
    private static readonly byte[] NES_TAG = [0x4e, 0x45, 0x53, 0x1a];
    private static ushort PRG_ROM_BLOCK_SIZE = 0x4000;
    private static ushort CHR_ROM_BLOCK_SIZE = 0x2000;
    public readonly Mirroring Mirroring;
    public readonly ROMFormat Format;
    public readonly byte Mapper;
    public readonly byte[] Trainer;
    public readonly byte[] PRG;
    public readonly byte[] CHR;
    public Cartridge(byte[] data)
    {
        byte[] header = data[0..16];
        byte[] rom = data[16..data.Length];

        byte[] nes_tag = header[0..4];
        if (nes_tag.SequenceEqual(NES_TAG)) Format = ROMFormat.iNES;
        else throw new Exception("Invalid ROM format.");

        ushort prg_rom_size = (ushort)(header[4] * PRG_ROM_BLOCK_SIZE);
        ushort chr_rom_size = (ushort)(header[5] * CHR_ROM_BLOCK_SIZE);

        INESFlag6 flag6 = (INESFlag6)header[6];
        INESFlag7 flag7 = (INESFlag7)header[7];

        if (flag7.IsNES2()) Format = ROMFormat.NES2;

        ushort prg_rom_start = 0;

        if (flag6.HasTrainer())
        {
            Trainer = rom[0..512];
            prg_rom_start += 512;
        }
        else Trainer = [];

        ushort prg_rom_end = (ushort)(prg_rom_start + prg_rom_size);

        Mirroring = flag6.GetMirroring();
        Mapper = (byte)(flag7.GetUpperMapperNybble() | flag6.GetLowerMapperNybble());

        PRG = rom[prg_rom_start..prg_rom_end];
        CHR = rom[prg_rom_end..(prg_rom_end + chr_rom_size)];
    }
}