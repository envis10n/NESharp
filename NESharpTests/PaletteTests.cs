namespace NESharpTests;

using NESharpLib.Modules.Render;

[TestClass]
public class PaletteTests
{
    [TestMethod]
    public void PaletteTestLoad()
    {
        Palette palette = new Palette(File.ReadAllBytes("Palettes/2C02G.pal"));
        Console.WriteLine("{0}", palette.GetPaletteColor(0, 0x2c));
    }
}
