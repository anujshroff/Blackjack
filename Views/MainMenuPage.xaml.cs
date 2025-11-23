using Blackjack.ViewModels;

namespace Blackjack.Views
{
    /// <summary>
    /// Main Menu page - entry point of the application.
    /// </summary>
    public partial class MainMenuPage : ContentPage
    {
        public MainMenuPage(MainMenuViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            // Show Exit button only on Windows platform
#if WINDOWS
            ExitButton.IsVisible = true;
#endif
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Initialize ViewModel if needed
            if (BindingContext is MainMenuViewModel viewModel)
            {
                await viewModel.InitializeAsync();
            }
        }
    }
}
