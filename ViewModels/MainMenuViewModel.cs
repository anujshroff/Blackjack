using Blackjack.Models;
using Blackjack.Services;
using Blackjack.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// ViewModel for the Main Menu page.
    /// Handles navigation to different sections of the application.
    /// </summary>
    public partial class MainMenuViewModel : ViewModelBase
    {
        private readonly BankrollService _bankrollService;
        private readonly GameSettings _settings;

        /// <summary>
        /// Indicates if there is a saved bankroll.
        /// </summary>
        [ObservableProperty]
        private bool hasSavedBankroll;

        /// <summary>
        /// The current saved bankroll amount.
        /// </summary>
        [ObservableProperty]
        private decimal savedBankroll;

        /// <summary>
        /// The starting bankroll from settings (for display).
        /// </summary>
        [ObservableProperty]
        private decimal startingBankroll;

        public MainMenuViewModel(BankrollService bankrollService)
        {
            Title = "Blackjack";
            _bankrollService = bankrollService;

            // Load settings from persistent storage
            _settings = new GameSettings
            {
                TableMinimum = SettingsService.LoadTableMinimum(),
                TableMaximum = SettingsService.LoadTableMaximum(),
                StartingBankroll = SettingsService.LoadStartingBankroll(),
                NumberOfDecks = SettingsService.LoadNumberOfDecks()
            };

            StartingBankroll = _settings.StartingBankroll;

            // Load current bankroll state
            RefreshBankrollState();
        }

        /// <summary>
        /// Refreshes the bankroll state from persistent storage.
        /// Also handles auto-reset if bankroll is $0 or below table minimum.
        /// </summary>
        public void RefreshBankrollState()
        {
            HasSavedBankroll = BankrollService.HasSavedBankroll();

            if (HasSavedBankroll)
            {
                SavedBankroll = BankrollService.GetSavedBankroll() ?? _settings.StartingBankroll;

                // Auto-reset if bankroll is below table minimum
                if (SavedBankroll < _settings.TableMinimum)
                {
                    BankrollService.ResetBankroll();
                    HasSavedBankroll = false;
                    SavedBankroll = _settings.StartingBankroll;
                }
            }
            else
            {
                SavedBankroll = _settings.StartingBankroll;
            }
        }

        /// <summary>
        /// Command to start a new game.
        /// Navigates to the seat selection page.
        /// </summary>
        [RelayCommand]
        private async Task StartGame()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                // Navigate to seat selection page
                await Shell.Current.GoToAsync(nameof(SeatSelectionPage));
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Command to open settings page.
        /// </summary>
        [RelayCommand]
        private async Task OpenSettings()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await Shell.Current.GoToAsync(nameof(SettingsPage));
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Reloads settings from persistent storage.
        /// Should be called when returning from the Settings page.
        /// </summary>
        public void ReloadSettings()
        {
            _settings.TableMinimum = SettingsService.LoadTableMinimum();
            _settings.TableMaximum = SettingsService.LoadTableMaximum();
            _settings.StartingBankroll = SettingsService.LoadStartingBankroll();
            _settings.NumberOfDecks = SettingsService.LoadNumberOfDecks();

            StartingBankroll = _settings.StartingBankroll;
            RefreshBankrollState();
        }

        /// <summary>
        /// Command to reset the bankroll to starting value.
        /// Shows confirmation dialog before resetting.
        /// </summary>
        [RelayCommand]
        private async Task ResetBankroll()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                bool confirmed = await Shell.Current.DisplayAlertAsync(
                    "Reset Bankroll",
                    $"Are you sure you want to reset your bankroll to ${_settings.StartingBankroll:N0}?\n\nCurrent bankroll: ${SavedBankroll:N0}",
                    "Reset",
                    "Cancel");

                if (confirmed)
                {
                    BankrollService.ResetBankroll();
                    RefreshBankrollState();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Command to exit the application.
        /// Only available on Windows platform.
        /// </summary>
        [RelayCommand]
        private static void ExitApp()
        {
#if WINDOWS
            Application.Current?.Quit();
#endif
        }
    }
}
