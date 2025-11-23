using CommunityToolkit.Mvvm.ComponentModel;

namespace Blackjack.Models
{
    /// <summary>
    /// Represents a player (human or AI) at the Blackjack table.
    /// </summary>
    public partial class Player : ObservableObject
    {
        [ObservableProperty]
        private string name = "";

        public int SeatPosition { get; set; } // 1-7 (first base to third base)

        [ObservableProperty]
        private decimal bankroll;

        public List<Hand> Hands { get; set; }

        [ObservableProperty]
        private bool isHuman;

        [ObservableProperty]
        private bool isActive;

        public Player(string name, int seatPosition, decimal startingBankroll, bool isHuman = false)
        {
            if (seatPosition < 1 || seatPosition > 7)
            {
                throw new ArgumentException("Seat position must be between 1 and 7", nameof(seatPosition));
            }

            Name = name;
            SeatPosition = seatPosition;
            Bankroll = startingBankroll;
            IsHuman = isHuman;
            IsActive = true;
            Hands = [new()]; // Start with one hand
        }

        /// <summary>
        /// Places a bet for the current hand. Deducts from bankroll.
        /// </summary>
        /// <param name="amount">The bet amount.</param>
        /// <returns>True if bet was successful, false if insufficient funds.</returns>
        public bool PlaceBet(decimal amount)
        {
            if (amount > Bankroll)
            {
                return false;
            }

            if (Hands.Count > 0)
            {
                Hands[0].Bet = amount;
                Bankroll -= amount;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds winnings to the player's bankroll.
        /// </summary>
        public void AddWinnings(decimal amount)
        {
            Bankroll += amount;
        }

        /// <summary>
        /// Clears all hands and prepares for a new round.
        /// </summary>
        public void ClearHands()
        {
            Hands.Clear();
            Hands.Add(new Hand());
        }

        /// <summary>
        /// Gets the player's current hand (for display purposes).
        /// </summary>
        public Hand CurrentHand => Hands.Count > 0 ? Hands[0] : new Hand();

        /// <summary>
        /// Checks if the player is bankrupt (no funds remaining).
        /// </summary>
        public bool IsBankrupt => Bankroll <= 0;

        public override string ToString()
        {
            return $"{Name} (Seat {SeatPosition}) - Bankroll: ${Bankroll:F2}";
        }
    }
}
