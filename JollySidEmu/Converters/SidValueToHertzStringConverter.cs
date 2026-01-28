using System.Globalization;
using System.Windows.Data;

namespace JollySidEmu.Converters
{
    public class SidValueToHertzStringConverter : IValueConverter
    {
        public bool IsPal { get; set; } = true;

        private double SidClock => IsPal ? 1023000.0 : 985248.0;
        private const double PhaseBits = 16777216.0; // 2^24

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int sidValue)
            {
                double hz = sidValue * SidClock / PhaseBits;
                return $"{hz:F1} Hz";
            }

            return "0.0 Hz";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Display only – no editing
            return Binding.DoNothing;
        }
    }
}
