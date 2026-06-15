namespace NESharpLib.Modules.CPU;

public enum InterruptType : byte
{
    NMI,
    IRQ,
    BRK,
    RESET,
}

public static class InterruptTypeExt
{
    public static Interrupt GetInterrupt(this InterruptType a)
    {
        switch (a)
        {
            case InterruptType.NMI:
                return Interrupt.NMI();
            case InterruptType.IRQ:
                return Interrupt.IRQ();
            case InterruptType.BRK:
                return Interrupt.BRK();
            case InterruptType.RESET:
                return Interrupt.RESET();
            default:
                throw new Exception("Invalid interrupt type.");
        }
    }
}

public record struct Interrupt
{
    public InterruptType Type { get; init; }
    public ushort Vector { get; init; }
    public static Interrupt NMI()
    {
        return new Interrupt { Type = InterruptType.NMI, Vector = 0xfffa };
    }
    public static Interrupt IRQ()
    {
        return new Interrupt { Type = InterruptType.IRQ, Vector = 0xfffe };
    }
    public static Interrupt BRK()
    {
        return new Interrupt { Type = InterruptType.BRK, Vector = 0xfffe };
    }
    public static Interrupt RESET()
    {
        return new Interrupt { Type = InterruptType.RESET, Vector = 0xfffc };
    }
}

public partial class CPU
{
    private Interrupt? interrupt = null;
    private void Interrupt(InterruptType i)
    {
        interrupt = i.GetInterrupt();
    }
    private void HandleInterrupt()
    {
        if (interrupt.HasValue)
        {
            Interrupt i = interrupt.GetValueOrDefault();
            if (i.Type != InterruptType.RESET)
                StackPushShort(ProgramCounter);
            CPUStatus status = Status;
            if (i.Type == InterruptType.BRK)
            {
                status |= CPUStatus.Break;
            }
            else
            {
                status &= ~CPUStatus.Break;
            }
            if (i.Type != InterruptType.RESET)
                StackPush((byte)status);
            else
                SP = SP.WrappingSub(3); // RESET doesn't write to the stack, but DOES decrement the stack pointer.
            ProgramCounter = bus.ReadShort(i.Vector);
            Status.SetInterruptDisable(true);
        }
    }
}