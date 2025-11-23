namespace Blackjack.Models
{
    /// <summary>
    /// Represents the configurable settings for a Blackjack game.
    /// </summary>
    public class GameSettings
    {
        /// <summary>
        /// Minimum bet allowed at the table.
        /// </summary>
        public decimal TableMinimum { get; set; }

        /// <summary>
        /// Maximum bet allowed at the table.
        /// </summary>
        public decimal TableMaximum { get; set; }

        /// <summary>
        /// Starting bankroll for each player.
        /// </summary>
        public decimal StartingBankroll { get; set; }

        /// <summary>
        /// Number of decks in the shoe (configurable: 1, 2, 4, 6, or 8).
        /// </summary>
        public int NumberOfDecks { get; set; }

        /// <summary>
        /// Dealer hits on soft 17 (H17 rule - standard for this game).
        /// </summary>
        public bool DealerHitsSoft17 { get; }

        /// <summary>
        /// Blackjack payout ratio (3:2 = 1.5, 6:5 = 1.2). Standard is 3:2.
        /// </summary>
        public decimal BlackjackPayout { get; set; }

        /// <summary>
        /// Insurance payout ratio (standard is 2:1).
        /// </summary>
        public decimal InsurancePayout { get; set; }

        /// <summary>
        /// Maximum number of times a hand can be split (typically 3 re-splits = 4 hands total).
        /// </summary>
        public int MaxSplits { get; set; }

        /// <summary>
        /// Whether double down is allowed after splitting.
        /// </summary>
        public bool DoubleAfterSplit { get; set; }

        /// <summary>
        /// Creates default game settings following standard US casino rules.
        /// </summary>
        public GameSettings()
        {
            TableMinimum = 5m;
            TableMaximum = 500m;
            StartingBankroll = 1000m;
            NumberOfDecks = 6; // Fixed at 6 decks
            DealerHitsSoft17 = true; // H17 rule
            BlackjackPayout = 1.5m; // 3:2
            InsurancePayout = 2.0m; // 2:1
            MaxSplits = 3; // Can split up to 3 times (4 hands total)
            DoubleAfterSplit = true; // Allowed except on split Aces
        }

        /// <summary>
        /// Creates custom game settings.
        /// </summary>
        public GameSettings(decimal tableMin, decimal tableMax, decimal startingBankroll)
        {
            TableMinimum = tableMin;
            TableMaximum = tableMax;
            StartingBankroll = startingBankroll;
            NumberOfDecks = 6; // Fixed at 6 decks
            DealerHitsSoft17 = true; // H17 rule
            BlackjackPayout = 1.5m; // 3:2
            InsurancePayout = 2.0m; // 2:1
            MaxSplits = 3; // Can split up to 3 times (4 hands total)
            DoubleAfterSplit = true; // Allowed except on split Aces
        }

        /// <summary>
        /// Validates that a bet amount is within table limits.
        /// </summary>
        public bool IsValidBet(decimal amount)
        {
            return amount >= TableMinimum && amount <= TableMaximum;
        }

        public override string ToString()
        {
            return $"Table: ${TableMinimum}-${TableMaximum}, Starting Bankroll: ${StartingBankroll}, " +
                   $"6 Decks, H17, Blackjack pays {BlackjackPayout}:1";
        }
    }
}
