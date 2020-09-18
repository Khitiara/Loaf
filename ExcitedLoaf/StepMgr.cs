using System;
using System.Runtime.CompilerServices;

namespace ExcitedLoaf
{
    public class StepMgr
    {
        private static readonly ControlFlags[,,] Microcode = new ControlFlags[255, 8, 16];

        private static bool Load(RegisterStore registers, Addressing addressing, out ControlFlags flags,
            out ushort bus) {
            flags = Microcode[registers.InstructionRegister, registers.StepCounter, (int) registers.Flags];
            bus = 0;
            if (flags.Has(ControlFlags.Hlt)) return true;
            LoadBusRegisters(registers, flags, ref bus);
            if (flags.Has(ControlFlags.LdMem)) addressing.Read(registers.RegAddr, ref bus);
            LoadBusAlu(registers, flags.AluFlags(), ref bus);
            // at this point the bus is fully loaded
            return false;
        }

        private static void Store(RegisterStore registers, Addressing addressing, in ControlFlags flags,
            in ushort bus) {
            if (flags.Has(ControlFlags.StepReset))
                registers.StepCounter = 0;
            else {
                registers.StepCounter += 1;
                registers.StepCounter %= 8;
            }

            if (flags.Has(ControlFlags.PCInc)) registers.RegProgCounter++;

            if (flags.Has(ControlFlags.Jump)) registers.RegProgCounter = bus;

            if (flags.Has(ControlFlags.StoMem)) addressing.Write(registers.RegAddr, bus);

            StoreBusRegisters(registers, flags, bus);
        }

        public static bool Clock(RegisterStore registers, Addressing addressing, ref ControlFlags flags, ref ushort bus,
            bool init = false) {
            if (!init)
                Store(registers, addressing, flags, bus);
            return Load(registers, addressing, out flags, out bus);
        }

        private static void StoreBusRegisters(RegisterStore registers, ControlFlags flags, in ushort bus) {
            if (flags.Has(ControlFlags.ARegIn))
                registers.RegA = bus;
            if (flags.Has(ControlFlags.BRegIn))
                registers.RegB = bus;
            if (flags.Has(ControlFlags.CRegIn))
                registers.RegC = bus;
            if (flags.Has(ControlFlags.DRegIn))
                registers.RegD = bus;
            if (flags.Has(ControlFlags.XRegIn))
                registers.RegX = bus;
            if (flags.Has(ControlFlags.YRegIn))
                registers.RegY = bus;
            if (flags.Has(ControlFlags.InstRegIn))
                registers.InstructionRegister = bus;
            if (flags.Has(ControlFlags.AddrIn))
                registers.RegAddr = bus;
        }

        private static void LoadBusAlu(RegisterStore registers, AluOp aluOp, ref ushort bus) {
            ushort x = registers.RegX,
                y = registers.RegY;

            switch (aluOp) {
                case AluOp.None:
                    break;
                case AluOp.Add:
                    bus |= (ushort) (x + y);
                    break;
                case AluOp.Sub:
                    bus |= (ushort) (x - y);
                    break;
                case AluOp.Mul:
                    bus |= (ushort) (x * y);
                    break;
                case AluOp.Div:
                    bus |= (ushort) (x / y);
                    break;
                case AluOp.And:
                    bus |= (ushort) (x & y);
                    break;
                case AluOp.Or:
                    bus |= (ushort) (x | y);
                    break;
                case AluOp.Xor:
                    bus |= (ushort) (x ^ y);
                    break;
                case AluOp.Not:
                    bus |= (ushort) ~x;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(aluOp), aluOp, null);
            }
        }

        private static void LoadBusRegisters(RegisterStore registers, ControlFlags flags,
            ref ushort bus) {
            if (flags.Has(ControlFlags.ARegOut)) bus |= registers.RegA;

            if (flags.Has(ControlFlags.BRegOut)) bus |= registers.RegB;

            if (flags.Has(ControlFlags.CRegOut)) bus |= registers.RegC;

            if (flags.Has(ControlFlags.DRegOut)) bus |= registers.RegD;

            if (flags.Has(ControlFlags.XRegOut)) bus |= registers.RegX;

            if (flags.Has(ControlFlags.YRegOut)) bus |= registers.RegY;

            if (flags.Has(ControlFlags.PCRegOut)) bus |= registers.RegProgCounter;

            if (flags.Has(ControlFlags.InstRegOut)) bus |= registers.InstructionRegister;

            if (flags.Has(ControlFlags.FlagsRegOut)) bus |= (ushort) registers.Flags;

            if (flags.Has(ControlFlags.AddrOut)) bus |= registers.RegAddr;
        }
    }

    public static class Extensions
    {
        public static AluOp AluFlags(this ControlFlags control) {
            if (!control.Has(ControlFlags.AluEnable)) return 0;
            uint flags = (uint) control >> 19;
            return (AluOp) (flags & 0xF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has(this ControlFlags self, ControlFlags flag) {
            return (self & flag) != 0;
        }
    }
}