using Blackjack.Views;
using CommunityToolkit.Mvvm.Input;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// ViewModel for the Main Menu page.
    /// Handles navigation to different sections of the application.
    /// </summary>
    public partial class MainMenuViewModel : ViewModelBase
    {
        public MainMenuViewModel()
        {
            Title = "Blackjack";
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
        /// Command to open settings.
        /// Placeholder for future implementation.
        /// </summary>
        [RelayCommand]
        private async Task OpenSettings()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await Shell.Current.DisplayAlertAsync("Coming Soon",
                    "Settings page will be available in Phase 5.",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Command to view statistics.
        /// Placeholder for future implementation.
        /// </summary>
        [RelayCommand]
        private async Task ViewStatistics()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await Shell.Current.DisplayAlertAsync("Coming Soon",
                    "Statistics page will be available in Phase 5.",
                    "OK");
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
