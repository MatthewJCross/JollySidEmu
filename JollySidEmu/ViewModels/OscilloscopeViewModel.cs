using JollySidEmu.Audio;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace JollySidEmu.ViewModels
{
    public sealed class OscilloscopeViewModel
    {
        private readonly SidRegisters? _sid;
        private readonly SidViewModel _svm;
        public float MasterVolume => _sid == null ? 0 : (_sid[0x18] & 0x0F) / 15.0f;
        
        public const int ScopeSize = 2048;

        public float[] V1 { get; } = new float[ScopeSize];
        public float[] V2 { get; } = new float[ScopeSize];
        public float[] V3 { get; } = new float[ScopeSize];
        public float[] Out { get; } = new float[ScopeSize];
        public double EstimatedFrequencyHz { get; private set; }

        private double _freqSmoothing = 0;

        private readonly object _lock = new();

        public OscilloscopeViewModel(SidViewModel svm)
        {
            _svm = svm;
        }

        public void Snapshot()
        {
            float master = MasterVolume; // 0..1 from $D418
            for (int i = 0; i < ScopeSize; i++)
            {
                V1[i] = _svm.Provider.Mixer.Voice1[i] * (float)_svm.Provider.Voices[0].Output * master;
                V2[i] = _svm.Provider.Mixer.Voice2[i] * (float)_svm.Provider.Voices[1].Output * master;
                V3[i] = _svm.Provider.Mixer.Voice3[i] * (float)_svm.Provider.Voices[2].Output * master;              
                Out[i] = Math.Clamp(V1[i] + V2[i] + V3[i], -1f, 1f);
            }

            lock (_lock)
            {
                _svm.Provider.Mixer.Snapshot(V1, V2, V3, Out);
            }

            EstimateFrequency(Out, 44100.0);
        }

        public void EstimateFrequency(float[] data, double sampleRate)
        {
            if (data.Length < 4)
                return;

            int crossings = 0;

            for (int i = 1; i < data.Length; i++)
            {
                if (data[i - 1] <= 0 && data[i] > 0)
                    crossings++;
            }

            double seconds = data.Length / sampleRate;
            double freq = crossings / seconds;

            _freqSmoothing = (_freqSmoothing * 0.85) + (freq * 0.15);
            EstimatedFrequencyHz = _freqSmoothing;
        }
    }
}
