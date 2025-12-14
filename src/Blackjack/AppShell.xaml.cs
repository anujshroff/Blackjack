using Blackjack.Views;

namespace Blackjack
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute(nameof(SeatSelectionPage), typeof(SeatSelectionPage));
            Routing.RegisterRoute(nameof(GameTablePage), typeof(GameTablePage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
        }
    }
}
