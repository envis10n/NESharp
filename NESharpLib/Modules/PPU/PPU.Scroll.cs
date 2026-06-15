namespace NESharpLib.Modules.PPU;

public partial class PPU
{
    private void IncrementCoarseX()
    {
        if ((V & 0x001f) == 31)
        {
            V = (ushort)(V & ~0x001f);
            V ^= 0x0400;
        }
        else
        {
            V = V.WrappingInc();
        }
    }
    private void IncrementY()
    {
        if ((V & 0x7000) != 0x7000)
        {
            V = V.WrappingAdd(0x1000);
        }
        else
        {
            V = (ushort)(V & ~0x7000);
            int y = (V & 0x03e0) >> 5;
            if (y == 29)
            {
                y = 0;
                V ^= 0x0800;
            }
            else if (y == 31)
            {
                y = 0;
            }
            else
            {
                y++;
            }
            V = (ushort)((V & ~0x03e0) | (y << 5));
        }
    }
    private ushort GetPatternAddr(byte tile, byte plane)
    {
        ushort v = V;
        v = (ushort)(v | (tile << 4) | (plane << 3));
        return v;
    }
    private ushort GetNextTileAddr()
    {
        return (ushort)(0x2000 | (V & 0x0fff));
    }
    private ushort GetNextAttributeAddr()
    {
        return (ushort)(0x23c0 | (V & 0x0c00) | ((V >> 4) & 0x38) | ((V >> 2) & 0x07));
    }
}