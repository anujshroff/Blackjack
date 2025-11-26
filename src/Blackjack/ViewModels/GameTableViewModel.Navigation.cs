using CommunityToolkit.Mvvm.Input;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// Navigation-related commands for GameTableViewModel.
    /// </summary>
    public partial class GameTableViewModel
    {
        /// <summary>
        /// Command to return to the main menu.
        /// </summary>
        [RelayCommand]
        private static async Task GoToMenu()
        {
            var result = await Shell.Current.DisplayAlertAsync(
                "Leave Table?",
                "Are you sure you want to leave the table and return to the main menu?",
                "Yes", "No");

            if (result)
            {
                await Shell.Current.GoToAsync("//MainMenuPage");
            }
        }
    }
}
