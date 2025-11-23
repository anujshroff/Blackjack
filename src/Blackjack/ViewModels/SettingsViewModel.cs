using Blackjack.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// ViewModel for the Settings page.
    /// Handles loading, editing, and saving game settings.
    /// </summary>
    public partial class SettingsViewModel : ViewModelBase
    {
        // Original values to track changes
        private decimal _originalTableMinimum;
        private decimal _originalTableMaximum;
        private decimal _originalStartingBankroll;
        private int _originalNumberOfDecks;

        /// <summary>
        /// Minimum bet allowed at the table.
        /// </summary>
        [ObservableProperty]
        private decimal tableMinimum;

        /// <summary>
        /// Maximum bet allowed at the table.
        /// </summary>
        [ObservableProperty]
        private decimal tableMaximum;

        /// <summary>
        /// Starting bankroll for new players.
        /// </summary>
        [ObservableProperty]
        private decimal startingBankroll;

        /// <summary>
        /// Number of decks in the shoe.
        /// </summary>
        [ObservableProperty]
        private int numberOfDecks;

        /// <summary>
        /// Available deck options for the picker.
        /// </summary>
        public ObservableCollection<int> DeckOptions { get; } = [1, 2, 4, 6, 8];

        /// <summary>
        /// Indicates whether there are unsaved changes.
        /// </summary>
        [ObservableProperty]
        private bool hasUnsavedChanges;

        public SettingsViewModel()
        {
            Title = "Settings";
            LoadSettings();
        }

        /// <summary>
        /// Called when TableMinimum changes.
        /// </summary>
        partial void OnTableMinimumChanged(decimal value)
        {
            CheckForChanges();
        }

        /// <summary>
        /// Called when TableMaximum changes.
        /// </summary>
        partial void OnTableMaximumChanged(decimal value)
        {
            CheckForChanges();
        }

        /// <summary>
        /// Called when StartingBankroll changes.
        /// </summary>
        partial void OnStartingBankrollChanged(decimal value)
        {
            CheckForChanges();
        }

        /// <summary>
        /// Called when NumberOfDecks changes.
        /// </summary>
        partial void OnNumberOfDecksChanged(int value)
        {
            CheckForChanges();
        }

        /// <summary>
        /// Checks if any settings have changed from their original values.
        /// </summary>
        private void CheckForChanges()
        {
            HasUnsavedChanges =
                TableMinimum != _originalTableMinimum ||
                TableMaximum != _originalTableMaximum ||
                StartingBankroll != _originalStartingBankroll ||
                NumberOfDecks != _originalNumberOfDecks;
        }

        /// <summary>
        /// Loads settings from persistent storage.
        /// </summary>
        private void LoadSettings()
        {
            TableMinimum = SettingsService.LoadTableMinimum();
            TableMaximum = SettingsService.LoadTableMaximum();
            StartingBankroll = SettingsService.LoadStartingBankroll();
            NumberOfDecks = SettingsService.LoadNumberOfDecks();

            // Store original values
            _originalTableMinimum = TableMinimum;
            _originalTableMaximum = TableMaximum;
            _originalStartingBankroll = StartingBankroll;
            _originalNumberOfDecks = NumberOfDecks;

            HasUnsavedChanges = false;
        }

        /// <summary>
        /// Command to save all settings.
        /// </summary>
        [RelayCommand]
        private async Task SaveSettings()
        {
            if (IsBusy || !HasUnsavedChanges)
                return;

            try
            {
                IsBusy = true;

                // Validate settings
                if (TableMinimum <= 0)
                {
                    await Shell.Current.DisplayAlertAsync("Invalid Setting",
                        "Table minimum must be greater than $0.",
                        "OK");
                    return;
                }

                if (TableMaximum <= TableMinimum)
                {
                    await Shell.Current.DisplayAlertAsync("Invalid Setting",
                        "Table maximum must be greater than table minimum.",
                        "OK");
                    return;
                }

                if (StartingBankroll < TableMinimum)
                {
                    await Shell.Current.DisplayAlertAsync("Invalid Setting",
                        "Starting bankroll must be at least the table minimum.",
                        "OK");
                    return;
                }

                // Save all settings
                SettingsService.SaveAllSettings(TableMinimum, TableMaximum, StartingBankroll, NumberOfDecks);

                // Update original values
                _originalTableMinimum = TableMinimum;
                _originalTableMaximum = TableMaximum;
                _originalStartingBankroll = StartingBankroll;
                _originalNumberOfDecks = NumberOfDecks;

                HasUnsavedChanges = false;

                await Shell.Current.DisplayAlertAsync("Settings Saved",
                    "Your settings have been saved successfully.",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Command to reset settings to defaults.
        /// </summary>
        [RelayCommand]
        private async Task ResetToDefaults()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                bool confirmed = await Shell.Current.DisplayAlertAsync(
                    "Reset Settings",
                    "Are you sure you want to reset all settings to their default values?",
                    "Reset",
                    "Cancel");

                if (confirmed)
                {
                    SettingsService.ResetToDefaults();
                    LoadSettings();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Command to go back to the main menu.
        /// </summary>
        [RelayCommand]
        private async Task GoBack()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (HasUnsavedChanges)
                {
                    bool discard = await Shell.Current.DisplayAlertAsync(
                        "Unsaved Changes",
                        "You have unsaved changes. Do you want to discard them?",
                        "Discard",
                        "Cancel");

                    if (!discard)
                    {
                        return;
                    }
                }

                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
