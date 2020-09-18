using System;

namespace ExcitedLoaf
{
    public class IOReadEventArgs
    {
        public ushort data;

        public IOReadEventArgs(ushort data) {
            this.data = data;
        }
    }

    public delegate void ReadAddressIO(ushort addr, IOReadEventArgs args);

    public delegate void WriteAddressIO(ushort addr, ushort b);

    public class Addressing
    {
        private Memory<ushort> _sram = new ushort[0x8000];
        private Memory<ushort> _prom = new ushort[0x7000];

        public event ReadAddressIO IORead;
        public event WriteAddressIO IOWrite;

        public void Read(ushort addr, ref ushort bus) {
            if (addr >= 0x9000) {
                // 0x9000 <= addr < 0xffff in PROM
                bus |= _prom.Span[addr - 0x9000];
            } else if (addr >= 0x8000) {
                // 0x8000 <= addr <= 0x8fff in IO Range 
                IOReadEventArgs args = new IOReadEventArgs(bus);
                IORead?.Invoke(addr, args);
                bus |= args.data;
            } else {
                // addr < 0x8000 in SRAM
                bus |= _sram.Span[addr];
            }
        }

        public void Write(ushort addr, ushort bus) {
            if (addr >= 0x9000) {
                // 0x9000 <= addr < 0xffff in PROM
                throw new InvalidOperationException("Cannot write to PROM");
            } else if (addr >= 0x8000) {
                // 0x8000 <= addr <= 0x8fff in IO Range
                IOWrite?.Invoke(addr, bus);
            } else {
                // addr < 0x8000 in SRAM
                _sram.Span[addr] = bus;
            }
        }

        public bool ProgramPROM(Span<ushort> image) {
            if (image.Length != _prom.Length) return false;
            image.CopyTo(_prom.Span);
            return true;
        }
    }
}