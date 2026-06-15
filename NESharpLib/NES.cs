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
        Cartridge = new Cartridge(new byte[0xffff]);
        Bus = new Bus();
        PPU = new PPU(Bus, Cartridge);
        CPU = new CPU(Bus);
    }
}