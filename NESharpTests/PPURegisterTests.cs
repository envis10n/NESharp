namespace NESharpTests;

using NESharpLib.Modules.PPU.Registers;

[TestClass]
public class PPURegisterTests
{
    [TestMethod]
    public void PPUCtrlTests()
    {
        PPUCtrl ctrl = PPUCtrl.Empty;
        ctrl |= PPUCtrl.Nametable1 | PPUCtrl.Nametable2;
        Assert.AreEqual(0x2c00, ctrl.GetBaseNametable());
    }
    [TestMethod]
    public void PPUMaskTests()
    {
        PPUMask mask = PPUMask.Empty;
        mask |= PPUMask.EmphasizeRed | PPUMask.EmphasizeGreen | PPUMask.EmphasizeBlue;
        Assert.AreEqual(7, mask.GetEmphasis());
    }
}
