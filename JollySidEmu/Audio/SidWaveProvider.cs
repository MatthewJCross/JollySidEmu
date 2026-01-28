using NAudio.Wave;

namespace JollySidEmu.Audio
{
    public class SidWaveProvider : WaveProvider32
    {
        private SidRegisters _sid;
        private SidVoice[] _voices = new SidVoice[3];
        private double _sampleRate;
        private readonly SidFilter _filter;
        private readonly SidMixer _mixer;

        public SidFilter Filter => _filter;
        public SidMixer Mixer => _mixer;
        public SidVoice[] Voices => _voices;

        public SidWaveProvider(SidRegisters sid)
        {
            _sid = sid;
            _sampleRate = 44100.0;
            _sid.RegisterChanged += Sid_RegisterChanged;
            for (int i = 0; i < 3; i++) 
                _voices[i] = new SidVoice(_sampleRate);

            _voices[0].PreviousVoice = null;        // Voice 1 has no previous
            _voices[1].PreviousVoice = _voices[0];  // Voice 2 syncs/ring with Voice 1
            _voices[2].PreviousVoice = _voices[1];  // Voice 3 syncs/ring with Voice 2

            _filter = new SidFilter(_sampleRate);
            _mixer = new SidMixer();
        }

        private void Sid_RegisterChanged(int index, byte value)
        {
            // --- Filter registers ---
            switch (index)
            {
                case 0x15: // Cutoff low
                    _filter.SetCutoffLow(value);
                    break;
                case 0x16: // Cutoff high
                    _filter.SetCutoffHigh(value);
                    break;
                case 0x17: // Resonance + routing
                    _filter.SetResonanceRouting(value);
                    break;
                case 0x18: // Mode + volume
                    _filter.SetModeVolume(value);
                    break;
            }

            int voiceNum = index / 7;
            if (voiceNum > 2) 
                return;

            var voice = _voices[voiceNum];
            int baseIdx = voiceNum * 7;

            int freq = (_sid[baseIdx + 1] << 8) | _sid[baseIdx];
            voice.SetFrequency((ushort)(freq * 0.1));

            int pw = (_sid[baseIdx + 3] << 8) | _sid[baseIdx + 2];
            voice.SetPulseWidth((ushort)Math.Clamp(pw & 0x0FFF, 1, 4095));

            byte ctrl = _sid[baseIdx + 4];
            if ((ctrl & 0x10) != 0) 
                voice.SetWaveform(SidWaveform.Triangle);
            else if ((ctrl & 0x20) != 0) 
                voice.SetWaveform(SidWaveform.Saw);
            else if ((ctrl & 0x40) != 0) 
                voice.SetWaveform(SidWaveform.Pulse);
            else if ((ctrl & 0x80) != 0) 
                voice.SetWaveform(SidWaveform.Noise);

            voice.SetGate((ctrl & 0x01) != 0);

            voice.SetAdsr(_sid[baseIdx + 5] >> 4 , _sid[baseIdx + 5] & 0x0F, _sid[baseIdx + 6] >> 4, _sid[baseIdx + 6] & 0x0F);
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            float masterGain = (_sid[0x18] & 0x0F) / 15f; // normalize 0.0..1.0
            double v1 = 0.0;
            double v2 = 0.0;
            double v3 = 0.0;
            double filtered = 0.0;
            double dry = 0.0;
            double output = 0.0;

            for (int n = 0; n < sampleCount; n++)
            {
                v1 = _voices[0].RenderSample();
                v2 = _voices[1].RenderSample();
                v3 = _voices[2].RenderSample();

                // Route voices through filter
                dry = 0.0;
                filtered = 0.0;

                if (_filter.Voice1Enabled) 
                    filtered += v1; 
                else 
                    dry += v1;

                if (_filter.Voice2Enabled) 
                    filtered += v2; 
                else 
                    dry += v2;

                if (_filter.Voice3Enabled) 
                    filtered += v3; 
                else 
                    dry += v3;

                filtered = _filter.Process(filtered);

                output = filtered + dry;
                buffer[n] = (float)(output * masterGain);

                _mixer.Push((float)v1, (float)v2, (float)v3, (float)output);
            }

            return sampleCount;
        }
    }
}
