using NAudio.Wave;
using JollySidEmu.Audio;

namespace JollySidEmu.ViewModels
{
    public class SidViewModel : ViewModelBase
    {
        private readonly C64Memory _memory;

        private WaveOutEvent? _audioOut = null;

        public SidWaveProvider Provider { get; }
        public VoiceViewModel Voice1 { get; }
        public VoiceViewModel Voice2 { get; }
        public VoiceViewModel Voice3 { get; }
        public FilterViewModel Filter { get; }
        public SidRegisterInspectorViewModel RegisterInspector { get; }

        public OscilloscopeViewModel Oscilloscope { get; }

        private bool _isMenuVisible;
        public bool IsMenuVisible
        {
            get => _isMenuVisible; 
            set => _isMenuVisible = value;
        }

        private bool _iconClickable;
        public bool IconClickable
        {
            get => _iconClickable;
            set => _iconClickable = value;
        }

        public SidViewModel()
        {
            var sid = new SidRegisters();
            _memory = new C64Memory(sid);
             
            Provider = new SidWaveProvider(sid);
            _audioOut = new WaveOutEvent();
            _audioOut.Init(Provider);
            _audioOut.Play();

            Voice1 = new VoiceViewModel(_memory, 0xD400);
            Voice2 = new VoiceViewModel(_memory, 0xD407);
            Voice3 = new VoiceViewModel(_memory, 0xD40E);

            Voice1.Frequency = 328;
            Voice1.Adsr.Sustain = 3;
            Voice1.Waveform = SidWaveform.Saw;
            Voice2.Frequency = 656;
            Voice2.Adsr.Sustain = 4;
            Voice2.Waveform = SidWaveform.Triangle;
            Voice3.Frequency = 984;
            Voice3.Adsr.Sustain = 5;
            Voice3.Waveform = SidWaveform.Noise;

            Filter = new FilterViewModel(_memory, 0xD400);
            RegisterInspector = new SidRegisterInspectorViewModel(sid);
            Oscilloscope = new OscilloscopeViewModel(this);

            IsMenuVisible = false;
            IconClickable = false;
        }

        public void Dispose()
        {
            _audioOut?.Stop();
            _audioOut?.Dispose();
        }
    }
}
