namespace NESharpTests;

using NESharpLib.Modules.CPU;
using NESharpLib.Modules;

[TestClass]
public class CPUTests
{
    [TestMethod]
    public void CPUStackTest()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.StackPush(0xfe);
        Assert.AreEqual(0xfe, cpu.StackPop());
        cpu.StackPushShort(0x1fc0);
        Assert.AreEqual(0x1fc0, cpu.StackPopShort());
    }
}
