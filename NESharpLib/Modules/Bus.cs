using NESharpLib.Modules.CPU;

namespace NESharpLib.Modules;

public class Bus : Addressable
{
    private byte[] cpu_ram = new byte[0x0800];
    public event EventHandler? OnPPUSync;
    public event EventHandler? OnNMI;
    public event EventHandler<int>? OnAddStallCycles;
    public Bus()
    {
        /* CPU RAM */
        HandleRead(0x0, 0x1fff, (addr) =>
        {
            return cpu_ram[addr % 0x0800];
        });
        HandleWrite(0x0, 0x1fff, (addr, data) =>
        {
            cpu_ram[addr % 0x0800] = data;
        });
    }
    public void ProcessCPUCycle(PPU.PPU ppu)
    {
        OnPPUSync?.Invoke(ppu, new());
    }
    public void AddCPUStallCycles(PPU.PPU ppu, int cycles)
    {
        OnAddStallCycles?.Invoke(ppu, cycles);
    }
    public void TriggerNMI(PPU.PPU ppu)
    {
        OnNMI?.Invoke(ppu, new());
    }
}