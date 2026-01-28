using JollySidEmu.Audio;

namespace JollySidEmu.ViewModels
{
    public class VoiceViewModel : ViewModelBase
    {
        private readonly C64Memory _memory;
        private readonly int _baseAddress;
        public AdsrViewModel Adsr { get; }

        public VoiceViewModel(C64Memory memory, int baseAddress)
        {
            _memory = memory;
            _baseAddress = baseAddress;
            Frequency = 0;
            PulseWidth = 0;
            Adsr = new AdsrViewModel(memory, baseAddress);
        }

        // ---------------- Frequency ----------------

        private int _frequency;
        public int Frequency
        {
            get => _frequency;
            set
            {
                _frequency = value;
                OnPropertyChanged();
                WriteFrequency();
            }
        }

        private void WriteFrequency()
        {
            _memory.Poke(_baseAddress + 0, (byte)(_frequency & 0xFF));
            _memory.Poke(_baseAddress + 1, (byte)((_frequency >> 8) & 0xFF));
        }

        // ---------------- Pulse Width ----------------

        private double _pulseWidth;
        public double PulseWidth
        {
            get => _pulseWidth;
            set
            {
                _pulseWidth = value;
                OnPropertyChanged();
                WritePulseWidth();
            }
        }

        private void WritePulseWidth()
        {
            int pw = (int)(_pulseWidth * 4095);
            _memory.Poke(_baseAddress + 2, (byte)(pw & 0xFF));
            _memory.Poke(_baseAddress + 3, (byte)((pw >> 8) & 0x0F));
        }

        // ---------------- Waveform + Gate ----------------

        private SidWaveform _waveform = SidWaveform.Saw;
        public SidWaveform Waveform
        {
            get => _waveform;
            set
            {
                _waveform = value;
                OnPropertyChanged();
                WriteControl();
            }
        }

        private bool _gate;
        public bool Gate
        {
            get => _gate;
            set
            {
                _gate = value;
                OnPropertyChanged();
                WriteControl();
            }
        }

        private bool _syncEnabled;
        public bool SyncEnabled
        {
            get => _syncEnabled;
            set
            {
                if (_syncEnabled == value) 
                    return;
                _syncEnabled = value;
                OnPropertyChanged();
                WriteControl();
            }
        }

        private bool _ringModEnabled;
        public bool RingModEnabled
        {
            get => _ringModEnabled;
            set
            {
                if (_ringModEnabled == value) 
                    return;
                _ringModEnabled = value;
                OnPropertyChanged();
                WriteControl();
            }
        }

        private void WriteControl()
        {
            byte ctrl = _waveform switch
            {
                SidWaveform.Triangle => 0x10,
                SidWaveform.Saw => 0x20,
                SidWaveform.Pulse => 0x40,
                SidWaveform.Noise => 0x80,
                _ => 0x20
            };

            if (_gate)
                ctrl |= 0x01;

            if (_syncEnabled)
                ctrl |= 0x02;

            if (_ringModEnabled)
                ctrl |= 0x04;

            _memory.Poke(_baseAddress + 4, ctrl);
        }
    }
}
