namespace JollySidEmu.Audio
{
    public class SidNoiseOscillator
    {
        private uint _lfsr = 0x7FFFFF; // 23-bit seed, non-zero
        private double _phase;
        private double _phaseIncrement;

        // Call this every sample
        public void SetFrequency(ushort freq, double sampleRate)
        {
            // SID uses freq * 16 as approx cycle increment
            _phaseIncrement = freq * 16.0f / sampleRate;
        }

        public float RenderSample()
        {
            _phase += _phaseIncrement;

            // Clock LFSR once per oscillator cycle
            while (_phase >= 1.0f)
            {
                _phase -= 1.0f;
                ClockLFSR();
            }

            // Map MSB to [-1,1]
            return (_lfsr & 0x400000) != 0 ? 1f : -1f;
        }

        private void ClockLFSR()
        {
            // taps at bits 22 and 17
            bool bit22 = (_lfsr & 0x400000) != 0;
            bool bit17 = (_lfsr & 0x020000) != 0;
            bool newBit = bit22 ^ bit17;

            _lfsr = ((_lfsr << 1) & 0x7FFFFF) | (newBit ? 1u : 0u);
        }
    }
}
