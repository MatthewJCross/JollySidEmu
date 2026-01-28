namespace JollySidEmu.Audio
{
    public sealed class SidVoice
    {
        private readonly double _sampleRate;
        private readonly SidEnvelopeGenerator _envelope;
        private SidNoiseOscillator _noiseOsc = new SidNoiseOscillator();

        public bool SyncEnabled { get; private set; }
        public bool RingModEnabled { get; private set; }

        private double _phase;
        private double _phaseIncrement;

        public SidVoice? PreviousVoice { get; set; } // reference to previous voice for sync/ring
        public double CurrentSample { get; private set; }
        public bool PhaseWrapped { get; private set; }

        private ushort _frequency;
        private ushort _pulseWidth;
        private SidWaveform _waveform;

        private bool _gate;

        public void SetSync(bool enabled) => SyncEnabled = enabled;
        public void SetRingMod(bool enabled) => RingModEnabled = enabled;

        public double Output => _envelope.Output;

        public SidVoice(double sampleRate)
        {
            _sampleRate = sampleRate;
            _envelope = new SidEnvelopeGenerator(sampleRate);
        }

        public void SetFrequency(ushort value)
        {
            _frequency = value;
            _phaseIncrement = _frequency / _sampleRate;
            _noiseOsc.SetFrequency(_frequency, _sampleRate);
        }

        public void SetPulseWidth(ushort value)
        {
            _pulseWidth = (ushort)(value & 0x0FFF); // 12-bit
        }

        public void SetWaveform(SidWaveform waveform)
        {
            _waveform = waveform;
        }

        public void SetGate(bool gate)
        {
            if (_gate == gate)
                return;

            _gate = gate;

            if (_gate)
                _envelope.GateOn();
            else
                _envelope.GateOff();
        }

        public void SetAdsr(int attack, int decay, int sustain, int release)
        {
            _envelope.Attack = attack;
            _envelope.Decay = decay;
            _envelope.Sustain = sustain;
            _envelope.Release = release;
        }

        public float RenderSample()
        {
            _envelope.Clock();

            double sample = GenerateWaveform();
            sample *= _envelope.Output;

            // Apply sync
            if (SyncEnabled && PreviousVoice != null && PreviousVoice.PhaseWrapped)
                _phase = 0;

            // Apply ring modulation
            if (RingModEnabled && PreviousVoice != null)
                sample *= PreviousVoice.CurrentSample;

            AdvancePhase();

            CurrentSample = sample;
            PhaseWrapped = _phase < (_frequency / _sampleRate);

            return (float)sample;
        }

        private void AdvancePhase()
        {
            _phase += _phaseIncrement;
            if (_phase >= 1.0)
                _phase -= 1.0;
        }

        private double GenerateWaveform()
        {
            switch (_waveform)
            {
                case SidWaveform.Triangle:
                    return RenderTriangle();

                case SidWaveform.Saw:
                    return RenderSaw();

                case SidWaveform.Pulse:
                    return RenderPulse();

                case SidWaveform.Noise:
                    return _noiseOsc.RenderSample();

                case SidWaveform.None:
                default:
                    return 0.0;
            }
        }

        private double RenderTriangle()
        {
            return _phase < 0.5f ? (_phase * 4f - 1f) : (3f - _phase * 4f);
        }

        private double RenderSaw()
        {
            return _phase * 2f - 1f;
        }        

        private double RenderPulse()
        {
            double phase = _phase; // 0..1
            double threshold = _pulseWidth / 4096f;
            return phase < threshold ? 1f : -1f;
        }
    }
}
