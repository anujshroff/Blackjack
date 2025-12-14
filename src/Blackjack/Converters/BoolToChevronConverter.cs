using FluentIcons.Common;
using System.Globalization;

namespace Blackjack.Converters;

/// <summary>
/// Converts a boolean to a chevron icon - ChevronDown when false, ChevronUp when true.
/// </summary>
public class BoolToChevronConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isExpanded)
        {
            return isExpanded ? Symbol.ChevronUp : Symbol.ChevronDown;
        }
        return Symbol.ChevronDown;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
