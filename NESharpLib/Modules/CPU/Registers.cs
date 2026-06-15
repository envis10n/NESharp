namespace NESharpLib.Modules.CPU;
using System;

/// <summary>
/// CPU status flags
/// </summary>
[Flags]
public enum CPUStatus : byte
{
    Empty = 0b0,
    Carry = 0b1,
    Zero = 0b10,
    InterruptDisable = 0b100,
    Decimal = 0b1000,
    Break = 0b10000,
    B2 = 0b100000,
    Overflow = 0b1000000,
    Negative = 0b10000000,
}

public static class CpuStatusExt
{
    public static void SetFlagState(this ref CPUStatus status, bool state, CPUStatus flag)
    {
        if (state) status |= flag;
        else status &= ~flag;
    }
    public static void SetZero(this ref CPUStatus status, bool state)
    {
        status.SetFlagState(state, CPUStatus.Zero);
    }
    public static void SetNegative(this ref CPUStatus status, bool state)
    {
        status.SetFlagState(state, CPUStatus.Negative);
    }
    public static void SetCarry(this ref CPUStatus status, bool state)
    {
        status.SetFlagState(state, CPUStatus.Carry);
    }
    public static void SetInterruptDisable(this ref CPUStatus status, bool state)
    {
        status.SetFlagState(state, CPUStatus.InterruptDisable);
    }
    public static void SetOverflow(this ref CPUStatus status, bool state)
    {
        status.SetFlagState(state, CPUStatus.Overflow);
    }
}
