namespace JollySidEmu.Models
{
    public record SidRegisterWrite(long Cycle, int Address, byte Value, string Source)
    {
        public string AddressHex => $"0x{Address:X4}";
        public string ValueHex => $"0x{Value:X2}";
        public string Decoded => SidRegisterDecoder.Decode(Address, Value);
    }
}
