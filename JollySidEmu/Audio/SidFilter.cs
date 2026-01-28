namespace JollySidEmu.Audio
{
    [Flags]
    public enum SidFilterMode
    {
        None = 0x00,
        LowPass = 0x10,
        BandPass = 0x20,
        HighPass = 0x40
    }

    public sealed class SidFilter
    {
        private readonly double _sampleRate;

        // Filter state
        private double _low;
        private double _band;

        // Registers
        private int _cutoff;
        private int _resonance;
        private SidFilterMode _mode;

        // Voice routing
        public bool Voice1Enabled { get; set; }
        public bool Voice2Enabled { get; set; }
        public bool Voice3Enabled { get; set; }

        public SidFilter(double sampleRate)
        {
            _sampleRate = sampleRate;
        }

        public void SetCutoffLow(byte value)
        {
            _cutoff = (_cutoff & 0x7F8) | (value & 0x07);
            
        }

        public void SetCutoffHigh(byte value)
        {
            _cutoff = ((value & 0xFF) << 3) | (_cutoff & 0x07);
        }

        public void SetResonanceRouting(byte value)
        {
            _resonance = (value >> 4) & 0x0F;

            Voice1Enabled = (value & 0x01) != 0;
            Voice2Enabled = (value & 0x02) != 0;
            Voice3Enabled = (value & 0x04) != 0;
        }

        public void SetModeVolume(byte value)
        {
            _mode = SidFilterMode.None;

            if ((value & 0x08) != 0) _mode |= SidFilterMode.LowPass;
            if ((value & 0x10) != 0) _mode |= SidFilterMode.BandPass;
            if ((value & 0x20) != 0) _mode |= SidFilterMode.HighPass;
        }

        public double Process(double input)
        {
            if (_mode == SidFilterMode.None)
                return input;

            double cutoffHz = CutoffToHz(_cutoff);
            double f = 2.0 * Math.Sin(Math.PI * cutoffHz / _sampleRate);

            double q = 1.0 + (_resonance / 15.0) * 4.0;

            _low += f * _band;
            double high = input - _low - q * _band;
            _band += f * high;

            double output = 0.0;

            if ((_mode & SidFilterMode.LowPass) != 0)
                output += _low;

            if ((_mode & SidFilterMode.BandPass) != 0)
                output += _band;

            if ((_mode & SidFilterMode.HighPass) != 0)
                output += high;

            return output;
        }

        private static double CutoffToHz(int cutoff)
        {
            // SID cutoff is VERY non-linear
            double normalized = cutoff / 2047.0;
            return 30.0 + Math.Pow(normalized, 2.2) * 12000.0;
        }
    }
}
