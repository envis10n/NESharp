using NESharpLib.Modules.PPU.Registers;
using NESharpLib.Modules.Render;
using SFML.Graphics;
using SFML.System;

namespace NESharpLib.Modules.PPU;

public class FrameReadyArgs : EventArgs
{
    public Image Frame { get; init; }
    public FrameReadyArgs(Image frame)
    {
        Frame = frame;
    }
}

public partial class PPU
{
    public event EventHandler<Image>? OnFrameReady;
    public Image frame = new Image(new Vector2u(256, 240));
    public (byte, byte) NextTile = (0, 0);
    private byte tileShiftRegisterLow1 = 0;
    private byte tileShiftRegisterHigh1 = 0;
    private byte tileShiftRegisterLow2 = 0;
    private byte tileShiftRegisterHigh2 = 0;
    public byte NextAttributes = 0;
    private byte latch1 = 0;
    private byte latch2 = 0;
    private byte attributeShiftRegister1 = 0;
    private byte attributeShiftRegister2 = 0;
    public byte[] SpriteOutput = new byte[32];
    private Palette palette = new Palette(File.ReadAllBytes("Palettes/2C02G.pal"));
    private int shiftCounter = 0;
    private int scanline = -1;
    private int cycle = 0;
    private byte bg_pixel = 0;
    private (byte, byte) GetCoarseScroll()
    {
        byte x = (byte)(V & 0b11111);
        byte y = (byte)((V & 0b1111100000) >> 5);
        return (x, y);
    }
    private byte GetNametableSelect()
    {
        return (byte)((V & 0b110000000000) >> 10);
    }
    private byte GetFineYScroll()
    {
        return (byte)((V & 0b111000000000000) >> 12);
    }
    private void RefillPatternData()
    {
        ushort addr = GetNextTileAddr();
        byte tileID = ReadByte(addr);
        byte plane0 = ReadByte(GetPatternAddr(tileID, 0));
        byte plane1 = ReadByte(GetPatternAddr(tileID, 1));
        NextTile = (plane0, plane1);
    }
    private void RefillAttributeData()
    {
        ushort addr = GetNextAttributeAddr();
        NextAttributes = ReadByte(addr);
    }
    private void ShiftRegisters()
    {
        tileShiftRegisterLow2.Shift(ref tileShiftRegisterLow1);
        tileShiftRegisterHigh2.Shift(ref tileShiftRegisterHigh1);

        attributeShiftRegister1.Shift(ref latch1);
        attributeShiftRegister2.Shift(ref latch2);
    }
    private byte CombineAttributes()
    {
        byte attribute1 = (byte)((attributeShiftRegister1 >> X) & 0b1);
        byte attribute2 = (byte)((attributeShiftRegister2 >> X) & 0b1);
        return (byte)((attribute2 << 1) | attribute1);
    }
    private byte CombinePattern()
    {
        byte tileLow = (byte)((tileShiftRegisterLow2 >> X) & 0b1);
        byte tileHigh = (byte)((tileShiftRegisterHigh2 >> X) & 0b1);
        return (byte)((tileHigh << 1) | tileLow);
    }
    private bool IsPixelTransparent(byte a)
    {
        return a == palette_control[0];
    }
    private byte SelectPixelBits()
    {
        byte pattern = CombinePattern();
        byte attribute = CombineAttributes();

        byte[] palette = palette_control[(attribute * 4)..((attribute * 4) + 4)];
        return palette[pattern];
    }
    private void DrawPixel(uint x, uint y, Color color)
    {
        frame.SetPixel(new Vector2u(x, y), color);
    }
    private void ProcessBackground()
    {
        byte palette_color = SelectPixelBits();
        bg_pixel = palette_color;
        Color pixel = palette.GetPaletteColor(mask.GetEmphasis(), palette_color);
        if (scanline != -1)
        {
            uint fx = (uint)(cycle - 1);
            uint fy = (uint)scanline;
            DrawPixel(fx, fy, pixel);
        }
    }
    private void ProcessSprite()
    {
        uint x = (uint)(cycle - 1);
        uint y = (uint)scanline;
        foreach (SpriteData sprite in sprite_output)
        {
            if (!sprite.Valid) continue;
            if (
                sprite.Y == 0 ||
                sprite.Y > 239 ||
                x >= sprite.X + 8 ||
                x < sprite.X ||
                x < 8 && !mask.GetSpriteLeft()
            ) continue;
            uint tile_idx = sprite.Tile;
            byte palette_id = sprite.Palette;
            bool front = sprite.Priority == 0;
            bool flipX = sprite.FlipHorizontal;
            bool flipY = sprite.FlipVertical;

            int px = (int)(x - sprite.X);
            int line = scanline - sprite.Y;

            ushort tableBase = ctrl.GetSpriteTableAddress();

            // TODO: handle tall sprites

            int logicalX = flipX ? 7 - px : px;
            int logicalLine = flipY ? 7 - line : line;

            ushort address = tableBase.WrappingAdd((uint)(tile_idx + logicalLine));
            uint color = (uint)(
                    (
                        (
                            (
                                // fetch upper bit from 2nd bit plane
                                ReadByte(address.WrappingAdd(8)) & (0x80 >> logicalX)
                            ) >> (7 - logicalX)
                        ) << 1 // this is the upper bit of the color number
                    ) |
                    (
                        (
                            ReadByte(address) & (0x80 >> logicalX)
                        ) >> (7 - logicalX)
                    ));
            if (color > 0)
            {
                if (!(
                    x < 8 && !mask.GetSpriteLeft() ||
                    bg_pixel == 0 ||
                    status.GetSpriteZeroHit() ||
                    x == 255
                )) status |= PPUStatus.SpriteZeroHit;

                if (mask.GetSpriteRenderEnabled() && (front || bg_pixel == 0))
                {
                    if (scanline != -1)
                    {
                        Color pixel = palette.GetPaletteColor(mask.GetEmphasis(), (byte)color);
                        uint fx = (uint)(cycle - 1);
                        uint fy = (uint)scanline;
                        DrawPixel(fx, fy, pixel);
                    }
                }
            }
        }
    }
    public void LoadShiftRegisters()
    {
        tileShiftRegisterLow1 = NextTile.Item1;
        tileShiftRegisterHigh1 = NextTile.Item2;
        (byte, byte) coarseScroll = GetCoarseScroll();
        byte attributeSelect = (byte)((coarseScroll.Item2 & 0b1) << 1 | (coarseScroll.Item1 & 0b1));
        byte attributeData = (byte)((NextAttributes & (0b11 << (attributeSelect * 2))) >> (attributeSelect * 2));
        latch1 = (byte)(attributeData & 0b1 << 7);
        latch2 = (byte)((attributeData & 0b10) << 6);
    }
    public void InvokeFrameReady()
    {
        OnFrameReady?.Invoke(this, frame);
    }
    private (byte, byte) GetSpritePattern(SpriteData sprite, byte y)
    {
        ushort patternAddr = ctrl.GetSpriteTableAddress();
        patternAddr |= (ushort)(sprite.Tile << 4);
        patternAddr |= (ushort)(y & 0b111);
        byte low = cartridge.CHR[patternAddr];
        patternAddr ^= 0b1000;
        byte high = cartridge.CHR[patternAddr];
        return (low, high);
    }
    public void RenderFrame()
    {
        for (int s = -1; s < 261; s++)
        {
            scanline = s;
            for (int c = 0; c < 341; c++)
            {
                cycle = c;
                ProcessCycle();
                if ((c % 3) == 0)
                {
                    bus.ProcessCPUCycle(this);
                }
            }
        }
    }
    private void ProcessCycle()
    {
        if (scanline.InRange(-1, 239))
        {
            if (scanline == -1 && cycle == 1)
            {
                frame = new Image(new Vector2u(256, 240));
                status &= ~PPUStatus.VBlankFlag;
                status &= ~PPUStatus.SpriteZeroHit;
                status &= ~PPUStatus.SpriteOverflow;
            }
            // Visible
            if (cycle == 0) { }
            else if (cycle.InRange(1, 256))
            {
                ProcessBackground();
                ProcessSprite();
                ShiftRegisters();
                if (shiftCounter == 8)
                {
                    if (cycle == 256)
                        IncrementY();
                    else
                        IncrementX();
                    RefillPatternData();
                    RefillAttributeData();
                    LoadShiftRegisters();
                    shiftCounter = 0;
                }
                else
                    shiftCounter++;
                SpriteEvaluation(scanline + 1, cycle);
            }
            else if (cycle.InRange(257, 320))
            {
                if (cycle == 257)
                    ReloadScrollX();
                SpriteFetch();
            }
            else if (cycle.InRange(321, 336))
            {
                // Fetch tiles for next frame
                if (cycle == 321)
                {
                    RefillPatternData();
                    RefillAttributeData();
                    LoadShiftRegisters();
                }
                else if (cycle == 336)
                {
                    RefillPatternData();
                    RefillAttributeData();
                }
            }
            else if (cycle.InRange(337, 340))
            {
                // Garbage fetches??
            }
        }
        else if (scanline == 240)
        {
            // Post-render
        }
        else if (scanline.InRange(241, 260))
        {
            // VBlank
            if (cycle == 1)
            {
                status |= PPUStatus.VBlankFlag;
                InvokeFrameReady();
            }
        }
    }
}