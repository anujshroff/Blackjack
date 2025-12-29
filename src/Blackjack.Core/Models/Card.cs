namespace Blackjack.Models
{
    /// <summary>
    /// Represents the four suits in a standard deck of playing cards.
    /// </summary>
    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    /// <summary>
    /// Represents the ranks of playing cards with their numeric values.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "The values are literallty the same but we need the different keys.")]
    public enum Rank
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 10,
        Queen = 10,
        King = 10,
        Ace = 11  // Ace can be 1 or 11, but default value is 11
    }

    /// <summary>
    /// Represents a single playing card with a suit and rank.
    /// </summary>
    public class Card
    {
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }

        /// <summary>
        /// The name of the rank (e.g., "Jack", "Queen", "King").
        /// Stored explicitly because Rank enum has duplicate values causing ToString() issues.
        /// </summary>
        public string RankName { get; set; }

        /// <summary>
        /// Gets the numeric value of the card (2-10 for number cards, 10 for face cards, 11 for Ace).
        /// Note: Ace value is handled at the Hand level (can be 1 or 11).
        /// </summary>
        public int Value => (int)Rank;

        /// <summary>
        /// Path to the card image resource. Will be set based on suit and rank.
        /// </summary>
        public string ImagePath { get; set; }

        public Card(Suit suit, Rank rank, string rankName)
        {
            Suit = suit;
            Rank = rank;
            RankName = rankName;

            // Map rank to file name using the explicit rank name
            string suitName = suit.ToString().ToLower();
            ImagePath = $"Resources/Images/Cards/{rankName.ToLower()}_{suitName}.svg";
        }

        /// <summary>
        /// Returns a string representation of the card (e.g., "Ace of Hearts").
        /// </summary>
        public override string ToString()
        {
            return $"{RankName} of {Suit}";
        }
    }
}
