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
    public event EventHandler<FrameReadyArgs>? FrameReady;
    private Image frame = new Image(new Vector2u(256, 240));
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
    public void HBlank()
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
        FrameReady?.Invoke(this, new FrameReadyArgs(new Image(frame)));
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
    private void ProcessScanline(int scanline)
    {
        
    }
    private void ProcessCycle(int scanline, int cycle)
    {
        if (scanline == -1)
        {
            // Pre-render
            
        }
        else if (scanline.InRange(0, 239))
        {
            // Visible
            if (cycle == 0) return;
            if (cycle.InRange(1, 256))
            {
                ShiftRegisters();
                if (shiftCounter == 8)
                {
                    HBlank();
                    shiftCounter = 0;
                } else
                {
                    shiftCounter++;
                }
                SpriteEvaluation(scanline, cycle);
            }
            else if (cycle.InRange(257, 320))
            {
                SpriteFetch(scanline, cycle);
            }
            else if (cycle.InRange(321, 336))
            {
                // Fetch tile for next frame

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
            if (cycle == 1) status |= PPUStatus.VBlankFlag;
        }
    }
}