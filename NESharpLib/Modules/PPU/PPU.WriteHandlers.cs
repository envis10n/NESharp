using NESharpLib.Modules.PPU.Registers;

namespace NESharpLib.Modules.PPU;

public partial class PPU
{
    private void PPUWriteHandler(ushort address, byte data)
    {
        if (address.InRange(0x2000, 0x3eff))
        {
            ushort addr = MirrorVRAMAddr(address);
            vram[addr] = data;
        }
        if (address.InRange(0x3f00, 0x3fff))
        {
            ushort addr = address.WrappingSub(0x3f00);
            palette_control[addr] = data;
        }
    }
    private void WriteCtrl(byte data)
    {
        ctrl = (PPUCtrl)data;
        T |= (ushort)((data & 0b11) << 10); // Add bits to temp
    }
    private void WriteMask(byte data)
    {
        mask = (PPUMask)data;
    }
    private void WriteOAMAddr(byte data)
    {
        oamaddr = data;
    }
    private void WriteScroll(byte data)
    {
        if (!W)
        {
            // First write
            scroll_x = data;
            T |= (ushort)((data & 0b11111000) >> 3); // Add bits to temp
            X = (byte)(data & 0b111); // Set X register
            W = true;
        } else
        {
            // Second write
            scroll_y = data;
            T |= (ushort)((data & 0b11111000) << 2); // Add bits to temp
            T |= (ushort)((data & 0b111) << 12);     // and again
            W = false;
        }
    }
    private void WritePPUAddr(byte data)
    {
        if (!W)
        {
            // First write
            ppuaddr = (ushort)(data << 8);
            T |= (ushort)((data & 0b111111) << 8); // Add bits to temp
            T = (ushort)(T & ~0x4000); // Unset bit 14
            W = true;
        } else
        {
            ppuaddr |= data;
            T |= data;
            W = false;
            V = T;
        }
    }
    private void WritePPUData(byte data)
    {
        ppudata = data;
        WriteByte(ppuaddr, data);
        IncrementPPUAddr();
    }
}