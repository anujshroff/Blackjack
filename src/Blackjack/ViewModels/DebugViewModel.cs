using Blackjack.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// Represents the result of a deck audit for a specific number of decks.
    /// </summary>
    public partial class DeckAuditResult : ObservableObject
    {
        [ObservableProperty]
        private int numberOfDecks;

        [ObservableProperty]
        private bool passed;

        [ObservableProperty]
        private int expectedTotal;

        [ObservableProperty]
        private int actualTotal;

        [ObservableProperty]
        private ObservableCollection<string> errors = [];
    }

    /// <summary>
    /// ViewModel for the Debug Menu page.
    /// Provides debug options and diagnostic tools (hidden menu).
    /// </summary>
    public partial class DebugViewModel : ViewModelBase
    {
        /// <summary>
        /// Available deck options to audit.
        /// </summary>
        private static readonly int[] DeckOptions = [1, 2, 4, 6, 8];

        /// <summary>
        /// Results from the deck audit.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<DeckAuditResult> auditResults = [];

        /// <summary>
        /// Indicates whether an audit has been run.
        /// </summary>
        [ObservableProperty]
        private bool hasAuditResults;

        public DebugViewModel()
        {
            Title = "Debug Menu";
        }

        /// <summary>
        /// Command to navigate back to the main menu.
        /// </summary>
        [RelayCommand]
        private static async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        /// <summary>
        /// Command to run the deck audit for all available deck configurations.
        /// </summary>
        [RelayCommand]
        private void RunDeckAudit()
        {
            AuditResults.Clear();

            foreach (int deckCount in DeckOptions)
            {
                var result = AuditDeck(deckCount);
                AuditResults.Add(result);
            }

            HasAuditResults = true;
        }

        /// <summary>
        /// Audits a deck with the specified number of decks.
        /// Verifies each rank/suit combination count and total card count.
        /// </summary>
        private static DeckAuditResult AuditDeck(int numberOfDecks)
        {
            var result = new DeckAuditResult
            {
                NumberOfDecks = numberOfDecks,
                Errors = []
            };

            // Create a fresh deck
            var deck = new Deck(numberOfDecks);
            var cards = deck.GetCardsForAudit();

            // Calculate expected values
            // Note: Rank enum has 13 distinct names even though some share the same value
            int rankCount = Enum.GetNames<Rank>().Length;
            int suitCount = Enum.GetNames<Suit>().Length;
            result.ExpectedTotal = rankCount * suitCount * numberOfDecks;
            result.ActualTotal = cards.Count;

            // Check total count
            bool totalCorrect = result.ActualTotal == result.ExpectedTotal;
            if (!totalCorrect)
            {
                result.Errors.Add($"Total count mismatch: expected {result.ExpectedTotal}, got {result.ActualTotal}");
            }

            // Check each rank/suit combination
            // Use Enum.GetNames to get distinct rank names, then parse back to enum
            // This avoids issues with duplicate enum values (Ten, Jack, Queen, King all = 10)
            foreach (string rankName in Enum.GetNames<Rank>())
            {
                Rank rank = Enum.Parse<Rank>(rankName);
                foreach (Suit suit in Enum.GetValues<Suit>())
                {
                    // Compare by enum name to handle duplicate values correctly
                    int count = cards.Count(c => c.Rank.ToString() == rankName && c.Suit == suit);
                    if (count != numberOfDecks)
                    {
                        result.Errors.Add($"{rank} of {suit}: expected {numberOfDecks}, got {count}");
                    }
                }
            }

            result.Passed = result.Errors.Count == 0;
            return result;
        }
    }
}
