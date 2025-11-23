namespace Blackjack.Models
{
    /// <summary>
    /// Represents the dealer in a Blackjack game.
    /// </summary>
    public class Dealer
    {
        public Hand Hand { get; set; }

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
        public bool HoleCardRevealed { get; set; }

        public Dealer()
        {
            Hand = new Hand();
            HoleCardRevealed = false;
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
        /// Determines if the dealer should hit according to H17 rules.
        /// H17: Dealer hits on soft 17, stands on hard 17 and all 18+.
        /// </summary>
        /// <returns>True if dealer should hit, false if dealer should stand.</returns>
        public bool ShouldHit()
        {
            int total = Hand.TotalValue;

            // Stand on hard 17 and all 18+
            if (total >= 18)
                return false;

            // Stand on hard 17
            if (total == 17 && !Hand.IsSoft)
                return false;

            // Hit on soft 17 (H17 rule)
            if (total == 17 && Hand.IsSoft)
                return true;

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
