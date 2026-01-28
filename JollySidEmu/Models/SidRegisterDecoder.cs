namespace JollySidEmu.Models
{
    public static class SidRegisterDecoder
    {
        public static string Decode(int address, byte value)
        {
            return address switch
            {
                // Voice 1 Control Register
                0xD404 => DecodeVoiceControl(value),
                0xD405 => DecodeADSR(value),
                0xD406 => DecodeADSR(value),
                // Voice 2 Control Register
                0xD40B => DecodeVoiceControl(value),
                0xD40C => DecodeADSR(value),
                0xD40D => DecodeADSR(value),
                // Voice 3 Control Register
                0xD412 => DecodeVoiceControl(value),
                0xD413 => DecodeADSR(value),
                0xD414 => DecodeADSR(value),
                // Voice routing / Resonance
                0xD417 => DecodeRoutingResonance(value),
                // Filter Control / Volume
                0xD418 => DecodeFilterModeVolume(value),
                _ => string.Empty
            };
        }

        private static string DecodeVoiceControl(byte value)
        {
            var bits = new List<string>();
            if ((value & 0x01) != 0) bits.Add("Gate");
            if ((value & 0x02) != 0) bits.Add("Sync");
            if ((value & 0x04) != 0) bits.Add("Ring");
            if ((value & 0x10) != 0) bits.Add("Triangle");
            if ((value & 0x20) != 0) bits.Add("Saw");
            if ((value & 0x40) != 0) bits.Add("Pulse");
            if ((value & 0x80) != 0) bits.Add("Noise");

            return string.Join(" | ", bits);
        }

        private static string DecodeADSR(byte value)
        {
            int highNibble = (value >> 4) & 0x0F;
            int lowNibble = value & 0x0F;
            return $"Hi:{highNibble:X} Lo:{lowNibble:X}";
        }

        private static string DecodeRoutingResonance(byte value)
        {
            var modes = new List<string>();
            if ((value & 0x01) != 0) modes.Add("Voice1");
            if ((value & 0x02) != 0) modes.Add("Voice2");
            if ((value & 0x04) != 0) modes.Add("Voice3");
            int resonance = (value >> 4) & 0x0F; // bits 4–7

            return $"{string.Join(" | ", modes)} | Resonance:{resonance}";
        }

        private static string DecodeFilterModeVolume(byte value)
        {
            var modes = new List<string>();
            if ((value & 0x10) != 0) modes.Add("Filter LowPass");
            if ((value & 0x20) != 0) modes.Add("Filter BandPass");
            if ((value & 0x40) != 0) modes.Add("Filter HighPass");
            int volume = value & 0x0F;

            return $"{string.Join(" | ", modes)} | Vol:{volume}";
        }
    }
}
