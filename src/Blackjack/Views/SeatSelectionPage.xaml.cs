using Blackjack.Models;
using Blackjack.ViewModels;
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
            var numberLabel = new Label
            {
                Text = seat.PositionLabel,
                FontSize = 36,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var grid = new Grid
            {
                Padding = new Thickness(8)
            };

            grid.Add(numberLabel);

            var border = new Border
            {
                Padding = 0,
                Margin = new Thickness(5),
                HeightRequest = 80,
                WidthRequest = 80,
                StrokeThickness = 3,
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                Content = grid,
                // Store references for easy updates
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
            var numberLabel = (Label)grid.Children[0];

            if (seat.IsPlayer)
            {
                // Player seat: white background, blue border, blue text
                border.BackgroundColor = Colors.White;
                border.Stroke = Application.Current?.Resources["Primary"] as Color ?? Colors.Blue;
                border.StrokeDashArray = null;
                numberLabel.TextColor = Application.Current?.Resources["Primary"] as Color ?? Colors.Blue;
            }
            else if (seat.IsAI)
            {
                // AI seat: blue background, white text
                border.BackgroundColor = Application.Current?.Resources["Secondary"] as Color ?? Colors.Blue;
                border.Stroke = Colors.Transparent;
                border.StrokeDashArray = null;
                numberLabel.TextColor = Colors.White;
            }
            else
            {
                // Empty seat: transparent background, dashed gray border, gray text
                border.BackgroundColor = Colors.Transparent;
                var grayColor = Application.Current?.Resources["Gray400"] as Color ?? Colors.Gray;
                border.Stroke = grayColor;
                border.StrokeDashArray = [2, 2];
                numberLabel.TextColor = grayColor;
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
