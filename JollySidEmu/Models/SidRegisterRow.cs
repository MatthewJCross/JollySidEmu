using JollySidEmu.ViewModels;

namespace JollySidEmu.Models
{
    public class SidRegisterRow : ViewModelBase
    {
        public int Address { get; }
        public string Name { get; }

        private byte _value;
        public byte Value
        {
            get => _value;
            set
            {
                if (_value == value) 
                    return;
                _value = value;
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(Hex));
                OnPropertyChanged(nameof(Decimal));
                OnPropertyChanged(nameof(Binary));
                OnPropertyChanged(nameof(Decoded));
            }
        }

        public string Hex => $"0x{Value:X2}";
        public int Decimal => Value;
        public string Binary => Convert.ToString(Value, 2).PadLeft(8, '0');
        public string Decoded => SidRegisterDecoder.Decode(Address, Value);

        public SidRegisterRow(int address, string name, byte initial = 0)
        {
            Address = address;
            Name = name;
            _value = initial;
        }
    }
}
