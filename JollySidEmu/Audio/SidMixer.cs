namespace JollySidEmu.Audio
{
    public sealed class SidMixer
    {
        private const int BufferSize = 2048;

        public float[] Voice1 = new float[BufferSize];
        public float[] Voice2 = new float[BufferSize];
        public float[] Voice3 = new float[BufferSize];
        public float[] Filtered = new float[BufferSize];

        private int _index;

        public void Push(float v1, float v2, float v3, float filtered)
        {
            Voice1[_index] = v1;
            Voice2[_index] = v2;
            Voice3[_index] = v3;
            Filtered[_index] = filtered;

            _index = (_index + 1) % BufferSize;
        }

        public void Snapshot(float[] v1, float[] v2, float[] v3, float[] filtered)
        {
            Array.Copy(Voice1, v1, BufferSize);
            Array.Copy(Voice2, v2, BufferSize);
            Array.Copy(Voice3, v3, BufferSize);
            Array.Copy(Filtered, filtered, BufferSize);
        }
    }
}
