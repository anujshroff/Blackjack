using Blackjack.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// ViewModel for the Seat Selection page.
    /// Handles seat selection, AI player configuration, and navigation to the game table.
    /// </summary>
    public partial class SeatSelectionViewModel : ViewModelBase
    {
        /// <summary>
        /// Collection of all 7 seats at the table.
        /// </summary>
        public ObservableCollection<SeatInfo> Seats { get; } = [];

        /// <summary>
        /// The seat number selected by the player (1-7), or null if no seat selected.
        /// </summary>
        [ObservableProperty]
        private int? selectedSeat;

        /// <summary>
        /// Number of AI players (0-6).
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanStartGame))]
        private int aiPlayerCount;

        /// <summary>
        /// Indicates whether the Start Game button should be enabled.
        /// Requires a seat to be selected.
        /// </summary>
        public bool CanStartGame => SelectedSeat.HasValue;

        private readonly Random _random = new();

        public SeatSelectionViewModel()
        {
            Title = "Choose Your Seat";

            // Initialize all 7 seats
            for (int i = 1; i <= 7; i++)
            {
                Seats.Add(new SeatInfo { SeatNumber = i });
            }
        }

        /// <summary>
        /// Command to select a seat for the player.
        /// </summary>
        [RelayCommand]
        private void SelectSeat(int seatNumber)
        {
            // Update the previously selected seat (if any)
            if (SelectedSeat.HasValue)
            {
                var previousSeat = Seats[SelectedSeat.Value - 1];
                previousSeat.IsPlayer = false;
            }

            // Update the newly selected seat
            SelectedSeat = seatNumber;
            var newSeat = Seats[seatNumber - 1];
            newSeat.IsPlayer = true;
            newSeat.IsAI = false; // Can't be both player and AI

            // Redistribute AI players (in case AI was at this seat)
            PlaceAIPlayers();

            // Notify that CanStartGame may have changed
            OnPropertyChanged(nameof(CanStartGame));
        }

        /// <summary>
        /// Called when AI player count changes. Redistributes AI players randomly.
        /// </summary>
        partial void OnAiPlayerCountChanged(int value)
        {
            PlaceAIPlayers();
        }

        /// <summary>
        /// Randomly places AI players at available seats (excluding player's seat).
        /// </summary>
        private void PlaceAIPlayers()
        {
            // First, clear all AI markers
            foreach (var seat in Seats)
            {
                if (!seat.IsPlayer)
                {
                    seat.IsAI = false;
                }
            }

            // If no AI players, we're done
            if (AiPlayerCount == 0)
                return;

            // Get list of available seat numbers (not occupied by player)
            var availableSeats = Seats
                .Where(s => !s.IsPlayer)
                .Select(s => s.SeatNumber)
                .ToList();

            // Randomly select seats for AI players
            var aiSeats = availableSeats
                .OrderBy(_ => _random.Next())
                .Take(AiPlayerCount)
                .ToList();

            // Mark the selected seats as AI
            foreach (var seatNumber in aiSeats)
            {
                Seats[seatNumber - 1].IsAI = true;
            }
        }

        /// <summary>
        /// Command to start the game and navigate to the game table.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanStartGame))]
        private async Task StartGame()
        {
            if (!SelectedSeat.HasValue)
                return;

            // Get the exact AI seat positions
            var aiPositions = Seats
                .Where(s => s.IsAI)
                .Select(s => s.SeatNumber)
                .OrderBy(n => n)
                .ToList();

            var aiPositionsString = string.Join(",", aiPositions);

            // Navigate to game table with configuration
            var navigationParameter = new Dictionary<string, object>
            {
                { "HumanPosition", SelectedSeat.Value },
                { "AICount", AiPlayerCount },
                { "AIPositions", aiPositionsString }
            };

            await Shell.Current.GoToAsync("GameTablePage", navigationParameter);
        }

        /// <summary>
        /// Command to go back to the main menu.
        /// </summary>
        [RelayCommand]
        private static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
