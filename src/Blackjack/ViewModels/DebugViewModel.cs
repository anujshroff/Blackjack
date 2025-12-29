using CommunityToolkit.Mvvm.Input;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// ViewModel for the Debug Menu page.
    /// Provides debug options and diagnostic tools (hidden menu).
    /// </summary>
    public partial class DebugViewModel : ViewModelBase
    {
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
    }
}
