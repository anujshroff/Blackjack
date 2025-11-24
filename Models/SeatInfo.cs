using CommunityToolkit.Mvvm.ComponentModel;

namespace Blackjack.Models
{
    /// <summary>
    /// Represents a seat at the blackjack table.
    /// </summary>
    public partial class SeatInfo : ObservableObject
    {
        /// <summary>
        /// Seat position number (1-7).
        /// </summary>
        public int SeatNumber { get; init; }

        /// <summary>
        /// Display label for the seat (e.g., "1").
        /// </summary>
        public string PositionLabel => SeatNumber.ToString();

        /// <summary>
        /// Indicates if this seat is occupied by the human player.
        /// </summary>
        [ObservableProperty]
        private bool isPlayer;

        /// <summary>
        /// Indicates if this seat is occupied by an AI player.
        /// </summary>
        [ObservableProperty]
        private bool isAI;

        /// <summary>
        /// Indicates if this seat is empty/available.
        /// </summary>
        public bool IsEmpty => !IsPlayer && !IsAI;
    }
}
