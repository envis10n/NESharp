namespace NESharpLib;

using NESharpLib.Modules.PPU;
using NESharpLib.Modules.Cartridge;
using NESharpLib.Modules.CPU;
using NESharpLib.Modules.Render;
using NESharpLib.Modules;

public class NES
{
    public PPU PPU;
    public Bus Bus;
    public CPU CPU;
    public Cartridge Cartridge;
    public NES(string filepath)
    {
        Cartridge = new Cartridge(File.ReadAllBytes(filepath));
        Bus = new Bus();
        Bus.HandleRead(0x4020, 0xffff, (addr) =>
        {
            ushort _addr = addr.WrappingSub(0x8000);
            if (Cartridge.PRG.Length == 0x4000 && _addr >= 0x4000)
            {
                _addr = (ushort)(_addr % 0x4000);
            }
            return Cartridge.PRG[_addr];
        });
        PPU = new PPU(Bus, Cartridge);
        CPU = new CPU(Bus);
        CPU.Reset();
    }
}