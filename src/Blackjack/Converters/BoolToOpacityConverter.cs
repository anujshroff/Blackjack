using System.Globalization;

namespace Blackjack.Converters
{
    /// <summary>
    /// Converts boolean values to opacity values for UI elements.
    /// True = 1.0 (fully opaque), False = 0.4 (semi-transparent/disabled look).
    /// </summary>
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? 1.0 : 0.4;
            }
            return 0.4; // Default to disabled look
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
