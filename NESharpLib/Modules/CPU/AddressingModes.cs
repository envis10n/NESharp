namespace NESharpLib.Modules.CPU;

public enum AddressingMode : byte
{
    Accumulator,
    Implicit,
    Immediate,
    ZeroPage,
    ZeroPage_X,
    ZeroPage_Y,
    Absolute,
    Absolute_X,
    Absolute_Y,
    Relative,
    Indirect,
    Indirect_X,
    Indirect_Y
}

public partial class CPU
{
    private bool PageCross(ushort addr1, ushort addr2)
    {
        return (ushort)(addr1 & 0xFF00) != (ushort)(addr2 & 0xFF00);
    }
    private (ushort, bool) GetOperandAddress(AddressingMode mode)
    {
        switch (mode)
        {
            case AddressingMode.Implicit:
            case AddressingMode.Accumulator:
                return (0, false);
            case AddressingMode.Immediate:
                return (ProgramCounter, false);
            case AddressingMode.ZeroPage:
                return (bus.ReadByte(ProgramCounter), false);
            case AddressingMode.ZeroPage_X:
                return (bus.ReadByte(ProgramCounter).WrappingAdd(X), false);
            case AddressingMode.ZeroPage_Y:
                return (bus.ReadByte(ProgramCounter).WrappingAdd(Y), false);
            case AddressingMode.Absolute:
            case AddressingMode.Indirect:
                return (bus.ReadShort(ProgramCounter), false);
            case AddressingMode.Absolute_X:
                {
                    ushort addr = bus.ReadShort(ProgramCounter);
                    ushort naddr = addr.WrappingAdd(X);
                    return (naddr, PageCross(addr, naddr));
                }
            case AddressingMode.Absolute_Y:
                {
                    ushort addr = bus.ReadShort(ProgramCounter);
                    ushort naddr = addr.WrappingAdd(Y);
                    return (naddr, PageCross(addr, naddr));
                }
            case AddressingMode.Relative:
                {
                    sbyte offset = bus.ReadSByte(ProgramCounter);
                    ushort addr = ProgramCounter.WrappingInc();
                    ushort naddr = addr.WrappingAdd(offset);
                    return (naddr, PageCross(addr, naddr));
                }
            case AddressingMode.Indirect_X:
                {
                    byte offset = bus.ReadByte(ProgramCounter);
                    byte a = bus.ReadByte(offset.WrappingAdd(X));
                    byte b = bus.ReadByte(offset.WrappingAdd(X.WrappingInc()));
                    return ((ushort)(a.WrappingAdd(b) * 256), false);
                }
            case AddressingMode.Indirect_Y:
                {
                    byte offset = bus.ReadByte(ProgramCounter);
                    byte a = bus.ReadByte(offset);
                    byte b = bus.ReadByte(offset.WrappingInc());
                    ushort addr = (ushort)(a.WrappingAdd(b) * 256);
                    ushort naddr = addr.WrappingAdd(Y);
                    return (naddr, PageCross(addr, naddr));
                }
            default:
                return (0, false);
        }
    }
}
