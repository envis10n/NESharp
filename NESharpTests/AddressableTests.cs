using NESharpLib;

namespace NESharpTests;

[TestClass]
public sealed class AddressableTests
{
    public class RAM : Addressable
    {
        public byte[] bank1 = new byte[0x0200];
        public byte[] bank2 = new byte[0x0200];
        public RAM()
        {
            HandleRead(0x0000, 0x01ff, (addr) =>
            {
                return bank1[addr];
            });
            HandleWrite(0x0000, 0x01ff, (addr, data) =>
            {
                bank1[addr] = data;
            });

            HandleRead(0x0200, 0x03ff, (addr) =>
            {
                return bank2[addr % 0x0200];
            });
            HandleWrite(0x0200, 0x03ff, (addr, data) =>
            {
                bank2[addr % 0x0200] = data;
            });
        }
    }
    [TestMethod]
    public void TestDerivedAddr()
    {
        RAM ram = new RAM();

        ram.WriteByte(0x0000, 0xff);
        Assert.AreEqual(0xff, ram.bank1[0]);
        Assert.AreEqual(0x0, ram.bank2[0]);
        ram.WriteByte(0x0200, 0xff);
        Assert.AreEqual(0xff, ram.bank2[0]);
        ram.WriteShort(0x01ff, 0xdead);
        Assert.AreEqual(0xdead, ram.ReadShort(0x01ff));
        ram.WriteSByte(0x1, -16);
        Assert.AreEqual(-16, ram.ReadSByte(0x1));
    }
    [TestMethod]
    public void TestBaseAddressable()
    {
        Addressable a = new Addressable();
        ushort b = 0;
        byte c = 0;
        a.HandleRead(0x0000, 0x1000, (addr) =>
        {
            return 0x10;
        });
        a.HandleRead(0x1001, 0x2000, (addr) =>
        {
            return 0x20;
        });
        a.HandleWrite(0x0000, 0x2000, (addr, data) =>
        {
            b = addr;
            c = data;
        });
        Assert.AreEqual(0x10, a.ReadByte(0x00ff));
        Assert.AreEqual(0x20, a.ReadByte(0x10ff));
        a.WriteByte(0xff, 0xff);
        Assert.AreEqual(0xff, b);
        Assert.AreEqual(0xff, c);
    }
}
