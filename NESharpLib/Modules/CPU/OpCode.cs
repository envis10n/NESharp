namespace NESharpLib.Modules.CPU;

public readonly record struct Instruction
{
    public string Mnemonic { get; init; }
    public int Length { get; init; }
    public int Cycles { get; init; }
    public AddressingMode Mode { get; init; }
    public Instruction(string mnemonic, int length, int cycles, AddressingMode mode)
    {
        Mnemonic = mnemonic;
        Length = length;
        Cycles = cycles;
        Mode = mode;
    }
}

public readonly record struct OpArgs
{
    private byte[] raw { get; init; }
    public byte OpCode
    {
        get
        {
            return raw[0];
        }
    }
    public byte OperandByte
    {
        get
        {
            return raw[1];
        }
    }
    public ushort OperandShort
    {
        get
        {
            return raw[1..3].FromLEBytes();
        }
    }
    public string Hex
    {
        get
        {
            string res = "";
            foreach (byte b in raw)
            {
                res += b.ToHexString();
                res += " ";
            }
            return res.Trim();
        }
    }
    public string Mnemonic { get; init; }
    public int Length { get; init; }
    public int Cycles { get; init; }
    public AddressingMode Mode { get; init; }
    public OpArgs(byte[] data, Instruction instruction)
    {
        raw = data;
        Mnemonic = instruction.Mnemonic;
        Length = instruction.Length;
        Cycles = instruction.Cycles;
        Mode = instruction.Mode;
    }
}

