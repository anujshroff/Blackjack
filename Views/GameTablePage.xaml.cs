using Blackjack.Models;
using Blackjack.ViewModels;
using FluentIcons.Maui;
using Microsoft.Maui.Controls.Shapes;

namespace Blackjack.Views
{
    public partial class GameTablePage : ContentPage
    {
        private GameTableViewModel ViewModel => (GameTableViewModel)BindingContext;

        public GameTablePage(GameTableViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            // Subscribe to ViewModel property changes
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // When ViewModel is initialized, rebuild the player positions
            if (e.PropertyName == nameof(GameTableViewModel.IsInitialized) && ViewModel.IsInitialized)
            {
                // Dispatch to main thread for UI updates
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    BuildPlayerPositions();
                });
            }
        }

        private void BuildPlayerPositions()
        {
            PlayersGrid.Children.Clear();
            PlayersGrid.ColumnDefinitions.Clear();
            PlayersGrid.RowDefinitions.Clear();

            // Create a semi-circular arrangement using a grid
            // Set up 7 columns for 7 positions
            for (int i = 0; i < 7; i++)
            {
                PlayersGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            // Add single row for players
            PlayersGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Calculate semi-circular positions
            foreach (var player in ViewModel.Players)
            {
                var playerCard = CreatePlayerCard(player);

                // Calculate position on the arc
                int position = player.SeatPosition - 1; // Convert to 0-based index

                // Calculate vertical offset for arc effect
                // Positions 1 and 7 are closest to dealer (top), position 4 is furthest (bottom of arc)
                double[] arcOffsets = [0, 15, 30, 40, 30, 15, 0];
                double yOffset = arcOffsets[position];

                // Apply margins for arc effect and make cards slightly overlap
                playerCard.Margin = new Thickness(2, yOffset, 2, 0);

                // Add to grid at the appropriate column
                Grid.SetColumn(playerCard, position);
                Grid.SetRow(playerCard, 0);

                PlayersGrid.Children.Add(playerCard);
            }
        }

        private static Border CreatePlayerCard(Player player)
        {
            // Active indicator border (highlights player's turn and active seats)
            var activeIndicatorColor = player.IsHuman
                ? Application.Current?.Resources["Primary"] as Color ?? Colors.Blue
                : Application.Current?.Resources["Secondary"] as Color ?? Colors.LightBlue;

            var outerBorder = new Border
            {
                Stroke = player.IsActive ? activeIndicatorColor : Colors.Transparent,
                StrokeThickness = player.IsActive ? 3 : 0,
                BackgroundColor = player.IsActive ? Color.FromArgb("#20FFFFFF") : Color.FromArgb("#10FFFFFF"),
                Padding = 12,
                Margin = new Thickness(5),
                HeightRequest = 180,
                WidthRequest = 140,
                StrokeShape = new RoundRectangle { CornerRadius = 12 }
            };

            var content = new VerticalStackLayout
            {
                Spacing = 8
            };

            // Position number
            var positionLabel = new Label
            {
                Text = $"Position {player.SeatPosition}",
                FontSize = 11,
                FontAttributes = FontAttributes.Bold,
                TextColor = Application.Current?.Resources["CasinoGold"] as Color ?? Colors.Gold,
                HorizontalOptions = LayoutOptions.Center
            };
            content.Add(positionLabel);

            // Player avatar and name
            var playerInfoStack = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 5
            };

            if (player.IsActive)
            {
                var avatar = new FluentIcon
                {
                    Icon = (FluentIcons.Common.Icon)FluentIcons.Common.Symbol.Person,
                    IconVariant = player.IsHuman ? FluentIcons.Common.IconVariant.Filled : FluentIcons.Common.IconVariant.Regular,
                    FontSize = 20,
                    ForegroundColor = player.IsHuman
                        ? Application.Current?.Resources["Primary"] as Color ?? Colors.Blue
                        : Application.Current?.Resources["Secondary"] as Color ?? Colors.LightBlue
                };
                playerInfoStack.Add(avatar);

                var nameLabel = new Label
                {
                    Text = player.Name,
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.White,
                    VerticalOptions = LayoutOptions.Center
                };
                playerInfoStack.Add(nameLabel);
            }
            else
            {
                var emptyLabel = new Label
                {
                    Text = "Empty",
                    FontSize = 13,
                    FontAttributes = FontAttributes.Italic,
                    TextColor = Application.Current?.Resources["Gray400"] as Color ?? Colors.Gray,
                    HorizontalOptions = LayoutOptions.Center
                };
                playerInfoStack.Add(emptyLabel);
            }
            content.Add(playerInfoStack);

            // Cards area (empty for Phase 2)
            var cardsArea = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 5,
                HeightRequest = 70
            };

            if (player.IsActive)
            {
                var cardsPlaceholder = new Label
                {
                    Text = "[Cards]",
                    FontSize = 10,
                    FontAttributes = FontAttributes.Italic,
                    TextColor = Application.Current?.Resources["Gray400"] as Color ?? Colors.Gray,
                    VerticalOptions = LayoutOptions.Center
                };
                cardsArea.Add(cardsPlaceholder);
            }
            content.Add(cardsArea);

            // Bet and Total info
            if (player.IsActive)
            {
                var infoGrid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                    RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto }
                    }
                };

                // Bet display
                var betStack = new VerticalStackLayout
                {
                    Spacing = 2
                };
                betStack.Add(new Label
                {
                    Text = "BET",
                    FontSize = 9,
                    TextColor = Application.Current?.Resources["Gray300"] as Color ?? Colors.LightGray,
                    HorizontalOptions = LayoutOptions.Center
                });
                betStack.Add(new Label
                {
                    Text = "$--",
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.White,
                    HorizontalOptions = LayoutOptions.Center
                });
                infoGrid.Add(betStack, 0, 0);

                // Total display
                var totalStack = new VerticalStackLayout
                {
                    Spacing = 2
                };
                totalStack.Add(new Label
                {
                    Text = "TOTAL",
                    FontSize = 9,
                    TextColor = Application.Current?.Resources["Gray300"] as Color ?? Colors.LightGray,
                    HorizontalOptions = LayoutOptions.Center
                });
                totalStack.Add(new Label
                {
                    Text = "--",
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.White,
                    HorizontalOptions = LayoutOptions.Center
                });
                infoGrid.Add(totalStack, 1, 0);

                content.Add(infoGrid);
            }

            outerBorder.Content = content;
            return outerBorder;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Unsubscribe from ViewModel events
            if (BindingContext is GameTableViewModel viewModel)
            {
                viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
        }
    }
}
