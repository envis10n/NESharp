namespace NESharpLib.Modules.PPU;
using NESharpLib.Modules.Cartridge;

public partial class PPU
{
    private byte[] palette_control = new byte[0x20];
    private byte PPUReadHandler(ushort address)
    {
        if (address.InRange(0x0, 0x1fff))
        {
            return cartridge.CHR[address];
        }
        if (address.InRange(0x2000, 0x3eff))
        {
            ushort addr = MirrorVRAMAddr(address);
            return vram[addr];
        }
        if (address.InRange(0x3f00, 0x3fff))
        {
            ushort addr = address.WrappingSub(0x3f00);
            return palette_control[addr];
        }
        return 0;
    }
    private byte ReadStatus()
    {
        W = false;
        return (byte)status;
    }
    private byte ReadPPUData()
    {
        byte res = ppudata_buffer;
        ppudata_buffer = ppudata;
        ppudata = ReadByte(ppuaddr);
        IncrementPPUAddr();
        return res;
    }
}