public static class OpCodes
{
    public static Dictionary<byte, Instruction> OpCodesMap { get => OPCODES_MAP; }
    private static readonly Dictionary<byte, Instruction> OPCODES_MAP = new Dictionary<byte, Instruction>() {
            /* General Ops */
            {0x00, new Instruction("BRK", 1, 7, AddressingMode.Implicit)},
            {0xea, new Instruction("NOP", 1, 2, AddressingMode.Implicit)},

            /* Arithmetic */
            {0x69, new Instruction("ADC", 2, 2, AddressingMode.Immediate)},
            {0x65, new Instruction("ADC", 2, 3, AddressingMode.ZeroPage)},
            {0x75, new Instruction("ADC", 2, 4, AddressingMode.ZeroPage_X)},
            {0x6d, new Instruction("ADC", 3, 4, AddressingMode.Absolute)},
            {0x7d, new Instruction("ADC", 3, 4, AddressingMode.Absolute_X)},
            {0x79, new Instruction("ADC", 3, 4, AddressingMode.Absolute_Y)},
            {0x61, new Instruction("ADC", 2, 6, AddressingMode.Indirect_X)},
            {0x71, new Instruction("ADC", 2, 5, AddressingMode.Indirect_Y)},

            {0xe9, new Instruction("SBC", 2, 2, AddressingMode.Immediate)},
            {0xe5, new Instruction("SBC", 2, 3, AddressingMode.ZeroPage)},
            {0xf5, new Instruction("SBC", 2, 4, AddressingMode.ZeroPage_X)},
            {0xed, new Instruction("SBC", 3, 4, AddressingMode.Absolute)},
            {0xfd, new Instruction("SBC", 3, 4, AddressingMode.Absolute_X)},
            {0xf9, new Instruction("SBC", 3, 4, AddressingMode.Absolute_Y)},
            {0xe1, new Instruction("SBC", 2, 6, AddressingMode.Indirect_X)},
            {0xf1, new Instruction("SBC", 2, 5, AddressingMode.Indirect_Y)},

            {0x29, new Instruction("AND", 2, 2, AddressingMode.Immediate)},
            {0x25, new Instruction("AND", 2, 3, AddressingMode.ZeroPage)},
            {0x35, new Instruction("AND", 2, 4, AddressingMode.ZeroPage_X)},
            {0x2d, new Instruction("AND", 3, 4, AddressingMode.Absolute)},
            {0x3d, new Instruction("AND", 3, 4, AddressingMode.Absolute_X)},
            {0x39, new Instruction("AND", 3, 4, AddressingMode.Absolute_Y)},
            {0x21, new Instruction("AND", 2, 6, AddressingMode.Indirect_X)},
            {0x31, new Instruction("AND", 2, 5, AddressingMode.Indirect_Y)},

            {0x49, new Instruction("EOR", 2, 2, AddressingMode.Immediate)},
            {0x45, new Instruction("EOR", 2, 3, AddressingMode.ZeroPage)},
            {0x55, new Instruction("EOR", 2, 4, AddressingMode.ZeroPage_X)},
            {0x4d, new Instruction("EOR", 3, 4, AddressingMode.Absolute)},
            {0x5d, new Instruction("EOR", 3, 4, AddressingMode.Absolute_X)},
            {0x59, new Instruction("EOR", 3, 4, AddressingMode.Absolute_Y)},
            {0x41, new Instruction("EOR", 2, 6, AddressingMode.Indirect_X)},
            {0x51, new Instruction("EOR", 2, 5, AddressingMode.Indirect_Y)},

            {0x09, new Instruction("ORA", 2, 2, AddressingMode.Immediate)},
            {0x05, new Instruction("ORA", 2, 3, AddressingMode.ZeroPage)},
            {0x15, new Instruction("ORA", 2, 4, AddressingMode.ZeroPage_X)},
            {0x0d, new Instruction("ORA", 3, 4, AddressingMode.Absolute)},
            {0x1d, new Instruction("ORA", 3, 4, AddressingMode.Absolute_X)},
            {0x19, new Instruction("ORA", 3, 4, AddressingMode.Absolute_Y)},
            {0x01, new Instruction("ORA", 2, 6, AddressingMode.Indirect_X)},
            {0x11, new Instruction("ORA", 2, 5, AddressingMode.Indirect_Y)},

            /* Shifts */
            {0x0a, new Instruction("ASL", 1, 2, AddressingMode.Accumulator)},
            {0x06, new Instruction("ASL", 2, 5, AddressingMode.ZeroPage)},
            {0x16, new Instruction("ASL", 2, 6, AddressingMode.ZeroPage_X)},
            {0x0e, new Instruction("ASL", 3, 6, AddressingMode.Absolute)},
            {0x1e, new Instruction("ASL", 3, 7, AddressingMode.Absolute_X)},

            {0x4a, new Instruction("LSR", 1, 2, AddressingMode.Accumulator)},
            {0x46, new Instruction("LSR", 2, 5, AddressingMode.ZeroPage)},
            {0x56, new Instruction("LSR", 2, 6, AddressingMode.ZeroPage_X)},
            {0x4e, new Instruction("LSR", 3, 6, AddressingMode.Absolute)},
            {0x5e, new Instruction("LSR", 3, 7, AddressingMode.Absolute_X)},

            {0x2a, new Instruction("ROL", 1, 2, AddressingMode.Accumulator)},
            {0x26, new Instruction("ROL", 2, 5, AddressingMode.ZeroPage)},
            {0x36, new Instruction("ROL", 2, 6, AddressingMode.ZeroPage_X)},
            {0x2e, new Instruction("ROL", 3, 6, AddressingMode.Absolute)},
            {0x3e, new Instruction("ROL", 3, 7, AddressingMode.Absolute_X)},

            {0x6a, new Instruction("ROR", 1, 2, AddressingMode.Accumulator)},
            {0x66, new Instruction("ROR", 2, 5, AddressingMode.ZeroPage)},
            {0x76, new Instruction("ROR", 2, 6, AddressingMode.ZeroPage_X)},
            {0x6e, new Instruction("ROR", 3, 6, AddressingMode.Absolute)},
            {0x7e, new Instruction("ROR", 3, 7, AddressingMode.Absolute_X)},

            {0xe6, new Instruction("INC", 2, 5, AddressingMode.ZeroPage)},
            {0xf6, new Instruction("INC", 2, 6, AddressingMode.ZeroPage_X)},
            {0xee, new Instruction("INC", 3, 6, AddressingMode.Absolute)},
            {0xfe, new Instruction("INC", 3, 7, AddressingMode.Absolute_X)},

            {0xe8, new Instruction("INX", 1, 2, AddressingMode.Implicit)},
            {0xc8, new Instruction("INY", 1, 2, AddressingMode.Implicit)},

            {0xc6, new Instruction("DEC", 2, 5, AddressingMode.ZeroPage)},
            {0xd6, new Instruction("DEC", 2, 6, AddressingMode.ZeroPage_X)},
            {0xce, new Instruction("DEC", 3, 6, AddressingMode.Absolute)},
            {0xde, new Instruction("DEC", 3, 7, AddressingMode.Absolute_X)},

            {0xca, new Instruction("DEX", 1, 2, AddressingMode.Implicit)},
            {0x88, new Instruction("DEY", 1, 2, AddressingMode.Implicit)},

            {0xc9, new Instruction("CMP", 2, 2, AddressingMode.Immediate)},
            {0xc5, new Instruction("CMP", 2, 3, AddressingMode.ZeroPage)},
            {0xd5, new Instruction("CMP", 2, 4, AddressingMode.ZeroPage_X)},
            {0xcd, new Instruction("CMP", 3, 4, AddressingMode.Absolute)},
            {0xdd, new Instruction("CMP", 3, 4, AddressingMode.Absolute_X)},
            {0xd9, new Instruction("CMP", 3, 4, AddressingMode.Absolute_Y)},
            {0xc1, new Instruction("CMP", 2, 6, AddressingMode.Indirect_X)},
            {0xd1, new Instruction("CMP", 2, 5, AddressingMode.Indirect_Y)},

            {0xc0, new Instruction("CPY", 2, 2, AddressingMode.Immediate)},
            {0xc4, new Instruction("CPY", 2, 3, AddressingMode.ZeroPage)},
            {0xcc, new Instruction("CPY", 3, 4, AddressingMode.Absolute)},

            {0xe0, new Instruction("CPX", 2, 2, AddressingMode.Immediate)},
            {0xe4, new Instruction("CPX", 2, 3, AddressingMode.ZeroPage)},
            {0xec, new Instruction("CPX", 3, 4, AddressingMode.Absolute)},


            /* Branching */

            {0x4c, new Instruction("JMP", 3, 3, AddressingMode.Absolute)}, //AddressingMode that acts as Immidiate
            {0x6c, new Instruction("JMP", 3, 5, AddressingMode.Indirect)}, //AddressingMode:Indirect with 6502 bug

            {0x20, new Instruction("JSR", 3, 6, AddressingMode.Absolute)},
            {0x60, new Instruction("RTS", 1, 6, AddressingMode.Implicit)},

            {0x40, new Instruction("RTI", 1, 6, AddressingMode.Implicit)},

            {0xd0, new Instruction("BNE", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.Relative)},
            {0x70, new Instruction("BVS", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.Relative)},
            {0x50, new Instruction("BVC", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.Relative)},
            {0x30, new Instruction("BMI", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.Relative)},
            {0xf0, new Instruction("BEQ", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.Relative)},
            {0xb0, new Instruction("BCS", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.Relative)},
            {0x90, new Instruction("BCC", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.Relative)},
            {0x10, new Instruction("BPL", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.Relative)},

            {0x24, new Instruction("BIT", 2, 3, AddressingMode.ZeroPage)},
            {0x2c, new Instruction("BIT", 3, 4, AddressingMode.Absolute)},


            /* Stores, Loads */
            {0xa9, new Instruction("LDA", 2, 2, AddressingMode.Immediate)},
            {0xa5, new Instruction("LDA", 2, 3, AddressingMode.ZeroPage)},
            {0xb5, new Instruction("LDA", 2, 4, AddressingMode.ZeroPage_X)},
            {0xad, new Instruction("LDA", 3, 4, AddressingMode.Absolute)},
            {0xbd, new Instruction("LDA", 3, 4, AddressingMode.Absolute_X)},
            {0xb9, new Instruction("LDA", 3, 4, AddressingMode.Absolute_Y)},
            {0xa1, new Instruction("LDA", 2, 6, AddressingMode.Indirect_X)},
            {0xb1, new Instruction("LDA", 2, 5, AddressingMode.Indirect_Y)},

            {0xa2, new Instruction("LDX", 2, 2, AddressingMode.Immediate)},
            {0xa6, new Instruction("LDX", 2, 3, AddressingMode.ZeroPage)},
            {0xb6, new Instruction("LDX", 2, 4, AddressingMode.ZeroPage_Y)},
            {0xae, new Instruction("LDX", 3, 4, AddressingMode.Absolute)},
            {0xbe, new Instruction("LDX", 3, 4, AddressingMode.Absolute_Y)},

            {0xa0, new Instruction("LDY", 2, 2, AddressingMode.Immediate)},
            {0xa4, new Instruction("LDY", 2, 3, AddressingMode.ZeroPage)},
            {0xb4, new Instruction("LDY", 2, 4, AddressingMode.ZeroPage_X)},
            {0xac, new Instruction("LDY", 3, 4, AddressingMode.Absolute)},
            {0xbc, new Instruction("LDY", 3, 4, AddressingMode.Absolute_X)},


            {0x85, new Instruction("STA", 2, 3, AddressingMode.ZeroPage)},
            {0x95, new Instruction("STA", 2, 4, AddressingMode.ZeroPage_X)},
            {0x8d, new Instruction("STA", 3, 4, AddressingMode.Absolute)},
            {0x9d, new Instruction("STA", 3, 5, AddressingMode.Absolute_X)},
            {0x99, new Instruction("STA", 3, 5, AddressingMode.Absolute_Y)},
            {0x81, new Instruction("STA", 2, 6, AddressingMode.Indirect_X)},
            {0x91, new Instruction("STA", 2, 6, AddressingMode.Indirect_Y)},

            {0x86, new Instruction("STX", 2, 3, AddressingMode.ZeroPage)},
            {0x96, new Instruction("STX", 2, 4, AddressingMode.ZeroPage_Y)},
            {0x8e, new Instruction("STX", 3, 4, AddressingMode.Absolute)},

            {0x84, new Instruction("STY", 2, 3, AddressingMode.ZeroPage)},
            {0x94, new Instruction("STY", 2, 4, AddressingMode.ZeroPage_X)},
            {0x8c, new Instruction("STY", 3, 4, AddressingMode.Absolute)},


            /* Flags clear */

            {0xD8, new Instruction("CLD", 1, 2, AddressingMode.Implicit)},
            {0x58, new Instruction("CLI", 1, 2, AddressingMode.Implicit)},
            {0xb8, new Instruction("CLV", 1, 2, AddressingMode.Implicit)},
            {0x18, new Instruction("CLC", 1, 2, AddressingMode.Implicit)},
            {0x38, new Instruction("SEC", 1, 2, AddressingMode.Implicit)},
            {0x78, new Instruction("SEI", 1, 2, AddressingMode.Implicit)},
            {0xf8, new Instruction("SED", 1, 2, AddressingMode.Implicit)},

            {0xaa, new Instruction("TAX", 1, 2, AddressingMode.Implicit)},
            {0xa8, new Instruction("TAY", 1, 2, AddressingMode.Implicit)},
            {0xba, new Instruction("TSX", 1, 2, AddressingMode.Implicit)},
            {0x8a, new Instruction("TXA", 1, 2, AddressingMode.Implicit)},
            {0x9a, new Instruction("TXS", 1, 2, AddressingMode.Implicit)},
            {0x98, new Instruction("TYA", 1, 2, AddressingMode.Implicit)},

            /* Stack */
            {0x48, new Instruction("PHA", 1, 3, AddressingMode.Implicit)},
            {0x68, new Instruction("PLA", 1, 4, AddressingMode.Implicit)},
            {0x08, new Instruction("PHP", 1, 3, AddressingMode.Implicit)},
            {0x28, new Instruction("PLP", 1, 4, AddressingMode.Implicit)},

            /* Unofficial */
            {0xc7, new Instruction("*DCP", 2, 5, AddressingMode.ZeroPage)},
            {0xd7, new Instruction("*DCP", 2, 6, AddressingMode.ZeroPage_X)},
            {0xCF,  new Instruction("*DCP", 3, 6, AddressingMode.Absolute)},
            {0xdF,  new Instruction("*DCP", 3, 7, AddressingMode.Absolute_X)},
            {0xdb,  new Instruction("*DCP", 3, 7, AddressingMode.Absolute_Y)},
            {0xd3,  new Instruction("*DCP", 2, 8, AddressingMode.Indirect_Y)},
            {0xc3,  new Instruction("*DCP", 2, 8, AddressingMode.Indirect_X)},


            {0x27,  new Instruction("*RLA", 2, 5, AddressingMode.ZeroPage)},
            {0x37,  new Instruction("*RLA", 2, 6, AddressingMode.ZeroPage_X)},
            {0x2F,  new Instruction("*RLA", 3, 6, AddressingMode.Absolute)},
            {0x3F,  new Instruction("*RLA", 3, 7, AddressingMode.Absolute_X)},
            {0x3b,  new Instruction("*RLA", 3, 7, AddressingMode.Absolute_Y)},
            {0x33,  new Instruction("*RLA", 2, 8, AddressingMode.Indirect_Y)},
            {0x23,  new Instruction("*RLA", 2, 8, AddressingMode.Indirect_X)},

            {0x07,  new Instruction("*SLO", 2, 5, AddressingMode.ZeroPage)},
            {0x17,  new Instruction("*SLO", 2, 6, AddressingMode.ZeroPage_X)},
            {0x0F,  new Instruction("*SLO", 3, 6, AddressingMode.Absolute)},
            {0x1f,  new Instruction("*SLO", 3, 7, AddressingMode.Absolute_X)},
            {0x1b,  new Instruction("*SLO", 3, 7, AddressingMode.Absolute_Y)},
            {0x03,  new Instruction("*SLO", 2, 8, AddressingMode.Indirect_X)},
            {0x13,  new Instruction("*SLO", 2, 8, AddressingMode.Indirect_Y)},

            {0x47,  new Instruction("*SRE", 2, 5, AddressingMode.ZeroPage)},
            {0x57,  new Instruction("*SRE", 2, 6, AddressingMode.ZeroPage_X)},
            {0x4F,  new Instruction("*SRE", 3, 6, AddressingMode.Absolute)},
            {0x5f,  new Instruction("*SRE", 3, 7, AddressingMode.Absolute_X)},
            {0x5b,  new Instruction("*SRE", 3, 7, AddressingMode.Absolute_Y)},
            {0x43,  new Instruction("*SRE", 2, 8, AddressingMode.Indirect_X)},
            {0x53,  new Instruction("*SRE", 2, 8, AddressingMode.Indirect_Y)},


            {0x80,  new Instruction("*NOP", 2,2, AddressingMode.Immediate)},
            {0x82,  new Instruction("*NOP", 2,2, AddressingMode.Immediate)},
            {0x89,  new Instruction("*NOP", 2,2, AddressingMode.Immediate)},
            {0xc2,  new Instruction("*NOP", 2,2, AddressingMode.Immediate)},
            {0xe2,  new Instruction("*NOP", 2,2, AddressingMode.Immediate)},


            {0xCB,  new Instruction("*AXS", 2,2, AddressingMode.Immediate)},

            {0x6B,  new Instruction("*ARR", 2,2, AddressingMode.Immediate)},

            {0xeb,  new Instruction("*SBC", 2,2, AddressingMode.Immediate)},

            {0x0b,  new Instruction("*ANC", 2,2, AddressingMode.Immediate)},
            {0x2b,  new Instruction("*ANC", 2,2, AddressingMode.Immediate)},

            {0x4b,  new Instruction("*ALR", 2,2, AddressingMode.Immediate)},
            // new Instruction(0xCB, "IGN", 3,4 /* or 5*/, AddressingMode.Absolute_X)},

            {0x04,  new Instruction("*NOP", 2,3, AddressingMode.ZeroPage)},
            {0x44,  new Instruction("*NOP", 2,3, AddressingMode.ZeroPage)},
            {0x64,  new Instruction("*NOP", 2,3, AddressingMode.ZeroPage)},
            {0x14,  new Instruction("*NOP", 2, 4, AddressingMode.ZeroPage_X)},
            {0x34,  new Instruction("*NOP", 2, 4, AddressingMode.ZeroPage_X)},
            {0x54,  new Instruction("*NOP", 2, 4, AddressingMode.ZeroPage_X)},
            {0x74,  new Instruction("*NOP", 2, 4, AddressingMode.ZeroPage_X)},
            {0xd4,  new Instruction("*NOP", 2, 4, AddressingMode.ZeroPage_X)},
            {0xf4,  new Instruction("*NOP", 2, 4, AddressingMode.ZeroPage_X)},
            {0x0c,  new Instruction("*NOP", 3, 4, AddressingMode.Absolute)},
            {0x1c,  new Instruction("*NOP", 3, 4 /*or 5*/, AddressingMode.Absolute_X)},
            {0x3c,  new Instruction("*NOP", 3, 4 /*or 5*/, AddressingMode.Absolute_X)},
            {0x5c,  new Instruction("*NOP", 3, 4 /*or 5*/, AddressingMode.Absolute_X)},
            {0x7c,  new Instruction("*NOP", 3, 4 /*or 5*/, AddressingMode.Absolute_X)},
            {0xdc,  new Instruction("*NOP", 3, 4 /* or 5*/, AddressingMode.Absolute_X)},
            {0xfc,  new Instruction("*NOP", 3, 4 /* or 5*/, AddressingMode.Absolute_X)},

            {0x67,  new Instruction("*RRA", 2, 5, AddressingMode.ZeroPage)},
            {0x77,  new Instruction("*RRA", 2, 6, AddressingMode.ZeroPage_X)},
            {0x6f,  new Instruction("*RRA", 3, 6, AddressingMode.Absolute)},
            {0x7f,  new Instruction("*RRA", 3, 7, AddressingMode.Absolute_X)},
            {0x7b,  new Instruction("*RRA", 3, 7, AddressingMode.Absolute_Y)},
            {0x63,  new Instruction("*RRA", 2, 8, AddressingMode.Indirect_X)},
            {0x73,  new Instruction("*RRA", 2, 8, AddressingMode.Indirect_Y)},


            {0xe7,  new Instruction("*ISB", 2,5, AddressingMode.ZeroPage)},
            {0xf7,  new Instruction("*ISB", 2,6, AddressingMode.ZeroPage_X)},
            {0xef,  new Instruction("*ISB", 3,6, AddressingMode.Absolute)},
            {0xff,  new Instruction("*ISB", 3,7, AddressingMode.Absolute_X)},
            {0xfb,  new Instruction("*ISB", 3,7, AddressingMode.Absolute_Y)},
            {0xe3,  new Instruction("*ISB", 2,8, AddressingMode.Indirect_X)},
            {0xf3,  new Instruction("*ISB", 2,8, AddressingMode.Indirect_Y)},

            {0x02,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0x12,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0x22,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0x32,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0x42,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0x52,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0x62,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0x72,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0x92,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0xb2,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0xd2,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0xf2,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},

            {0x1a,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0x3a,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0x5a,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0x7a,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0xda,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            //{0xea,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},
            {0xfa,  new Instruction("*NOP", 1,2, AddressingMode.Implicit)},

            //{0xab,  new Instruction("*LXA", 2, 3, AddressingMode.Immediate)}, //todo: highly unstable and not used
            //http://visual6502.org/wiki/index.php?title=6502_Instruction_8B_%28XAA,_ANE%29
            //{0x8b,  new Instruction("*XAA", 2, 3, AddressingMode.Immediate)}, //todo: highly unstable and not used
            //{0xbb,  new Instruction("*LAS", 3, 2, AddressingMode.Absolute_Y)}, //todo: highly unstable and not used
            //{0x9b,  new Instruction("*TAS", 3, 2, AddressingMode.Absolute_Y)}, //todo: highly unstable and not used
            //{0x93,  new Instruction("*AHX", 2, /* guess */ 8, AddressingMode.Indirect_Y)}, //todo: highly unstable and not used
            //{0x9f,  new Instruction("*AHX", 3, /* guess */ 4/* or 5*/, AddressingMode.Absolute_Y)}, //todo: highly unstable and not used
            {0x9e,  new Instruction("*SHX", 3, /* guess */ 4/* or 5*/, AddressingMode.Absolute_Y)}, //todo: highly unstable and not used
            {0x9c,  new Instruction("*SHY", 3, /* guess */ 4/* or 5*/, AddressingMode.Absolute_X)}, //todo: highly unstable and not used

            {0xa7,  new Instruction("*LAX", 2, 3, AddressingMode.ZeroPage)},
            {0xb7,  new Instruction("*LAX", 2, 4, AddressingMode.ZeroPage_Y)},
            {0xaf,  new Instruction("*LAX", 3, 4, AddressingMode.Absolute)},
            {0xbf,  new Instruction("*LAX", 3, 4, AddressingMode.Absolute_Y)},
            {0xa3,  new Instruction("*LAX", 2, 6, AddressingMode.Indirect_X)},
            {0xb3,  new Instruction("*LAX", 2, 5, AddressingMode.Indirect_Y)},

            {0x87,  new Instruction("*SAX", 2, 3, AddressingMode.ZeroPage)},
            {0x97,  new Instruction("*SAX", 2, 4, AddressingMode.ZeroPage_Y)},
            {0x8f,  new Instruction("*SAX", 3, 4, AddressingMode.Absolute)},
            {0x83,  new Instruction("*SAX", 2, 6, AddressingMode.Indirect_X)},
        };
}