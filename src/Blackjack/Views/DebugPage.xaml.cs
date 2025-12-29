using Blackjack.ViewModels;

namespace Blackjack.Views
{
    /// <summary>
    /// Debug Menu page - hidden developer tools and diagnostics.
    /// Access by tapping the logo on the main menu 10 times.
    /// </summary>
    public partial class DebugPage : ContentPage
    {
        public DebugPage(DebugViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
