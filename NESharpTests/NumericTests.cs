using NESharpLib;

namespace NESharpTests;

[TestClass]
public sealed class NumericTests
{
    [TestMethod]
    public void TestNumericExtensions8()
    {
        byte b = 0b10000001;
        byte c = b.ShiftOut();
        byte d = 0b01001100;
        byte e = 0b00000001;
        e.Shift(ref d);
        Assert.AreEqual(2, e);
        Assert.AreEqual(0b10011000, d);
        Assert.AreEqual(1, c);
        Assert.AreEqual(2, b);
        byte a = 0xff;
        Assert.AreEqual(0x0, a.WrappingInc());
        Assert.AreEqual("FF", a.ToHexString());
        Assert.AreEqual("11111111", a.ToBinaryString());
        byte f = 0b10100100;
        Assert.AreEqual(1, f.GetBit(7));
        Assert.AreEqual(0, f.GetBit(0));
    }
    [TestMethod]
    public void TestNumericExtensions16()
    {
        ushort a = 0x01ff;
        ushort b = 0xffff;
        byte[] bytes = [0xff, 0x01];
        Assert.AreEqual(0, b.WrappingInc());
        Assert.AreEqual(a, bytes.FromLEBytes());
        Assert.IsTrue(bytes.SequenceEqual(a.ToLEBytes()));
        Assert.AreEqual("01FF", a.ToHexString());
        Assert.AreEqual("1111111111111111", b.ToBinaryString());
    }
}
