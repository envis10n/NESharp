namespace NESharpLib.Modules
{
    public class Bus : Addressable
    {
        private byte[] cpu_ram = new byte[0x0800];
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
    }
}
