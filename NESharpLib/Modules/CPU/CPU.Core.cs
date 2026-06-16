namespace NESharpLib.Modules.CPU;
public partial class CPU
{
    private Bus bus;
    /// <summary>
    /// 8-bit accumulator
    /// </summary>
    public byte Accumulator = 0;
    /// <summary>
    /// Current program address
    /// </summary>
    public ushort ProgramCounter = 0xfffc;
    /// <summary>
    /// Current stack pointer
    /// </summary>
    public byte SP = 0xfd;
    public ushort StackAddress
    {
        get
        {
            ushort addr = 0x0100;
            addr = addr.WrappingAdd(SP);
            return addr;
        }
    }
    /// <summary>
    /// 8-bit X register
    /// </summary>
    public byte X = 0;
    /// <summary>
    /// 8-bit Y register
    /// </summary>
    public byte Y = 0;
    /// <summary>
    /// Current CPU status flags
    /// </summary>
    public CPUStatus Status = CPUStatus.Empty;
    public CPU(Bus _bus)
    {
        bus = _bus;
        AssignHandlers();
    }
    private void UpdateZeroNegative(byte value)
    {
        Status.SetZero(value == 0);
        Status.SetNegative((value & 0x80) != 0);
    }
    public void StackPush(byte data)
    {
        ushort addr = StackAddress;
        bus.WriteByte(addr, data);
        SP = SP.WrappingDec();
    }
    public byte StackPop()
    {
        SP = SP.WrappingInc();
        ushort addr = StackAddress;
        byte res = bus.ReadByte(addr);
        return res;
    }
    public void StackPushShort(ushort data)
    {
        byte[] bytes = data.ToLEBytes();
        StackPush(bytes[1]);
        StackPush(bytes[0]);
    }
    public ushort StackPopShort()
    {
        byte[] bytes = [StackPop(), StackPop()];
        return bytes.FromLEBytes();
    }
    public int ExecuteNext()
    {
        byte op = bus.ReadByte(ProgramCounter);
        if (!OpCodes.OpCodesMap.ContainsKey(op))
            throw new Exception(string.Format("Unknown opcode {0:X2}", op));

        Instruction instruction = OpCodes.OpCodesMap[op];

        if (!InstructionHandlers.ContainsKey(instruction.Mnemonic))
            throw new Exception(string.Format("Unhandled instruction {0}", instruction.Mnemonic));

        byte[] bytes = new byte[instruction.Length];

        bytes[0] = op;

        for (int i = 1; i < instruction.Length; i++)
        {
            bytes[i] = bus.ReadByte(ProgramCounter.WrappingAdd(i));
        }

        ProgramCounter = ProgramCounter.WrappingInc();

        ushort PCState = ProgramCounter;

        OpArgs args = new OpArgs(bytes, instruction);

        OpResult result = InstructionHandlers[instruction.Mnemonic].Invoke(args);
        int cycles = instruction.Cycles;
        if (result.HasFlag(OpResult.CROSS))
            cycles++;
        if (result.HasFlag(OpResult.BRANCH))
            cycles++;

        if (PCState == ProgramCounter)
            ProgramCounter = ProgramCounter.WrappingAdd(instruction.Length - 1);

        return cycles;
    }
}
