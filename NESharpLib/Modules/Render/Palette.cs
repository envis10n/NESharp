namespace NESharpLib.Modules.Render;

using SFML.Graphics;

public record struct Palette
{
    private Color[,] palette = new Color[8,0x40];
    public Palette(byte[] bytes)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int i = 0; i < 0x40; i++)
            {
                int idx = (i * 3) + (x * 64);
                byte[] tri = bytes[idx..(idx + 3)];
                palette[x,i] = new Color(tri[0], tri[1], tri[2]);
            }
        }
    }
    public Color GetPaletteColor(byte emphasis, byte select)
    {
        return palette[emphasis, select];
    }
}