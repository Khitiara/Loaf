using System;

namespace ExcitedLoaf
{
    [Flags]
    public enum FlagsRegister : byte
    {
        CarrySet = 1,
        ZeroSet  = 2,
    }

    [Flags]
    public enum ControlFlags : uint
    {
        StepReset   = 0b_00000000_00000000_00000000_00000001,
        ARegOut     = 0b_00000000_00000000_00000000_00000010,
        BRegOut     = 0b_00000000_00000000_00000000_00000100,
        CRegOut     = 0b_00000000_00000000_00000000_00001000,
        DRegOut     = 0b_00000000_00000000_00000000_00010000,
        XRegOut     = 0b_00000000_00000000_00000000_00100000,
        YRegOut     = 0b_00000000_00000000_00000000_01000000,
        PCRegOut    = 0b_00000000_00000000_00000000_10000000,
        InstRegOut  = 0b_00000000_00000000_00000001_00000000,
        FlagsRegOut = 0b_00000000_00000000_00000010_00000000,
        ARegIn      = 0b_00000000_00000000_00000100_00000000,
        BRegIn      = 0b_00000000_00000000_00001000_00000000,
        CRegIn      = 0b_00000000_00000000_00010000_00000000,
        DRegIn      = 0b_00000000_00000000_00100000_00000000,
        XRegIn      = 0b_00000000_00000000_01000000_00000000,
        YRegIn      = 0b_00000000_00000000_10000000_00000000,
        PCInc       = 0b_00000000_00000001_00000000_00000000,
        InstRegIn   = 0b_00000000_00000010_00000000_00000000,
        Jump        = 0b_00000000_00000100_00000000_00000000,

        // ALU operation select in 3 bits plus fourth bit for ALU enable
        AluEnable = 0b_00000000_00001000_00000000_00000000,
        AddrIn    = 0b_00000000_10000000_00000000_00000000,
        AddrOut   = 0b_00000001_00000000_00000000_00000000,
        LdMem     = 0b_00000010_00000000_00000000_00000000,
        StoMem    = 0b_00000100_00000000_00000000_00000000,
        Hlt       = 0b_10000000_00000000_00000000_00000000
    }

    public enum AluOp : byte
    {
        None = 0b0000,
        Add  = 0b0001,
        Sub  = 0b0011,
        Mul  = 0b0101,
        Div  = 0b0111,
        And  = 0b1101,
        Or   = 0b1011,
        Xor  = 0b1001,
        Not  = 0b1111
    }

    public enum RegisterSelect : byte
    {
        A           = 0b0000,
        B           = 0b0001,
        C           = 0b0010,
        D           = 0b0011,
        X           = 0b0100,
        Y           = 0b0101,
        PC          = 0b1000,
        Instruction = 0b1001,
        Flags       = 0b1010,
    }

    public class RegisterStore
    {
        public ushort RegA;
        public ushort RegB;
        public ushort RegC;
        public ushort RegD;

        public ushort RegX;
        public ushort RegY;

        public ushort RegAddr;

        public ushort RegProgCounter;
        public ushort InstructionRegister;
        public byte   StepCounter;

        public FlagsRegister Flags;
    }
}