namespace Blackjack.Models
{
    /// <summary>
    /// Represents the dealer in a Blackjack game.
    /// </summary>
    /// <remarks>
    /// Creates a dealer with the specified soft 17 rule.
    /// </remarks>
    /// <param name="hitsSoft17">True for H17 (dealer hits soft 17), false for S17 (dealer stands on soft 17)</param>
    public class Dealer(bool hitsSoft17)
    {
        public Hand Hand { get; set; } = new Hand();

        /// <summary>
        /// The dealer's face-up card (visible to all players).
        /// </summary>
        public Card? UpCard => Hand.Cards.Count > 0 ? Hand.Cards[0] : null;

        /// <summary>
        /// The dealer's face-down card (hole card, hidden until dealer's turn).
        /// </summary>
        public Card? HoleCard => Hand.Cards.Count > 1 ? Hand.Cards[1] : null;

        /// <summary>
        /// True if the hole card has been revealed.
        /// </summary>
        public bool HoleCardRevealed { get; set; } = false;

        /// <summary>
        /// Whether the dealer hits on soft 17 (H17 rule).
        /// </summary>
        public bool HitsSoft17 { get; set; } = hitsSoft17;

        /// <summary>
        /// Creates a dealer with the default H17 rule (hits soft 17).
        /// </summary>
        public Dealer() : this(hitsSoft17: true)
        {
        }

        /// <summary>
        /// Adds a card to the dealer's hand.
        /// </summary>
        public void AddCard(Card card)
        {
            Hand.AddCard(card);
        }

        /// <summary>
        /// Reveals the dealer's hole card.
        /// </summary>
        public void RevealHoleCard()
        {
            HoleCardRevealed = true;
        }

        /// <summary>
        /// Determines if the dealer should hit based on the configured soft 17 rule.
        /// H17: Dealer hits on soft 17, stands on hard 17 and all 18+.
        /// S17: Dealer stands on all 17s (soft and hard).
        /// </summary>
        /// <returns>True if dealer should hit, false if dealer should stand.</returns>
        public bool ShouldHit()
        {
            int total = Hand.TotalValue;

            // Stand on 18+
            if (total >= 18)
                return false;

            // Stand on hard 17
            if (total == 17 && !Hand.IsSoft)
                return false;

            // Soft 17: hit or stand based on rule setting
            if (total == 17 && Hand.IsSoft)
                return HitsSoft17;

            // Hit on anything less than 17
            if (total < 17)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if the dealer has Blackjack (when up card is Ace or 10-value).
        /// </summary>
        public bool HasBlackjack => Hand.IsBlackjack;

        /// <summary>
        /// Clears the dealer's hand for a new round.
        /// </summary>
        public void ClearHand()
        {
            Hand.Clear();
            HoleCardRevealed = false;
        }

        public override string ToString()
        {
            if (!HoleCardRevealed && Hand.Cards.Count >= 2)
            {
                return $"Dealer: {UpCard} + [Hidden]";
            }
            return $"Dealer: {Hand}";
        }
    }
}
