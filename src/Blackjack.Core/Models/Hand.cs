namespace Blackjack.Models
{
    /// <summary>
    /// Represents the status of a hand during gameplay.
    /// </summary>
    public enum HandStatus
    {
        Active,      // Hand is still being played
        Standing,    // Player has chosen to stand
        Busted,      // Hand total exceeds 21
        Blackjack,   // Natural 21 (Ace + 10-value card on initial deal)
        Won,         // Hand won against dealer
        Lost,        // Hand lost to dealer
        Push         // Tie with dealer
    }

    /// <summary>
    /// Represents a Blackjack hand with cards, value calculation, and betting.
    /// </summary>
    public class Hand
    {
        public List<Card> Cards { get; set; }
        public decimal Bet { get; set; }
        public HandStatus Status { get; set; }

        /// <summary>
        /// Indicates if this hand is waiting for its second card (used in splits).
        /// </summary>
        public bool NeedsSecondCard { get; set; }

        /// <summary>
        /// The index of this hand in the player's hand collection (0 = first hand, 1 = second split hand, etc.).
        /// </summary>
        public int HandIndex { get; set; }

        /// <summary>
        /// Indicates if this hand was created from a split (not eligible for natural blackjack payout).
        /// </summary>
        public bool IsFromSplit { get; set; }

        /// <summary>
        /// Gets the total value of the hand, automatically handling Aces as 1 or 11.
        /// </summary>
        public int TotalValue
        {
            get
            {
                int total = 0;
                int aceCount = 0;

                // First pass: add all card values, counting Aces
                foreach (var card in Cards)
                {
                    total += card.Value;
                    if (card.Rank == Rank.Ace)
                    {
                        aceCount++;
                    }
                }

                // Adjust for Aces: convert from 11 to 1 if busting
                while (total > 21 && aceCount > 0)
                {
                    total -= 10; // Convert one Ace from 11 to 1
                    aceCount--;
                }

                return total;
            }
        }

        /// <summary>
        /// True if the hand contains an Ace counted as 11 (soft hand).
        /// A hand is soft when at least one Ace can be counted as 11 without busting.
        /// </summary>
        public bool IsSoft
        {
            get
            {
                int baseTotal = 0;  // Total with all Aces counted as 1
                int aceCount = 0;

                foreach (var card in Cards)
                {
                    if (card.Rank == Rank.Ace)
                    {
                        baseTotal += 1;  // Count Ace as 1
                        aceCount++;
                    }
                    else
                    {
                        baseTotal += card.Value;
                    }
                }

                // Soft hand: has an Ace AND counting one Ace as 11 doesn't bust
                // (i.e., base total + 10 <= 21)
                return aceCount > 0 && (baseTotal + 10) <= 21;
            }
        }

        /// <summary>
        /// True if this is a natural Blackjack (Ace + 10-value card on first 2 cards, not from a split).
        /// </summary>
        public bool IsBlackjack
        {
            get
            {
                // Split hands cannot have natural blackjack - they pay 1:1 instead of 3:2
                if (IsFromSplit)
                    return false;

                if (Cards.Count != 2)
                    return false;

                bool hasAce = Cards.Any(c => c.Rank == Rank.Ace);
                bool hasTenValue = Cards.Any(c => c.Value == 10);

                return hasAce && hasTenValue;
            }
        }

        /// <summary>
        /// True if the hand total exceeds 21.
        /// </summary>
        public bool IsBusted => TotalValue > 21;

        /// <summary>
        /// True if the hand is a pair (two cards of the same rank), eligible for splitting.
        /// </summary>
        public bool IsPair
        {
            get
            {
                if (Cards.Count != 2)
                    return false;

                // All 10-value cards (10, J, Q, K) are considered a pair
                return Cards[0].Value == Cards[1].Value;
            }
        }

        public Hand()
        {
            Cards = [];
            Bet = 0;
            Status = HandStatus.Active;
            NeedsSecondCard = false;
            HandIndex = 0;
        }

        /// <summary>
        /// Adds a card to the hand.
        /// </summary>
        public void AddCard(Card card)
        {
            Cards.Add(card);

            // Auto-update status if busted
            if (IsBusted)
            {
                Status = HandStatus.Busted;
            }
        }

        /// <summary>
        /// Clears all cards from the hand and resets status.
        /// </summary>
        public void Clear()
        {
            Cards.Clear();
            Bet = 0;
            Status = HandStatus.Active;
            IsFromSplit = false;
        }

        /// <summary>
        /// Returns a string representation of the hand.
        /// </summary>
        public override string ToString()
        {
            string cards = string.Join(", ", Cards.Select(c => c.ToString()));
            return $"Hand: [{cards}] Total: {TotalValue} ({(IsSoft ? "Soft" : "Hard")})";
        }
    }
}
