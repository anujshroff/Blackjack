using Blackjack.ViewModels;

namespace Blackjack.Views
{
    /// <summary>
    /// Settings page for configuring game options.
    /// </summary>
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
