using static SFML.Window.Keyboard;

namespace NESharpLib
{
    public class Addressable
    {
        private SortedDictionary<(ushort, ushort), Func<ushort, byte>> ReadHandlers = [];
        private (ushort, ushort)[] ReadKeys { get => [.. ReadHandlers.Keys]; }
        private SortedDictionary<(ushort, ushort), Action<ushort, byte>> WriteHandlers = [];
        private (ushort, ushort)[] WriteKeys { get => [.. WriteHandlers.Keys]; }
        private bool TryGetKeyInRange(ushort a, out (ushort, ushort) key, bool write = false)
        {
            (ushort, ushort)[] keys = write ? WriteKeys : ReadKeys;
            key = (0, 0);
            if (keys.Length == 0) return false;
            foreach ((ushort start, ushort end) in keys)
            {
                if (a.InRange(start, end))
                {
                    key = (start, end);
                    return true;
                }
            }
            return false;
        }
        private bool TryGetReadKey(ushort a, out (ushort, ushort) key)
        {
            return TryGetKeyInRange(a, out key);
        }
        private bool TryGetWriteKey(ushort a, out (ushort, ushort) key)
        {
            return TryGetKeyInRange(a, out key, true);
        }
        public void HandleRead(ushort start, ushort end, Func<ushort, byte> handler)
        {
            (ushort, ushort) key = (start, end);
            if (TryGetReadKey(start, out (ushort, ushort) _) || TryGetReadKey(end, out (ushort, ushort) _) || ReadHandlers.ContainsKey(key))
                throw new Exception("address range conflict.");
            ReadHandlers.Add(key, handler);
        }
        public void HandleWrite(ushort start, ushort end, Action<ushort, byte> handler)
        {
            (ushort, ushort) key = (start, end);
            if (TryGetWriteKey(start, out (ushort, ushort) _) || TryGetWriteKey(end, out (ushort, ushort) _) || WriteHandlers.ContainsKey(key))
                throw new Exception("address range conflict.");
            WriteHandlers.Add(key, handler);
        }
        public byte ReadByte(ushort address)
        {
            if (TryGetReadKey(address, out (ushort, ushort) key))
                return ReadHandlers[key].Invoke(address);
            else
                return 0;
        }
        public ushort ReadShort(ushort address)
        {
            byte[] bytes = new byte[2];
            bytes[1] = ReadByte(address);
            bytes[0] = ReadByte(address.WrappingInc());
            return bytes.FromLEBytes();
        }
        public void WriteShort(ushort address, ushort data)
        {
            byte[] bytes = data.ToLEBytes();
            WriteByte(address, bytes[1]);
            WriteByte(address.WrappingInc(), bytes[0]);
        }
        public sbyte ReadSByte(ushort address)
        {
            return (sbyte)ReadByte(address);
        }
        public void WriteSByte(ushort address, sbyte data)
        {
            WriteByte(address, (byte)data);
        }
        public void WriteByte(ushort address, byte data)
        {
            if (TryGetWriteKey(address, out (ushort, ushort) key))
            {
                WriteHandlers[key].Invoke(address, data);
            }
        }
    }
}