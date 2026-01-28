using JollySidEmu.Audio;

namespace JollySidEmu.ViewModels
{
    public class FilterViewModel : ViewModelBase
    {
        private readonly C64Memory _memory;
        private readonly int _baseAddress;

        public FilterViewModel(C64Memory memory, int baseAddress)
        {
            _memory = memory;   
            _baseAddress = baseAddress;
            FilterModes = Enum.GetValues(typeof(SidFilterMode)).Cast<SidFilterMode>().ToList();
            Volume = 15;
            Resonance = 0;
            CutoffLow = 0;
            CutoffHigh = 0;
        }

        public IReadOnlyList<SidFilterMode> FilterModes { get; }

        private SidFilterMode _filterMode;
        public SidFilterMode FilterMode
        {
            get => _filterMode;
            set
            {
                if (_filterMode == value) 
                    return;
                _filterMode = value;
                OnPropertyChanged();
                WriteD418();
            }
        }

        private int _volume; // 0–15
        public int Volume
        {
            get => _volume;
            set
            {
                value = Math.Clamp(value, 0, 15);
                if (_volume == value) 
                    return;
                _volume = value;
                OnPropertyChanged();
                WriteD418();
            }
        }

        private bool _v1;
        public bool Voice1Enabled
        {
            get => _v1;
            set
            {
                if (_v1 == value) 
                    return;
                _v1 = value;
                OnPropertyChanged();
                WriteD417();
            }
        }

        private bool _v2;
        public bool Voice2Enabled
        {
            get => _v2;
            set
            {
                if (_v2 == value) 
                    return;
                _v2 = value;
                OnPropertyChanged();
                WriteD417();
            }
        }

        private bool _v3;
        public bool Voice3Enabled
        {
            get => _v3;
            set
            {
                if (_v3 == value) 
                    return;
                _v3 = value;
                OnPropertyChanged();
                WriteD417();
            }
        }

        private int _resonance;
        public int Resonance
        {
            get => _resonance;
            set
            {
                value = Math.Clamp(value, 0, 15);
                if (_resonance == value) 
                    return;
                _resonance = value;
                OnPropertyChanged();
                WriteD417();
            }
        }

        private double _cutoffLow;
        public double CutoffLow
        {
            get => _cutoffLow;
            set
            {
                if (_cutoffLow == value) return;
                _cutoffLow = value;
                OnPropertyChanged();

                _memory.Poke(0xD415, (byte)(int)value);
            }
        }

        private double _cutoffHigh;
        public double CutoffHigh
        {
            get => _cutoffHigh;
            set
            {
                if (_cutoffHigh == value) 
                    return;
                _cutoffHigh = value;
                OnPropertyChanged();

                _memory.Poke(0xD415, (byte)(int)value);
            }
        }

        private void WriteD418()
        {
            byte d418 =
                (byte)(((int)_filterMode & 0x70) |   // bits 6–4
                       (_volume & 0x0F));            // bits 3–0

            _memory.Poke(0xD418, d418);
        }

        private void WriteD417()
        {
            byte d417 =
                (byte)((_resonance << 4) |
                       (_v3 ? 0x04 : 0) |
                       (_v2 ? 0x02 : 0) |
                       (_v1 ? 0x01 : 0));

            _memory.Poke(0xD417, d417);
        }
    }
}
