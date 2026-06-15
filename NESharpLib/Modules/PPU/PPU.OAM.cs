using NESharpLib.Modules.PPU.Registers;

namespace NESharpLib.Modules.PPU;

public record struct SpriteData
{
    public byte[] Data { get; init; }
    public byte Y { get => Data[0]; set => Data[0] = value; }
    public byte Tile { get => Data[1]; set => Data[1] = value; }
    public byte Attributes { get => Data[2]; set => Data[2] = value; }
    public byte X { get => Data[3]; set => Data[3] = value; }
    public SpriteData(byte[] data)
    {
        Data = data;
    }
}

public enum SpriteEvalState : byte
{
    CLEAR,
    READ_PRIMARY,
    WRITE_SECONDARY,
    OVERFLOW_CHECK,
    WRITE_DISABLED
}

public partial class PPU
{
    private byte oamaddr = 0x00;
    private byte oamdata = 0x00;
    private byte oamdma = 0x00;
    private bool is_dma = false;
    public bool InDMA { get => is_dma; }
    private byte[] oam_primary = new byte[0xff];
    private byte[] oam_secondary = new byte[32];
    private SpriteEvalState EvalState = SpriteEvalState.CLEAR;
    private byte sprite_eval_index = 0;
    private byte sprite_eval_m = 0;
    private byte sprite_eval_sprites = 0;
    private byte oam_index = 0;
    private SpriteData[] sprite_output = new SpriteData[8];
    private SpriteData GetSpriteData(byte idx)
    {
        return new SpriteData(oam_primary[idx..(idx + 3)]);
    }
    private void SpriteEvaluation(int scanline, int cycle)
    {
        if (!mask.GetBGRenderEnabled() && !mask.GetSpriteRenderEnabled()) return;
        byte pointer = (byte)(sprite_eval_index * 4);
        byte offset = sprite_eval_m;
        oamaddr = pointer.WrappingAdd(offset);
        if (cycle.InRange(1, 64))
        {
            EvalState = SpriteEvalState.CLEAR;
        }
        else if (cycle.InRange(65, 256))
        {
            if (EvalState != SpriteEvalState.WRITE_DISABLED)
            {
                if ((cycle % 2) == 1)
                {
                    EvalState = SpriteEvalState.READ_PRIMARY;
                }
                else
                {
                    if (sprite_eval_sprites == 8)
                    {
                        EvalState = SpriteEvalState.OVERFLOW_CHECK;
                    }
                    else
                    {
                        EvalState = SpriteEvalState.WRITE_SECONDARY;
                    }
                }
            }
            switch (EvalState)
            {
                case SpriteEvalState.CLEAR:
                    {
                        oamdata = 0xff;
                        oam_secondary[cycle - 1] = 0xff;
                        break;
                    }
                case SpriteEvalState.READ_PRIMARY:
                    {
                        oamdata = oam_primary[pointer + offset];
                        break;
                    }
                case SpriteEvalState.WRITE_SECONDARY:
                    {
                        oam_secondary[oam_index] = oamdata;
                        if (offset == 0)
                        {
                            // Check range
                            if (oamdata == scanline)
                            {
                                // In range
                                sprite_eval_m++;
                                oam_index++;
                            }
                            else
                            {
                                sprite_eval_index++;
                            }
                        }
                        else
                        {
                            oam_index++;
                            if (offset + 1 == 3)
                            {
                                sprite_eval_sprites++;
                                sprite_eval_m = 0;
                                sprite_eval_index++;
                                if (sprite_eval_index == 64)
                                {
                                    sprite_eval_index = 0;
                                    EvalState = SpriteEvalState.WRITE_DISABLED;
                                }
                            }
                            else
                            {
                                sprite_eval_m++;
                            }
                        }
                        break;
                    }
                case SpriteEvalState.OVERFLOW_CHECK:
                    {
                        if (offset == 0)
                        {
                            if (oamdata == scanline)
                            {
                                status |= PPUStatus.SpriteOverflow;
                                sprite_eval_m++;
                            }
                            else
                            {
                                sprite_eval_m++;
                                sprite_eval_index++;
                                if (sprite_eval_index == 64)
                                {
                                    sprite_eval_index = 0;
                                    EvalState = SpriteEvalState.WRITE_DISABLED;
                                }
                            }
                        }
                        else
                        {
                            sprite_eval_m++;
                            if (sprite_eval_m == 3)
                            {
                                sprite_eval_index++;
                                sprite_eval_m = 0;
                                if (sprite_eval_index == 64)
                                {
                                    sprite_eval_index = 0;
                                    EvalState = SpriteEvalState.WRITE_DISABLED;
                                }
                            }
                        }
                        break;
                    }
                case SpriteEvalState.WRITE_DISABLED:
                    {
                        // Ignore
                        break;
                    }
            }
        }
    }
    private void SpriteFetch(int scanline, int cycle)
    {
        // SPRITE FETCH
        if (cycle == 257)
        {
            sprite_eval_index = 0;
        }
        byte soff = (byte)((cycle - 257) % 7);
        byte spo = (byte)(sprite_eval_index * 4);
        if (soff.InRange(0, 3))
        {
            oamdata = oam_secondary[spo + soff];
        }
        else if (soff.InRange(4, 7))
        {
            oamdata = oam_secondary[spo + soff];
            if (soff == 7)
            {
                sprite_output[sprite_eval_index] = new SpriteData(oam_secondary[spo..(spo + 4)]);
                sprite_eval_index++;
            }
        }
    }
    private void WriteOAMDATA(byte data)
    {
        oamdata = data;
        oam_primary[oamaddr] = oamdata;
        oamaddr = oamaddr.WrappingInc();
    }
    private byte ReadOAMDATA()
    {
        oamdata = oam_primary[oamaddr];
        return oamdata;
    }
    private void BeginOAMDMA(byte data)
    {
        oamdma = data;
        is_dma = true;
    }
}