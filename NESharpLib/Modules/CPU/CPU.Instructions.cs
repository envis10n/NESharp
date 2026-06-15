namespace NESharpLib.Modules.CPU;

public record struct InstructionResult
{
    public bool PageCrossed { get; init; }
    public bool BranchTaken { get; init; }
}

public partial class CPU
{
    private Dictionary<string, Func<byte, Instruction, InstructionResult>> InstructionHandlers = [];
    private byte CurrentOpCode = 0;
    private bool WouldCarry(byte a, byte b)
    {
        byte c = a.WrappingAdd(b);
        return a > c;
    }
    private bool WouldOverflow(byte a, byte b, byte res)
    {
        return ((res ^ a) & (res ^ b) & 0x80) != 0;
    }
    private byte GetCarry()
    {
        return (byte)(Status.HasFlag(CPUStatus.Carry) ? 1 : 0);
    }
    private InstructionResult ADC(byte opcode, Instruction instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode);
        byte memory = bus.ReadByte(addr).WrappingAdd(GetCarry());
        bool carry = WouldCarry(Accumulator, memory);
        byte result = Accumulator.WrappingAdd(memory);
        bool overflow = WouldOverflow(Accumulator, memory, result);
        Status.SetFlagState(carry, CPUStatus.Carry);
        Status.SetFlagState(overflow, CPUStatus.Overflow);
        UpdateZeroNegative(Accumulator);
        return new InstructionResult { PageCrossed = cross };
    }
    private InstructionResult AND(byte opcode, Instruction instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode);
        byte memory = bus.ReadByte(addr);
        Accumulator = Accumulator.BitAND(memory);
        UpdateZeroNegative(Accumulator);
        return new InstructionResult { PageCrossed = cross };
    }
    private InstructionResult ASL(byte opcode, Instruction instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode);
        byte value = instruction.Mode == AddressingMode.Accumulator ? Accumulator : bus.ReadByte(addr);
        byte old = value;
        bool carry = value.ShiftOut() == 1;
        if (instruction.Mode == AddressingMode.Accumulator)
            Accumulator = value;
        else
        {
            bus.WriteByte(addr, old);
            bus.WriteByte(addr, value);
        }
        Status.SetFlagState(carry, CPUStatus.Carry);
        UpdateZeroNegative(value);
        return new InstructionResult { PageCrossed = cross };
    }
    private InstructionResult Branch(bool state)
    {
        (ushort addr, bool cross) = GetOperandAddress(AddressingMode.Relative);
        if (state)
        {
            ProgramCounter = addr;
            return new InstructionResult { PageCrossed = cross, BranchTaken = true };
        }
        else
        {
            return new InstructionResult { };
        }
    }
    private InstructionResult BCC(byte opcode, Instruction instruction)
    {
        return Branch(Status.HasFlag(CPUStatus.Carry));
    }
    private InstructionResult BCS(byte opcode, Instruction instruction)
    {
        return Branch(!Status.HasFlag(CPUStatus.Carry));
    }
    private InstructionResult BEQ(byte opcode, Instruction instruction)
    {
        return Branch(Status.HasFlag(CPUStatus.Zero));
    }
    private InstructionResult BMI(byte opcode, Instruction instruction)
    {
        return Branch(Status.HasFlag(CPUStatus.Negative));
    }
    private InstructionResult BNE(byte opcode, Instruction instruction)
    {
        return Branch(!Status.HasFlag(CPUStatus.Zero));
    }
    private InstructionResult BPL(byte opcode, Instruction instruction)
    {
        return Branch(!Status.HasFlag(CPUStatus.Negative));
    }
    private InstructionResult BVC(byte opcode, Instruction instruction)
    {
        return Branch(!Status.HasFlag(CPUStatus.Overflow));
    }
    private InstructionResult BVS(byte opcode, Instruction instruction)
    {
        return Branch(Status.HasFlag(CPUStatus.Overflow));
    }
    private InstructionResult BIT(byte opcode, Instruction instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode);
        byte memory = bus.ReadByte(addr);
        byte result = Accumulator.BitAND(memory);
        Status.SetZero(result == 0);
        Status.SetOverflow((memory & 0b1000000) != 0);
        Status.SetNegative((memory & 0b10000000) != 0);
        return new InstructionResult { };
    }
    private InstructionResult BRK(byte opcode, Instruction instruction)
    {
        Interrupt(InterruptType.BRK);
        return new InstructionResult { };
    }
    private InstructionResult CLC(byte opcode, Instruction instruction)
    {
        Status.SetCarry(false);
        return new InstructionResult { };
    }
    private InstructionResult CLD(byte opcode, Instruction instruction)
    {
        Status.SetFlagState(false, CPUStatus.Decimal);
        return new InstructionResult { };
    }
    private InstructionResult CLI(byte opcode, Instruction instruction)
    {
        Status.SetInterruptDisable(false);
        return new InstructionResult { };
    }
    private InstructionResult CLV(byte opcode, Instruction instruction)
    {
        Status.SetOverflow(false);
        return new InstructionResult { };
    }
    private void Compare(byte a, byte b)
    {
        byte result = a.WrappingSub(b);
        Status.SetCarry(a >= b);
        Status.SetZero(a == b);
        Status.SetNegative(result.GetBit(7) == 1);
    }
    private InstructionResult CMP(byte opcode, Instruction instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode);
        byte memory = bus.ReadByte(addr);
        Compare(Accumulator, memory);
        return new InstructionResult { PageCrossed = cross };
    }
    private InstructionResult CPX(byte opcode, Instruction instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode);
        byte memory = bus.ReadByte(addr);
        Compare(X, memory);
        return new InstructionResult { };
    }
    private InstructionResult CPY(byte opcode, Instruction instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode);
        byte memory = bus.ReadByte(addr);
        Compare(Y, memory);
        return new InstructionResult { };
    }
    private InstructionResult DEC(byte opcode, Instruction instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode);
        byte memory = bus.ReadByte(addr);
        memory = memory.WrappingDec();
        bus.WriteByte(addr, memory);
        UpdateZeroNegative(memory);
        return new InstructionResult { };
    }
    private InstructionResult DEX(byte opcode, Instruction instruction)
    {
        X = X.WrappingDec();
        UpdateZeroNegative(X);
        return new InstructionResult { };
    }
    private InstructionResult DEY(byte opcode, Instruction instruction)
    {
        Y = Y.WrappingDec();
        UpdateZeroNegative(Y);
        return new InstructionResult { };
    }
}