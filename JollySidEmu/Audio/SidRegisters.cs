namespace JollySidEmu.Audio
{
    public class SidRegisters
    {
        public byte[] Registers { get; private set; } = new byte[29];

        public byte this[int index]
        {
            get => Registers[index];
            set
            {
                Registers[index] = value;
                RegisterChanged?.Invoke(index, value);
            }
        }

        public event Action<int, byte>? RegisterChanged;
    }
}
