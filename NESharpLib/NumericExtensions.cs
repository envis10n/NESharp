namespace NESharpLib
{
    public static class UnsignedExtensions
    {
        /* 8-bit */
        /// <summary>
        /// Shift all bits left, returning the MSB.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static byte ShiftOut(this ref byte a)
        {
            byte res = (byte)((a & 0b10000000) >> 7);
            a = (byte)(a << 1);
            return res;
        }
        /// <summary>
        /// Shift all bits left, shifting the MSB of B into the LSB of A.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Shift(this ref byte a, ref byte b)
        {
            byte o = b.ShiftOut();
            a.ShiftOut();
            a |= o;
        }
        public static byte GetBit(this byte a, byte bit)
        {
            byte mask = (byte)(0b1 << bit);
            return (byte)((a & mask) >> bit);
        }
        public static void SetBit(this ref byte a, byte bit, bool state)
        {
            byte mask = (byte)(0b1 << bit);
            if (state)
                a |= mask;
            else
                a = (byte)(a & ~mask);
        }
        public static bool InRange(this byte a, byte start, byte end)
        {
            return start <= a && a <= end;
        }
        public static bool InRange(this byte a, uint start, uint end)
        {
            return start <= a && a <= end;
        }
        public static bool InRange(this byte a, int start, int end)
        {
            return start <= a && a <= end;
        }
        public static byte BitAND(this byte a, byte b)
        {
            return (byte)(a & b);
        }
        public static byte BitOR(this byte a, byte b)
        {
            return (byte)(a | b);
        }
        public static byte BitXOR(this byte a, byte b)
        {
            return (byte)(a ^ b);
        }
        public static byte WrappingAdd(this byte a, sbyte b)
        {
            return unchecked((byte)(a + b));
        }
        public static byte WrappingAdd(this byte a, byte b)
        {
            return unchecked((byte)(a + b));
        }
        public static byte WrappingAdd(this byte a, ushort b)
        {
            return unchecked((byte)(a + b));
        }
        public static byte WrappingAdd(this byte a, int b)
        {
            return unchecked((byte)(a + b));
        }
        public static byte WrappingAdd(this byte a, uint b)
        {
            return unchecked((byte)(a + b));
        }
        public static byte WrappingSub(this byte a, byte b)
        {
            return unchecked((byte)(a - b));
        }
        public static byte WrappingSub(this byte a, ushort b)
        {
            return unchecked((byte)(a - b));
        }
        public static byte WrappingSub(this byte a, int b)
        {
            return unchecked((byte)(a - b));
        }
        public static byte WrappingSub(this byte a, uint b)
        {
            return unchecked((byte)(a - b));
        }
        public static byte WrappingInc(this byte a)
        {
            return a.WrappingAdd(1);
        }
        public static byte WrappingDec(this byte a)
        {
            return a.WrappingSub(1);
        }
        public static byte SetUpperNybble(this byte a, byte b)
        {
            byte lower = (byte)(a & 0b1111);
            return (byte)(b | lower);
        }
        public static byte SetLowerNybble(this byte a, byte b)
        {
            byte upper = (byte)(a & 0b11110000);
            return (byte)(upper | b);
        }
        public static string ToHexString(this byte a)
        {
            return string.Format($"{a:X2}");
        }
        public static string ToBinaryString(this byte a)
        {
            return string.Format($"{a:b8}");
        }
        /* 16-bit */
        public static bool InRange(this ushort a, ushort start, ushort end)
        {
            return start <= a && a <= end;
        }
        public static bool InRange(this ushort a, uint start, uint end)
        {
            return start <= a && a <= end;
        }
        public static bool InRange(this ushort a, int start, int end)
        {
            return start <= a && a <= end;
        }
        public static ushort WrappingAdd(this ushort a, sbyte b)
        {
            return unchecked((ushort)(a + b));
        }
        public static ushort WrappingAdd(this ushort a, ushort b)
        {
            return unchecked((ushort)(a + b));
        }
        public static ushort WrappingAdd(this ushort a, byte b)
        {
            return unchecked((ushort)(a + b));
        }
        public static ushort WrappingAdd(this ushort a, int b)
        {
            return unchecked((ushort)(a + b));
        }
        public static ushort WrappingAdd(this ushort a, uint b)
        {
            return unchecked((ushort)(a + b));
        }
        public static ushort WrappingSub(this ushort a, ushort b)
        {
            return unchecked((ushort)(a - b));
        }
        public static ushort WrappingSub(this ushort a, byte b)
        {
            return unchecked((ushort)(a - b));
        }
        public static ushort WrappingSub(this ushort a, int b)
        {
            return unchecked((ushort)(a - b));
        }
        public static ushort WrappingSub(this ushort a, uint b)
        {
            return unchecked((ushort)(a - b));
        }
        public static ushort WrappingInc(this ushort a)
        {
            return a.WrappingAdd(1);
        }
        public static ushort WrappingDec(this ushort a)
        {
            return a.WrappingSub(1);
        }
        public static byte GetUpperByte(this ushort a)
        {
            return (byte)((a & 0xff00) >> 8);
        }
        public static byte GetLowerByte(this ushort a)
        {
            return (byte)(a & 0xff);
        }
        public static ushort SetUpperByte(this ushort a, byte b)
        {
            byte lower = (byte)(a & 0xff);
            return (ushort)((b << 8) | lower);
        }
        public static ushort SetLowerByte(this ushort a, byte b)
        {
            byte upper = (byte)(a & 0xff00);
            return (ushort)(upper | b);
        }
        public static string ToHexString(this ushort a)
        {
            return string.Format($"{a:X4}");
        }
        public static string ToBinaryString(this ushort a)
        {
            return string.Format($"{a:b16}");
        }
        public static byte[] ToLEBytes(this ushort a)
        {
            byte le = (byte)((a & 0xff00) >> 8);
            byte he = (byte)(a & 0xff);
            return [he, le];
        }
        public static byte[] ToBEBytes(this ushort a)
        {
            byte he = (byte)((a & 0xff00) >> 8);
            byte le = (byte)(a & 0xff);
            return [he, le];
        }
        public static ushort FromLEBytes(this byte[] bytes)
        {
            byte le = bytes[1];
            byte he = bytes[0];
            return (ushort)((le << 8) | he);
        }
        public static ushort FromBEBytes(this byte[] bytes)
        {
            byte le = bytes[1];
            byte he = bytes[0];
            return (ushort)((he << 8) | le);
        }
        /* int */
        public static bool InRange(this int a, int start, int end)
        {
            return start <= a && a <= end;
        }
    }
}