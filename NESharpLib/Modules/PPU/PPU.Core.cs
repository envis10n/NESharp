namespace NESharpLib.Modules.PPU;

using Registers;
using NESharpLib.Modules.Cartridge;
using NESharpLib.Modules.Render;
using NESharpLib.Modules.Cartridge.INES;

public partial class PPU : Addressable
{
    private Bus bus;
    private Cartridge cartridge;
    private byte[] vram = new byte[0x800];
    private PPUCtrl ctrl = PPUCtrl.Empty;
    private PPUMask mask = PPUMask.Empty;
    private PPUStatus status = PPUStatus.Empty;
    private ushort ppuaddr = 0x0000;
    private byte ppudata = 0x00;
    private byte scroll_x = 0x00;
    private byte scroll_y = 0x00;
    private byte ppudata_buffer = 0x00;
    private bool W = false;
    private ushort V = 0x0000;
    private ushort T = 0x0000;
    private byte X = 0x00;
    private bool OddFrame = false;
    public PPU(Bus _bus, Cartridge _cartridge)
    {
        bus = _bus;
        cartridge = _cartridge;
        HandleRead(0x0000, 0x3fff, PPUReadHandler);
        HandleWrite(0x0000, 0x3fff, PPUWriteHandler);
        bus.HandleRead(0x2000, 0x3fff, (address) =>
        {
            ushort addr = (ushort)((address % 8) + 0x2000);
            switch (addr)
            {
                case 0x2002:
                    {
                        return ReadStatus();
                    }
                case 0x2004:
                    {
                        return ReadOAMDATA();
                    }
                case 0x2007:
                    {
                        return ReadPPUData();
                    }
            }
            return 0;
        });
        bus.HandleWrite(0x2000, 0x3fff, (address, data) =>
        {
            ushort addr = (ushort)((address % 8) + 0x2000);
            switch (addr)
            {
                case 0x2000:
                    {
                        WriteCtrl(data);
                        break;
                    }
                case 0x2001:
                    {
                        WriteMask(data);
                        break;
                    }
                case 0x2003:
                    {
                        WriteOAMAddr(data);
                        break;
                    }
                case 0x2004:
                    {
                        WriteOAMDATA(data);
                        break;
                    }
                case 0x2005:
                    {
                        WriteScroll(data);
                        break;
                    }
            }
        });
        bus.HandleWrite(0x4014, 0x4014, (addr, data) => BeginOAMDMA(data));
    }
    private ushort MirrorVRAMAddr(ushort addr)
    {
        ushort mirrored_vram = (ushort)(addr & 0b10111111111111);
        ushort vram_index = (ushort)(mirrored_vram - 0x2000);
        ushort name_table = (ushort)(vram_index / 0x400);
        if (cartridge.Mirroring == Mirroring.Vertical && (name_table == 2 || name_table == 3))
        {
            return (ushort)(vram_index - 0x800);
        }
        else if (cartridge.Mirroring == Mirroring.Horizontal)
        {
            if (name_table == 2 || name_table == 1) return (ushort)(vram_index - 0x400);
            else if (name_table == 3) return (ushort)(vram_index - 0x800);
        }
        return vram_index;
    }
    private void IncrementPPUAddr()
    {
        ushort inc = ctrl.GetVRAMIncrement();
        ppuaddr = ppuaddr.WrappingAdd(inc);
    }
}
