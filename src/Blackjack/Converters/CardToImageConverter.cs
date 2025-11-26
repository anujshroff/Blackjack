using Blackjack.Models;
using System.Globalization;

namespace Blackjack.Converters
{
    /// <summary>
    /// Converts a Card object to an ImageSource.
    /// </summary>
    public class CardToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Card card)
            {
                // Return the card's image path
                return ImageSource.FromFile(GetCardImageFileName(card));
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the image file name for a card (e.g., "ace_hearts.png")
        /// </summary>
        private static string GetCardImageFileName(Card card)
        {
            string rankName = card.Rank.ToString().ToLower();
            string suitName = card.Suit.ToString().ToLower();
            return $"{rankName}_{suitName}.png";
        }
    }

    /// <summary>
    /// Converts a boolean to return either a card image or card back.
    /// Used for showing/hiding the dealer's hole card.
    /// </summary>
    public class CardFaceConverter : IMultiValueConverter
    {
        public object? Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return null;

            // values[0] = Card object
            // values[1] = index (to determine if it's the hole card)
            // values[2] = DealerHoleCardFaceDown boolean

            if (values[0] is not Card card)
                return null;

            // Check if this is the second card (index 1) and if hole card should be face down
            bool isHoleCard = values[1] is int index && index == 1;
            bool isFaceDown = values.Length > 2 && values[2] is bool faceDown && faceDown;

            if (isHoleCard && isFaceDown)
            {
                return ImageSource.FromFile("card_back.png");
            }

            string rankName = card.Rank.ToString().ToLower();
            string suitName = card.Suit.ToString().ToLower();
            return ImageSource.FromFile($"{rankName}_{suitName}.png");
        }

        public object?[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
