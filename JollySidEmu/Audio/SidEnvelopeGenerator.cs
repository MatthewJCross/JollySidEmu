namespace JollySidEmu.Audio
{
    public enum SidEnvelopeState
    {
        Idle,
        Attack,
        Decay,
        Sustain,
        Release
    }

    public sealed class SidEnvelopeGenerator
    {
        private static readonly double[] AttackTimesMs =
        {
            2, 8, 16, 24, 38, 56, 68, 80,
            100, 250, 500, 800, 1000, 3000, 5000, 8000
        };

        private static readonly double[] DecayReleaseTimesMs =
        {
            6, 24, 48, 72, 114, 168, 204, 240,
            300, 750, 1500, 2400, 3000, 9000, 15000, 24000
        };

        private readonly double _sampleRate;

        private SidEnvelopeState _state = SidEnvelopeState.Idle;
        private double _envelope; // 0.0 – 1.0

        private double _ratePerSample;
        private double _target;

        public SidEnvelopeGenerator(double sampleRate)
        {
            _sampleRate = sampleRate;
        }

        public double Output => _envelope;

        // -------- SID register inputs --------
        public int Attack { get; set; }
        public int Decay { get; set; }
        public int Sustain { get; set; }
        public int Release { get; set; }

        // -------- Gate control --------
        public void GateOn()
        {
            _state = SidEnvelopeState.Attack;
            StartAttack();
        }

        public void GateOff()
        {
            _state = SidEnvelopeState.Release;
            StartRelease();
        }

        // -------- Processing --------
        public void Clock()
        {
            if (_state == SidEnvelopeState.Idle)
                return;

            _envelope += _ratePerSample;

            if ((_ratePerSample > 0 && _envelope >= _target) || (_ratePerSample < 0 && _envelope <= _target))
            {
                _envelope = _target;
                AdvanceState();
            }
        }

        // -------- State transitions --------
        private void AdvanceState()
        {
            switch (_state)
            {
                case SidEnvelopeState.Attack:
                    _state = SidEnvelopeState.Decay;
                    StartDecay();
                    break;

                case SidEnvelopeState.Decay:
                    _state = SidEnvelopeState.Sustain;
                    break;

                case SidEnvelopeState.Release:
                    _state = SidEnvelopeState.Idle;
                    break;
            }
        }

        private void StartAttack()
        {
            _target = 1.0;
            _ratePerSample = 1.0 / (AttackTimesMs[Attack] * _sampleRate / 1000.0);
        }

        private void StartDecay()
        {
            _target = Sustain / 15.0;
            _ratePerSample = -(_envelope - _target) / (DecayReleaseTimesMs[Decay] * _sampleRate / 1000.0);
        }

        private void StartRelease()
        {
            _target = 0.0;
            _ratePerSample = -_envelope / (DecayReleaseTimesMs[Release] * _sampleRate / 1000.0);
        }
    }
}
