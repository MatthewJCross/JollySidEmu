namespace JollySidEmu.Audio
{
    public class C64Memory
    {
        private SidRegisters _sid;
        public C64Memory(SidRegisters sid) => _sid = sid;

        public void Poke(int addr, byte value)
        {
            if (addr >= 0xD400 && addr <= 0xD41C)
                _sid[addr - 0xD400] = value;
        }

        public byte Peek(int addr)
        {
            if (addr >= 0xD400 && addr <= 0xD41C)
                return _sid[addr - 0xD400];
            return 0xFF;
        }
    }
}
