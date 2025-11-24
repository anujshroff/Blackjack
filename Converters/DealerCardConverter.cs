using System.Globalization;

namespace Blackjack.Converters
{
    /// <summary>
    /// Converts dealer cards to images, showing card_back for the hole card when face-down.
    /// Takes the card index as parameter to determine if it's the hole card (index 1).
    /// </summary>
    public class DealerCardConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not Models.Card card)
                return null;

            // Check if this is the hole card (second card, index 1)
            // Parameter will be passed from XAML indicating if hole card should be hidden
            if (parameter is bool isHoleCardHidden && isHoleCardHidden)
            {
                return ImageSource.FromFile("card_back.png");
            }

            // Otherwise show the actual card
            var converter = new CardToImageConverter();
            return converter.Convert(card, targetType, null, culture);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
