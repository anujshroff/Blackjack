using Blackjack.Models;
using Blackjack.ViewModels;
using FluentIcons.Maui;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.Specialized;

namespace Blackjack.Views
{
    public partial class SeatSelectionPage : ContentPage
    {
        private SeatSelectionViewModel ViewModel => (SeatSelectionViewModel)BindingContext;

        public SeatSelectionPage(SeatSelectionViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            // Build seat UI when page loads
            Loaded += OnPageLoaded;
        }

        private void OnPageLoaded(object? sender, EventArgs e)
        {
            BuildSeatUI();

            // Subscribe to collection changes to rebuild UI when seats change
            ViewModel.Seats.CollectionChanged += OnSeatsCollectionChanged;
        }

        private void OnSeatsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            BuildSeatUI();
        }

        private void BuildSeatUI()
        {
            SeatsContainer.Children.Clear();

            foreach (var seat in ViewModel.Seats)
            {
                var seatBorder = CreateSeatBorder(seat);
                SeatsContainer.Children.Add(seatBorder);

                // Subscribe to property changes for this seat
                seat.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(SeatInfo.IsPlayer) ||
                        e.PropertyName == nameof(SeatInfo.IsAI))
                    {
                        SeatSelectionPage.UpdateSeatVisuals(seatBorder, seat);
                    }
                };
            }
        }

        private Border CreateSeatBorder(SeatInfo seat)
        {
            var icon = new FluentIcon
            {
                Icon = (FluentIcons.Common.Icon)FluentIcons.Common.Symbol.Person,
                IconVariant = FluentIcons.Common.IconVariant.Filled,
                FontSize = 32,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var emptyIcon = new FluentIcon
            {
                Icon = (FluentIcons.Common.Icon)FluentIcons.Common.Symbol.PersonCircle,
                IconVariant = FluentIcons.Common.IconVariant.Regular,
                FontSize = 32,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Opacity = 0.5
            };

            var label = new Label
            {
                Text = seat.PositionLabel,
                FontSize = 11,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 4, 0, 0)
            };

            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                Padding = new Thickness(8)
            };

            grid.Add(icon, 0, 1);
            grid.Add(emptyIcon, 0, 1);
            grid.Add(label, 0, 2);

            var border = new Border
            {
                Padding = 0,
                Margin = new Thickness(5),
                HeightRequest = 100,
                WidthRequest = 90,
                StrokeThickness = 3,
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Content = grid,
                // Store references to icons and label for easy updates
                ClassId = seat.SeatNumber.ToString()
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => ViewModel.SelectSeatCommand.Execute(seat.SeatNumber);
            border.GestureRecognizers.Add(tapGesture);

            // Set initial visuals
            SeatSelectionPage.UpdateSeatVisuals(border, seat);

            return border;
        }

        private static void UpdateSeatVisuals(Border border, SeatInfo seat)
        {
            var grid = (Grid)border.Content!;
            var icon = (FluentIcon)grid.Children[0];
            var emptyIcon = (FluentIcon)grid.Children[1];
            var label = (Label)grid.Children[2];

            if (seat.IsPlayer)
            {
                // Player seat: white background, blue border, blue icon
                border.BackgroundColor = Colors.White;
                border.Stroke = Application.Current?.Resources["Primary"] as Color ?? Colors.Blue;
                icon.ForegroundColor = Application.Current?.Resources["Primary"] as Color ?? Colors.Blue;
                icon.IsVisible = true;
                emptyIcon.IsVisible = false;
                label.TextColor = Application.Current?.Resources["Gray900"] as Color ?? Colors.Black;
            }
            else if (seat.IsAI)
            {
                // AI seat: blue background, white icon
                border.BackgroundColor = Application.Current?.Resources["Secondary"] as Color ?? Colors.Blue;
                border.Stroke = Colors.Transparent;
                icon.ForegroundColor = Colors.White;
                icon.IsVisible = true;
                emptyIcon.IsVisible = false;
                label.TextColor = Colors.White;
            }
            else
            {
                // Empty seat: transparent background, dashed gray border, gray icon
                border.BackgroundColor = Colors.Transparent;
                var grayColor = Application.Current?.Resources["Gray400"] as Color ?? Colors.Gray;
                border.Stroke = grayColor;
                border.StrokeDashArray = [2, 2];
                emptyIcon.ForegroundColor = grayColor;
                icon.IsVisible = false;
                emptyIcon.IsVisible = true;
                label.TextColor = grayColor;
            }
        }

        private void OnDecreaseAI(object sender, EventArgs e)
        {
            if (ViewModel.AiPlayerCount > 0)
            {
                ViewModel.AiPlayerCount--;
            }
        }

        private void OnIncreaseAI(object sender, EventArgs e)
        {
            // Maximum 6 AI players (7 total seats - 1 for player)
            if (ViewModel.AiPlayerCount < 6)
            {
                ViewModel.AiPlayerCount++;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Unsubscribe from events
            ViewModel.Seats.CollectionChanged -= OnSeatsCollectionChanged;
            Loaded -= OnPageLoaded;
        }
    }
}
