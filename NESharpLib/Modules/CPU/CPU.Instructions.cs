namespace NESharpLib.Modules.CPU;

[Flags]
public enum OpResult : byte
{
    EMPTY = 0b0,
    CROSS = 0b1,
    BRANCH = 0b10,
}

public record struct ExecutionResult
{
    public ushort PC { get; init; }
    public byte SP { get; init; }
    public byte Accumulator { get; init; }
    public byte X { get; init; }
    public byte Y { get; init; }
    public CPUStatus Status { get; init; }
    public OpArgs Instruction { get; init; }
    public ushort OperandAddress { get; init; }
    public bool PageCross { get; init; }
}

public partial class CPU
{
    private Dictionary<string, Func<OpArgs, OpResult>> InstructionHandlers = [];
    private void AssignHandlers()
    {
        InstructionHandlers["ADC"] = ADC;
        InstructionHandlers["AND"] = AND;
        InstructionHandlers["ASL"] = ASL;
        InstructionHandlers["BCC"] = BCC;
        InstructionHandlers["BCS"] = BCS;
        InstructionHandlers["BEQ"] = BEQ;
        InstructionHandlers["BIT"] = BIT;
        InstructionHandlers["BMI"] = BMI;
        InstructionHandlers["BNE"] = BNE;
        InstructionHandlers["BPL"] = BPL;
        InstructionHandlers["BRK"] = BRK;
        InstructionHandlers["BVC"] = BVC;
        InstructionHandlers["BVS"] = BVS;
        InstructionHandlers["CLC"] = CLC;
        InstructionHandlers["CLD"] = CLD;
        InstructionHandlers["CLI"] = CLI;
        InstructionHandlers["CLV"] = CLV;
        InstructionHandlers["CMP"] = CMP;
        InstructionHandlers["CPX"] = CPX;
        InstructionHandlers["CPY"] = CPY;
        InstructionHandlers["DEC"] = DEC;
        InstructionHandlers["DEX"] = DEX;
        InstructionHandlers["DEY"] = DEY;
        InstructionHandlers["EOR"] = EOR;
        InstructionHandlers["INC"] = INC;
        InstructionHandlers["INX"] = INX;
        InstructionHandlers["INY"] = INY;
        InstructionHandlers["JMP"] = JMP;
        InstructionHandlers["JSR"] = JSR;
        InstructionHandlers["LDA"] = LDA;
        InstructionHandlers["LDX"] = LDX;
        InstructionHandlers["LDY"] = LDY;
        InstructionHandlers["LSR"] = LSR;
        InstructionHandlers["NOP"] = NOP;
        InstructionHandlers["*NOP"] = NOP;
        InstructionHandlers["ORA"] = ORA;
        InstructionHandlers["PHA"] = PHA;
        InstructionHandlers["PHP"] = PHP;
        InstructionHandlers["PLA"] = PLA;
        InstructionHandlers["PLP"] = PLP;
        InstructionHandlers["ROL"] = ROL;
        InstructionHandlers["ROR"] = ROR;
        InstructionHandlers["RTI"] = RTI;
        InstructionHandlers["RTS"] = RTS;
        InstructionHandlers["SBC"] = SBC;
        InstructionHandlers["SEC"] = SEC;
        InstructionHandlers["SED"] = SED;
        InstructionHandlers["SEI"] = SEI;
        InstructionHandlers["STA"] = STA;
        InstructionHandlers["STX"] = STX;
        InstructionHandlers["STY"] = STY;
        InstructionHandlers["TAX"] = TAX;
        InstructionHandlers["TAY"] = TAY;
        InstructionHandlers["TSX"] = TSX;
        InstructionHandlers["TXA"] = TXA;
        InstructionHandlers["TXS"] = TXS;
        InstructionHandlers["TYA"] = TYA;
        InstructionHandlers["*ALR"] = ALR;
        InstructionHandlers["*ANC"] = ANC;
        InstructionHandlers["*ARR"] = ARR;
        InstructionHandlers["*AXS"] = AXS;
        InstructionHandlers["*LAX"] = LAX;
        InstructionHandlers["*SAX"] = SAX;
        InstructionHandlers["*DCP"] = DCP;
        InstructionHandlers["*ISC"] = ISC;
        InstructionHandlers["*ISB"] = ISC;
        InstructionHandlers["*RLA"] = RLA;
        InstructionHandlers["*RRA"] = RRA;
        InstructionHandlers["*SLO"] = SLO;
        InstructionHandlers["*SRE"] = SRE;
        InstructionHandlers["*SHX"] = SHX;
        InstructionHandlers["*SHY"] = SHY;
        InstructionHandlers["*SBC"] = USBC;
    }
    public string TraceInstruction(ExecutionResult e)
    {
        string hexStr = e.Instruction.Hex;
        string memory = "";
        ushort addr = e.OperandAddress;
        switch (e.Instruction.Length)
        {
            case 1:
                {
                    
                    switch (e.Instruction.Mode)
                    {
                        case AddressingMode.Accumulator:
                            {
                                memory = "A ";
                                break;
                            }
                        default:
                            {
                                memory = "  ";
                                break;
                            }
                    }
                    break;
                }
            case 2:
                {
                    byte operand = e.Instruction.OperandByte;
                    byte stored = 0;
                    if (e.Instruction.Mode != AddressingMode.Immediate)
                        stored = bus.ReadByte(addr);
                    switch (e.Instruction.Mode)
                    {
                        case AddressingMode.Immediate:
                            {
                                memory = string.Format($"#${operand:X2}");
                                break;
                            }
                        case AddressingMode.ZeroPage:
                            {
                                memory = string.Format($"${operand:X2} = {stored:X2}");
                                break;
                            }
                        case AddressingMode.ZeroPage_X:
                            {
                                memory = string.Format($"${operand:X2},X @ {addr:X2} = {stored:X2}");
                                break;
                            }
                        case AddressingMode.ZeroPage_Y:
                            {
                                memory = string.Format($"${operand:X2},Y @ {addr:X2} = {stored:X2}");
                                break;
                            }
                        case AddressingMode.Indirect_X:
                            {
                                memory = string.Format($"(${operand:X2},X) @ {addr:X2} = 0000 = 00");
                                break;
                            }
                        case AddressingMode.Indirect_Y:
                            {
                                memory = string.Format($"(${operand:X2}),Y = {addr.WrappingSub(e.Y):X4} @ {addr:X4} = {stored:X2}");
                                break;
                            }
                        default:
                            {
                                memory = string.Format($"${addr:X4}");
                                break;
                            }
                    }
                    break;
                }
            case 3:
                {
                    ushort address = e.Instruction.OperandShort;
                    byte stored = bus.ReadByte(addr);
                    switch (e.Instruction.Mode)
                    {
                        case AddressingMode.Absolute:
                            {
                                memory = string.Format($"${address:X4} = {stored:X2}");
                                break;
                            }
                        case AddressingMode.Absolute_X:
                            {
                                memory = string.Format($"${address:X4},X @ {addr:X4} = {stored:X2}");
                                break;
                            }
                        case AddressingMode.Absolute_Y:
                            {
                                memory = string.Format($"${address:X4},Y @ {addr:X4} = {stored:X2}");
                                break;
                            }
                        default:
                            {
                                if (e.Instruction.OpCode == 0x6c)
                                {
                                    memory = string.Format($"(${address:X$}) = {addr:X4}");
                                }
                                else
                                {
                                    memory = string.Format($"${address:X4}");
                                }
                                break;
                            }
                    }
                    break;
                }
        }
        string asmString = string.Format($"{e.PC:X4}  {hexStr,-8} {e.Instruction.Mnemonic,4} {memory}");
        return string.Format($"{asmString,-47} A:{e.Accumulator:X2} X:{e.X:X2} Y:{e.Y:X2} P:{(byte)e.Status:b8} SP:{e.SP:X2} | 02h: {bus.ReadByte(0x0002):X2} 03h: {bus.ReadByte(0x0003):X2}");
    }
    private bool WouldOverflowPos(byte a, byte b, byte res)
    {
        return ((res ^ a) & (res ^ b) & 0x80) != 0;
    }
    private bool WouldOverflowNeg(byte a, byte b, byte res)
    {
        return ((res ^ a) & (res ^ ~b) & 0x80) != 0;
    }
    private byte GetCarry()
    {
        return (byte)(Status.HasFlag(CPUStatus.Carry) ? 1 : 0);
    }
    private OpResult ADC(OpArgs instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte memory = bus.ReadByte(addr).WrappingAdd(GetCarry());
        byte result = Accumulator.WrappingAdd(memory);
        bool carry = result < Accumulator;
        bool overflow = WouldOverflowPos(Accumulator, memory, result);
        Accumulator = result;
        Status.SetFlagState(carry, CPUStatus.Carry);
        Status.SetFlagState(overflow, CPUStatus.Overflow);
        UpdateZeroNegative(Accumulator);
        OpResult res = OpResult.EMPTY;
        if (cross)
            res |= OpResult.CROSS;
        return res;
    }
    private OpResult AND(OpArgs instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte memory = bus.ReadByte(addr);
        Accumulator = Accumulator.BitAND(memory);
        UpdateZeroNegative(Accumulator);
        OpResult res = OpResult.EMPTY;
        if (cross)
            res |= OpResult.CROSS;
        return res;
    }
    private OpResult ASL(OpArgs instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode, ProgramCounter);
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
        OpResult res = OpResult.EMPTY;
        if (cross)
            res |= OpResult.CROSS;
        return res;
    }
    private OpResult Branch(bool state)
    {
        (ushort addr, bool cross) = GetOperandAddress(AddressingMode.Relative, ProgramCounter);
        if (state)
        {
            ProgramCounter = addr;
            OpResult res = OpResult.BRANCH;
            if (cross)
                res |= OpResult.CROSS;
            return res;
        }
        else
        {
            return OpResult.EMPTY;
        }
    }
    private OpResult BCC(OpArgs instruction)
    {
        return Branch(Status.HasFlag(CPUStatus.Carry));
    }
    private OpResult BCS(OpArgs instruction)
    {
        return Branch(!Status.HasFlag(CPUStatus.Carry));
    }
    private OpResult BEQ(OpArgs instruction)
    {
        return Branch(Status.HasFlag(CPUStatus.Zero));
    }
    private OpResult BMI(OpArgs instruction)
    {
        return Branch(Status.HasFlag(CPUStatus.Negative));
    }
    private OpResult BNE(OpArgs instruction)
    {
        return Branch(!Status.HasFlag(CPUStatus.Zero));
    }
    private OpResult BPL(OpArgs instruction)
    {
        return Branch(!Status.HasFlag(CPUStatus.Negative));
    }
    private OpResult BVC(OpArgs instruction)
    {
        return Branch(!Status.HasFlag(CPUStatus.Overflow));
    }
    private OpResult BVS(OpArgs instruction)
    {
        return Branch(Status.HasFlag(CPUStatus.Overflow));
    }
    private OpResult BIT(OpArgs instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte memory = bus.ReadByte(addr);
        byte result = Accumulator.BitAND(memory);
        Status.SetZero(result == 0);
        Status.SetOverflow((memory & 0b1000000) != 0);
        Status.SetNegative((memory & 0b10000000) != 0);
        return OpResult.EMPTY;
    }
    private OpResult BRK(OpArgs instruction)
    {
        Interrupt(InterruptType.BRK);
        return OpResult.EMPTY;
    }
    private OpResult CLC(OpArgs instruction)
    {
        Status.SetCarry(false);
        return OpResult.EMPTY;
    }
    private OpResult CLD(OpArgs instruction)
    {
        Status.SetFlagState(false, CPUStatus.Decimal);
        return OpResult.EMPTY;
    }
    private OpResult CLI(OpArgs instruction)
    {
        Status.SetInterruptDisable(false);
        return OpResult.EMPTY;
    }
    private OpResult CLV(OpArgs instruction)
    {
        Status.SetOverflow(false);
        return OpResult.EMPTY;
    }
    private void Compare(byte a, byte b)
    {
        byte result = a.WrappingSub(b);
        Status.SetCarry(a >= b);
        Status.SetZero(a == b);
        Status.SetNegative(result.GetBit(7) == 1);
    }
    private OpResult CMP(OpArgs instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte memory = bus.ReadByte(addr);
        Compare(Accumulator, memory);
        OpResult res = OpResult.EMPTY;
        if (cross)
            res |= OpResult.CROSS;
        return res;
    }
    private OpResult CPX(OpArgs instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte memory = bus.ReadByte(addr);
        Compare(X, memory);
        return OpResult.EMPTY;
    }
    private OpResult CPY(OpArgs instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte memory = bus.ReadByte(addr);
        Compare(Y, memory);
        return OpResult.EMPTY;
    }
    private OpResult DEC(OpArgs instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte memory = bus.ReadByte(addr);
        memory = memory.WrappingDec();
        bus.WriteByte(addr, memory);
        UpdateZeroNegative(memory);
        return OpResult.EMPTY;
    }
    private OpResult DEX(OpArgs instruction)
    {
        X = X.WrappingDec();
        UpdateZeroNegative(X);
        return OpResult.EMPTY;
    }
    private OpResult DEY(OpArgs instruction)
    {
        Y = Y.WrappingDec();
        UpdateZeroNegative(Y);
        return OpResult.EMPTY;
    }
    private OpResult EOR(OpArgs instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte memory = bus.ReadByte(addr);
        byte result = Accumulator.BitXOR(memory);
        Accumulator = result;
        UpdateZeroNegative(result);
        OpResult res = OpResult.EMPTY;
        if (cross)
            res |= OpResult.CROSS;
        return res;
    }
    private OpResult INC(OpArgs instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte memory = bus.ReadByte(addr);
        memory = memory.WrappingInc();
        UpdateZeroNegative(memory);
        bus.WriteByte(addr, memory);
        return OpResult.EMPTY;
    }
    private OpResult INX(OpArgs instruction)
    {
        X = X.WrappingInc();
        UpdateZeroNegative(X);
        return OpResult.EMPTY;
    }
    private OpResult INY(OpArgs instruction)
    {
        Y = Y.WrappingInc();
        UpdateZeroNegative(Y);
        return OpResult.EMPTY;
    }
    private OpResult JMP(OpArgs instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode, ProgramCounter);
        if (instruction.Mode == AddressingMode.Indirect)
        {
            ushort addr2 = addr.WrappingInc();
            if (addr.GetLowerByte() == 0xff)
            {
                addr2 = addr2.SetUpperByte(addr.GetUpperByte()).SetLowerByte(0x0);
            }
            byte[] bytes = [bus.ReadByte(addr), bus.ReadByte(addr2)];
            ProgramCounter = bytes.FromLEBytes();
        } else
        {
            ProgramCounter = addr;
        }
        return OpResult.EMPTY;
    }
    private OpResult JSR(OpArgs instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode, ProgramCounter);
        ushort ret = ProgramCounter.WrappingAdd(2);
        StackPushShort(ret);
        ProgramCounter = addr;
        return OpResult.EMPTY;
    }
    private OpResult LDA(OpArgs instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode, ProgramCounter);
        Accumulator = bus.ReadByte(addr);
        UpdateZeroNegative(Accumulator);
        OpResult res = OpResult.EMPTY;
        if (cross)
            res |= OpResult.CROSS;
        return res;
    }
    private OpResult LDX(OpArgs instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode, ProgramCounter);
        X = bus.ReadByte(addr);
        UpdateZeroNegative(X);
        OpResult res = OpResult.EMPTY;
        if (cross)
            res |= OpResult.CROSS;
        return res;
    }
    private OpResult LDY(OpArgs instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode, ProgramCounter);
        Y = bus.ReadByte(addr);
        UpdateZeroNegative(Y);
        OpResult res = OpResult.EMPTY;
        if (cross)
            res |= OpResult.CROSS;
        return res;
    }
    private OpResult LSR(OpArgs instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte value = addr == 0 ? Accumulator : bus.ReadByte(addr);
        byte carry = value.GetBit(0);
        value = (byte)(value >> 1);
        if (addr == 0)
            Accumulator = value;
        else
            bus.WriteByte(addr, value);
        UpdateZeroNegative(value);
        Status.SetCarry(carry == 1);
        return OpResult.EMPTY;
    }
    private OpResult NOP(OpArgs instruction)
    {
        return OpResult.EMPTY;
    }
    private OpResult ORA(OpArgs instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte memory = bus.ReadByte(addr);
        Accumulator = Accumulator.BitOR(memory);
        UpdateZeroNegative(Accumulator);
        OpResult res = OpResult.EMPTY;
        if (cross)
            res |= OpResult.CROSS;
        return res;
    }
    private OpResult PHA(OpArgs instruction)
    {
        StackPush(Accumulator);
        return OpResult.EMPTY;
    }
    private OpResult PHP(OpArgs instruction)
    {
        byte status = (byte)Status;
        status |= 0b110000;
        StackPush(status);
        return OpResult.EMPTY;
    }
    private OpResult PLA(OpArgs instruction)
    {
        Accumulator = StackPop();
        UpdateZeroNegative(Accumulator);
        return OpResult.EMPTY;
    }
    private OpResult PLP(OpArgs instruction)
    {
        byte status = StackPop();
        Status = (CPUStatus)((byte)Status | (status & 0b11001111));
        return OpResult.EMPTY;
    }
    private OpResult ROL(OpArgs instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte value = addr == 0 ? Accumulator : bus.ReadByte(addr);
        byte old_carry = ((byte)Status).GetBit(0);
        byte new_carry = value.GetBit(7);
        value = (byte)((value << 1) | old_carry);
        if (addr == 0)
            Accumulator = value;
        else
            bus.WriteByte(addr, value);
        UpdateZeroNegative(value);
        return OpResult.EMPTY;
    }
    private OpResult ROR(OpArgs instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte value = addr == 0 ? Accumulator : bus.ReadByte(addr);
        byte old_carry = ((byte)Status).GetBit(7);
        byte new_carry = value.GetBit(0);
        value = (byte)((old_carry << 7) | (value >> 1));
        if (addr == 0)
            Accumulator = value;
        else
            bus.WriteByte(addr, value);
        UpdateZeroNegative(value);
        return OpResult.EMPTY;
    }
    private OpResult RTI(OpArgs instruction)
    {
        byte status = StackPop();
        Status = (CPUStatus)((byte)Status | (status & 0b11001111));
        ProgramCounter = StackPopShort();
        return OpResult.EMPTY;
    }
    private OpResult RTS(OpArgs instruction)
    {
        ProgramCounter = StackPopShort().WrappingInc();
        return OpResult.EMPTY;
    }
    private OpResult SBC(OpArgs instruction)
    {
        (ushort addr, bool cross) = GetOperandAddress(instruction.Mode, ProgramCounter);
        byte memory = bus.ReadByte(addr).WrappingAdd(GetCarry());
        byte result = Accumulator.WrappingAdd(memory);
        bool carry = result < Accumulator;
        bool overflow = WouldOverflowNeg(Accumulator, memory, result);
        Accumulator = result;
        Status.SetFlagState(carry, CPUStatus.Carry);
        Status.SetFlagState(overflow, CPUStatus.Overflow);
        UpdateZeroNegative(Accumulator);
        OpResult res = OpResult.EMPTY;
        if (cross)
            res |= OpResult.CROSS;
        return res;
    }
    private OpResult SEC(OpArgs instruction)
    {
        Status.SetCarry(true);
        return OpResult.EMPTY;
    }
    private OpResult SED(OpArgs instruction)
    {
        Status.SetFlagState(true, CPUStatus.Decimal);
        return OpResult.EMPTY;
    }
    private OpResult SEI(OpArgs instruction)
    {
        Status.SetInterruptDisable(true);
        return OpResult.EMPTY;
    }
    private OpResult STA(OpArgs instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode, ProgramCounter);
        bus.WriteByte(addr, Accumulator);
        return OpResult.EMPTY;
    }
    private OpResult STX(OpArgs instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode, ProgramCounter);
        bus.WriteByte(addr, X);
        return OpResult.EMPTY;
    }
    private OpResult STY(OpArgs instruction)
    {
        (ushort addr, _) = GetOperandAddress(instruction.Mode, ProgramCounter);
        bus.WriteByte(addr, Y);
        return OpResult.EMPTY;
    }
    private OpResult TAX(OpArgs instruction)
    {
        X = Accumulator;
        UpdateZeroNegative(X);
        return OpResult.EMPTY;
    }
    private OpResult TAY(OpArgs instruction)
    {
        Y = Accumulator;
        UpdateZeroNegative(Y);
        return OpResult.EMPTY;
    }
    private OpResult TSX(OpArgs instruction)
    {
        X = SP;
        UpdateZeroNegative(X);
        return OpResult.EMPTY;
    }
    private OpResult TXA(OpArgs instruction)
    {
        Accumulator = X;
        UpdateZeroNegative(Accumulator);
        return OpResult.EMPTY;
    }
    private OpResult TXS(OpArgs instruction)
    {
        SP = X;
        return OpResult.EMPTY;
    }
    private OpResult TYA(OpArgs instruction)
    {
        Accumulator = Y;
        UpdateZeroNegative(Accumulator);
        return OpResult.EMPTY;
    }
    /* UNOFFICIAL INSTRUCTIONS */
    private OpResult ALR(OpArgs instruction)
    {
        OpResult res = AND(new() { Mode = AddressingMode.Immediate });
        res |= LSR(new() { Mode = AddressingMode.Accumulator });
        return res;
    }
    private OpResult ANC(OpArgs instruction)
    {
        OpResult res = AND(new() { Mode = AddressingMode.Immediate });
        byte n = ((byte)Status).GetBit(7);
        Status.SetCarry(n == 1);
        return res;
    }
    private OpResult ARR(OpArgs instruction)
    {
        OpResult res = AND(new() { Mode = AddressingMode.Immediate });
        res |= ROR(new() { Mode = AddressingMode.Accumulator });
        bool carry = ((byte)Status).GetBit(6) == 1;
        bool overflow = (byte)(((byte)Status).GetBit(6) ^ ((byte)Status).GetBit(5)) == 1;
        Status.SetOverflow(overflow);
        Status.SetCarry(carry);
        return res;
    }
    private OpResult AXS(OpArgs instruction)
    {
        byte data = (byte)(Accumulator & X);
        byte result = data.WrappingSub(instruction.OperandByte);
        UpdateZeroNegative(result);
        Status.SetCarry(result > data);
        return OpResult.EMPTY;
    }
    private OpResult LAX(OpArgs args)
    {
        OpResult res = LDA(args);
        res |= TAX(args);
        return res;
    }
    private OpResult SAX(OpArgs args)
    {
        (ushort addr, bool cross) = GetOperandAddress(args.Mode, ProgramCounter);
        byte result = Accumulator.BitAND(X);
        bus.WriteByte(addr, result);
        if (cross) return OpResult.CROSS;
        else return OpResult.EMPTY;
    }
    private OpResult DCP(OpArgs args)
    {
        OpResult res = DEC(args);
        res |= CMP(args);
        return res;
    }
    private OpResult ISC(OpArgs args)
    {
        OpResult res = INC(args);
        res |= SBC(args);
        return res;
    }
    private OpResult RLA(OpArgs args)
    {
        OpResult res = ROL(args);
        res |= AND(args);
        return res;
    }
    private OpResult RRA(OpArgs args)
    {
        OpResult res = ROR(args);
        res |= ADC(args);
        return res;
    }
    private OpResult SLO(OpArgs args)
    {
        OpResult res = ASL(args);
        res |= ORA(args);
        return res;
    }
    private OpResult SRE(OpArgs args)
    {
        OpResult res = LSR(args);
        res |= EOR(args);
        return res;
    }
    private OpResult USBC(OpArgs args)
    {
        (ushort addr, bool cross) = GetOperandAddress(args.Mode, ProgramCounter);
        byte memory = ((byte)0xff).WrappingSub(bus.ReadByte(addr)).WrappingAdd(GetCarry());
        byte result = Accumulator.WrappingAdd(memory);
        bool carry = result < Accumulator;
        bool overflow = WouldOverflowNeg(Accumulator, memory, result);
        Accumulator = result;
        Status.SetFlagState(carry, CPUStatus.Carry);
        Status.SetFlagState(overflow, CPUStatus.Overflow);
        UpdateZeroNegative(Accumulator);
        OpResult res = OpResult.EMPTY;
        if (cross)
            res |= OpResult.CROSS;
        return res;
    }
    private OpResult SHX(OpArgs args)
    {
        (ushort addr, bool cross) = GetOperandAddress(args.Mode, ProgramCounter);
        byte high = addr.WrappingInc().GetUpperByte();
        bus.WriteByte(addr, X.BitAND(high));
        if (cross) return OpResult.CROSS;
        else return OpResult.EMPTY;
    }
    private OpResult SHY(OpArgs args)
    {
        (ushort addr, bool cross) = GetOperandAddress(args.Mode, ProgramCounter);
        byte high = addr.WrappingInc().GetUpperByte();
        bus.WriteByte(addr, Y.BitAND(high));
        if (cross) return OpResult.CROSS;
        else return OpResult.EMPTY;
    }
}