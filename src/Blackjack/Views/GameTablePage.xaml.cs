using Blackjack.Models;
using Blackjack.ViewModels;
using FluentIcons.Maui;
using Microsoft.Maui.Controls.Shapes;

namespace Blackjack.Views
{
    public partial class GameTablePage : ContentPage
    {
        private GameTableViewModel ViewModel => (GameTableViewModel)BindingContext;

        // Get screen dimensions for dynamic sizing
        private static double ScreenHeight => DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density;
        private static double ScreenWidth => DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density;

        // Dynamic card sizing methods based on container context
        private static (double width, double height) GetCardSizeForDealerArea()
        {
            // Dealer area is in Row 0 (35% of screen height)
            // Account for padding, margins, labels, etc.
            double availableHeight = GameTablePage.ScreenHeight * 0.35 * 0.5; // Use 50% of row height for cards
            double maxHeight = Math.Min(availableHeight, 100); // Cap at reasonable size
            double height = Math.Max(60, maxHeight); // Minimum 60px height
            double width = height / 1.4; // Maintain card aspect ratio
            return (width, height);
        }

        private static (double width, double height) GetCardSizeForActiveHandArea()
        {
            // Active hand area is in Row 0 (35% of screen height)
            // Similar to dealer area but might have slightly different constraints
            double availableHeight = GameTablePage.ScreenHeight * 0.35 * 0.5; // Use 50% of row height for cards
            double maxHeight = Math.Min(availableHeight, 100); // Cap at reasonable size
            double height = Math.Max(60, maxHeight); // Minimum 60px height
            double width = height / 1.4; // Maintain card aspect ratio
            return (width, height);
        }

        private static (double width, double height) GetCardSizeForSummaryStrip()
        {
            // Summary strip is in Row 1 (25% of screen height)
            // Cards should be smaller here
            double availableHeight = GameTablePage.ScreenHeight * 0.25 * 0.6; // Use 60% of row height
            double maxHeight = Math.Min(availableHeight, 70); // Smaller cap for summary cards
            double height = Math.Max(45, maxHeight); // Minimum 45px height
            double width = height / 1.4; // Maintain card aspect ratio
            return (width, height);
        }

        private static double GetCardOverlap(double cardWidth)
        {
            return cardWidth * 0.5; // 50% overlap for better space usage
        }

        public GameTablePage(GameTableViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            // Set dynamic heights for ScrollViews
            UpdateScrollViewHeights();

            // Subscribe to ViewModel property changes
            viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Wire up Return to Active button
            ReturnToActiveButton.Clicked += (s, e) => ViewModel.ReturnToActivePlayer();
        }

        /// <summary>
        /// Update ScrollView heights based on dynamic card sizing
        /// </summary>
        private void UpdateScrollViewHeights()
        {
            // Set dealer ScrollView height to accommodate full card with some padding
            var (_, height) = GameTablePage.GetCardSizeForDealerArea();
            DealerCardsScrollView?.HeightRequest = height + 10; // Add padding
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // When ViewModel is initialized, build initial UI
            if (e.PropertyName == nameof(GameTableViewModel.IsInitialized) && ViewModel.IsInitialized)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    BuildPlayerSummaryStrip();
                    BuildActiveHandDisplay();
                    BuildDealerCards();
                    UpdateLayoutForPhase();
                });
            }
            // When phase changes, rebuild everything
            else if (e.PropertyName == nameof(GameTableViewModel.CurrentPhase))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    BuildPlayerSummaryStrip();
                    BuildActiveHandDisplay();
                    BuildDealerCards();
                    UpdateLayoutForPhase();
                });
            }
            // When dealer cards change
            else if (e.PropertyName == nameof(GameTableViewModel.DealerCards))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    BuildDealerCards();
                });
            }
            // When dealer hole card is revealed/hidden
            else if (e.PropertyName == nameof(GameTableViewModel.DealerHoleCardFaceDown))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    BuildDealerCards();
                });
            }
            // When Players property changes (cards dealt, etc.)
            else if (e.PropertyName == nameof(GameTableViewModel.Players))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    BuildPlayerSummaryStrip();
                    BuildActiveHandDisplay();
                });
            }
            // When active player position changes
            else if (e.PropertyName == nameof(GameTableViewModel.ActivePlayerPosition))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Auto-view the active player when it changes
                    if (ViewModel.ActivePlayerPosition.HasValue)
                    {
                        ViewModel.ViewPlayer(ViewModel.ActivePlayerPosition.Value);
                    }
                    BuildPlayerSummaryStrip();
                    BuildActiveHandDisplay();
                });
            }
            // When viewed player position changes
            else if (e.PropertyName == nameof(GameTableViewModel.ViewedPlayerPosition) ||
                     e.PropertyName == nameof(GameTableViewModel.ViewedHandIndex))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    BuildActiveHandDisplay();

                    // Update Return to Active button visibility
                    ReturnToActiveButton.IsVisible = ViewModel.IsViewingInactivePlayer;
                });
            }
        }

        /// <summary>
        /// Update layout based on game phase (expand dealer area during dealer turn).
        /// </summary>
        private void UpdateLayoutForPhase()
        {
            // During dealer's turn, expand dealer area to full width
            bool isDealerTurn = ViewModel.CurrentPhase == GamePhase.DealerAction;

            if (isDealerTurn)
            {
                // Expand dealer area to use both columns
                Grid.SetColumnSpan(DealerAreaBorder, 2);
                ActiveHandAreaBorder.IsVisible = false;
            }
            else
            {
                // Normal layout: dealer on left, active hand on right
                Grid.SetColumnSpan(DealerAreaBorder, 1);
                ActiveHandAreaBorder.IsVisible = true;
            }
        }

        /// <summary>
        /// Build the dealer's card display.
        /// </summary>
        private void BuildDealerCards()
        {
            DealerCardsContainer.Children.Clear();

            if (ViewModel.DealerCards.Count == 0)
                return;

            var cardConverter = new Converters.CardToImageConverter();
            var (cardWidth, cardHeight) = GameTablePage.GetCardSizeForDealerArea();

            for (int i = 0; i < ViewModel.DealerCards.Count; i++)
            {
                var card = ViewModel.DealerCards[i];

                var cardBorder = new Border
                {
                    StrokeThickness = 1,
                    Stroke = Colors.Black,
                    WidthRequest = cardWidth,
                    HeightRequest = cardHeight,
                    BackgroundColor = Colors.White,
                    StrokeShape = new RoundRectangle { CornerRadius = 4 }
                };

                ImageSource? imageSource;

                // Show card_back for hole card (index 1) when face-down
                if (i == 1 && ViewModel.DealerHoleCardFaceDown)
                {
                    imageSource = ImageSource.FromFile("card_back.png");
                }
                else
                {
                    imageSource = cardConverter.Convert(card, typeof(ImageSource), null!,
                        System.Globalization.CultureInfo.CurrentCulture) as ImageSource;
                }

                var cardImage = new Image
                {
                    Source = imageSource,
                    Aspect = Aspect.AspectFit
                };

                cardBorder.Content = cardImage;
                DealerCardsContainer.Children.Add(cardBorder);
            }

            // Add total badge when hole card is revealed
            if (!ViewModel.DealerHoleCardFaceDown)
            {
                var totalBadge = new Border
                {
                    BackgroundColor = Color.FromArgb("#D4AF37"),
                    StrokeThickness = 0,
                    Padding = new Thickness(10, 6),
                    StrokeShape = new RoundRectangle { CornerRadius = 8 },
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(15, 0, 0, 0),
                    Content = new Label
                    {
                        Text = ViewModel.DealerTotal,
                        FontSize = 18,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.Black,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                };
                DealerCardsContainer.Children.Add(totalBadge);
            }
        }

        /// <summary>
        /// Build the player summary strip showing all 7 seat positions.
        /// </summary>
        private void BuildPlayerSummaryStrip()
        {
            PlayerSummaryStrip.Children.Clear();

            // Show all 7 seat positions (1-7)
            for (int seatPosition = 1; seatPosition <= 7; seatPosition++)
            {
                var player = ViewModel.Players[seatPosition - 1];
                Border summaryCard;

                if (player.IsActive)
                {
                    // Active player - show full summary
                    summaryCard = CreatePlayerSummaryCard(player);
                }
                else
                {
                    // Vacant seat - show placeholder
                    summaryCard = CreateVacantSeatCard(seatPosition);
                }

                // Set the column position in the Grid
                Grid.SetColumn(summaryCard, seatPosition - 1);
                PlayerSummaryStrip.Children.Add(summaryCard);
            }
        }

        /// <summary>
        /// Create a placeholder card for a vacant seat.
        /// </summary>
        private static Border CreateVacantSeatCard(int seatPosition)
        {
            var vacantBorder = new Border
            {
                BackgroundColor = Color.FromArgb("#10FFFFFF"),
                Stroke = Colors.Transparent,
                StrokeThickness = 0,
                Padding = 3,
                StrokeShape = new RoundRectangle { CornerRadius = 6 },
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center
            };

            var content = new VerticalStackLayout
            {
                Spacing = 2,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            // Position label
            content.Add(new Label
            {
                Text = $"P{seatPosition}",
                FontSize = 9,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#4A5568"),
                HorizontalOptions = LayoutOptions.Center
            });

            // Empty seat icon
            var seatIcon = new FluentIcon
            {
                Icon = (FluentIcons.Common.Icon)FluentIcons.Common.Symbol.PersonAvailable,
                IconVariant = FluentIcons.Common.IconVariant.Regular,
                FontSize = 12,
                ForegroundColor = Color.FromArgb("#4A5568")
            };
            content.Add(seatIcon);

            // Vacant text
            content.Add(new Label
            {
                Text = "VACANT",
                FontSize = 8,
                TextColor = Color.FromArgb("#4A5568"),
                HorizontalOptions = LayoutOptions.Center
            });

            vacantBorder.Content = content;

            return vacantBorder;
        }

        /// <summary>
        /// Create a compact summary card for a player.
        /// </summary>
        private Border CreatePlayerSummaryCard(Player player)
        {
            // Determine colors based on player state
            var borderColor = Colors.Transparent;
            var backgroundColor = Color.FromArgb("#1AFFFFFF");

            // Highlight active player's turn
            if (player.SeatPosition == ViewModel.ActivePlayerPosition)
            {
                borderColor = Color.FromArgb("#10B981"); // Green for active turn
                backgroundColor = Color.FromArgb("#2010B981");
            }
            // Highlight viewed player
            else if (player.SeatPosition == ViewModel.ViewedPlayerPosition)
            {
                borderColor = Color.FromArgb("#F59E0B"); // Orange for viewed
                backgroundColor = Color.FromArgb("#20F59E0B");
            }

            // Determine status text and color
            var statusText = "";
            var statusColor = Colors.White;

            if (player.Hands.Count > 0 && player.Hands[0].Cards.Count > 0)
            {
                var hand = player.Hands[0];
                if (hand.IsBusted)
                {
                    statusText = "BUST";
                    statusColor = Color.FromArgb("#EF4444"); // Red
                }
                else if (hand.IsBlackjack)
                {
                    statusText = "BJ!";
                    statusColor = Color.FromArgb("#FFD700"); // Gold
                }
                else if (hand.Status == HandStatus.Standing)
                {
                    statusText = hand.TotalValue.ToString();
                    statusColor = Colors.White;
                }
                else if (hand.Cards.Count > 0)
                {
                    statusText = hand.TotalValue.ToString();
                    statusColor = Colors.White;
                }
            }

            var summaryBorder = new Border
            {
                BackgroundColor = backgroundColor,
                Stroke = borderColor,
                StrokeThickness = borderColor == Colors.Transparent ? 0 : 2,
                Padding = 3,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                StrokeShape = new RoundRectangle { CornerRadius = 6 }
            };

            var content = new VerticalStackLayout
            {
                Spacing = 2
            };

            // Position label with split indicator
            var positionText = player.Hands.Count > 1
                ? $"P{player.SeatPosition} ({player.Hands.Count}H)"
                : $"P{player.SeatPosition}";

            content.Add(new Label
            {
                Text = positionText,
                FontSize = 9,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#D4AF37"),
                HorizontalOptions = LayoutOptions.Center
            });

            // Player icon and name
            var iconStack = new HorizontalStackLayout
            {
                Spacing = 2,
                HorizontalOptions = LayoutOptions.Center
            };

            var playerIcon = new FluentIcon
            {
                Icon = (FluentIcons.Common.Icon)FluentIcons.Common.Symbol.Person,
                IconVariant = player.IsHuman ?
                    FluentIcons.Common.IconVariant.Filled :
                    FluentIcons.Common.IconVariant.Regular,
                FontSize = 12,
                ForegroundColor = player.IsHuman ?
                    Color.FromArgb("#3B82F6") :
                    Color.FromArgb("#64748B")
            };
            iconStack.Add(playerIcon);
            content.Add(iconStack);

            // Total bet amount across all hands
            var totalBet = player.Hands.Sum(h => h.Bet);
            if (totalBet > 0)
            {
                content.Add(new Label
                {
                    Text = $"${totalBet:N0}",
                    FontSize = 10,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#10B981"),
                    HorizontalOptions = LayoutOptions.Center
                });
            }

            // Status - show all hand totals for splits, or single status
            if (player.Hands.Count > 1 && player.Hands.Any(h => h.Cards.Count > 0))
            {
                // Multiple hands - show each status compactly
                var statusStack = new VerticalStackLayout
                {
                    Spacing = 1,
                    HorizontalOptions = LayoutOptions.Center
                };

                foreach (var hand in player.Hands)
                {
                    if (hand.Cards.Count == 0)
                        continue;

                    string handStatus;
                    Color handColor;

                    if (hand.NeedsSecondCard)
                    {
                        handStatus = "?";
                        handColor = Color.FromArgb("#6B7280");
                    }
                    else if (hand.IsBusted)
                    {
                        handStatus = "B";
                        handColor = Color.FromArgb("#EF4444");
                    }
                    else if (hand.IsBlackjack)
                    {
                        handStatus = "BJ";
                        handColor = Color.FromArgb("#FFD700");
                    }
                    else if (hand.Status == HandStatus.Standing)
                    {
                        handStatus = hand.TotalValue.ToString();
                        handColor = Colors.White;
                    }
                    else
                    {
                        handStatus = hand.TotalValue.ToString();
                        handColor = Colors.White;
                    }

                    statusStack.Add(new Label
                    {
                        Text = handStatus,
                        FontSize = 9,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = handColor,
                        HorizontalOptions = LayoutOptions.Center
                    });
                }

                content.Add(statusStack);
            }
            else if (!string.IsNullOrEmpty(statusText))
            {
                // Single hand - show normal status
                content.Add(new Label
                {
                    Text = statusText,
                    FontSize = 11,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = statusColor,
                    HorizontalOptions = LayoutOptions.Center
                });
            }

            summaryBorder.Content = content;

            // Add tap gesture to view this player
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => ViewModel.ViewPlayer(player.SeatPosition);
            summaryBorder.GestureRecognizers.Add(tapGesture);

            return summaryBorder;
        }

        /// <summary>
        /// Build the active hand display showing the currently viewed player's hand.
        /// </summary>
        private void BuildActiveHandDisplay()
        {
            ActiveHandArea.Children.Clear();

            // Don't show active hand during dealer's turn
            if (ViewModel.CurrentPhase == GamePhase.DealerAction)
                return;

            // Keep the Return to Active button
            if (ReturnToActiveButton.Parent == null)
            {
                ActiveHandArea.Children.Add(ReturnToActiveButton);
            }

            // Find the player to display
            Player? viewedPlayer = null;
            if (ViewModel.ViewedPlayerPosition.HasValue)
            {
                viewedPlayer = ViewModel.Players.FirstOrDefault(p =>
                    p.SeatPosition == ViewModel.ViewedPlayerPosition.Value);
            }
            else if (ViewModel.ActivePlayerPosition.HasValue)
            {
                viewedPlayer = ViewModel.Players.FirstOrDefault(p =>
                    p.SeatPosition == ViewModel.ActivePlayerPosition.Value);
            }

            if (viewedPlayer == null || !viewedPlayer.IsActive)
                return;

            var handDisplay = CreateActiveHandDisplay(viewedPlayer);
            ActiveHandArea.Children.Insert(0, handDisplay); // Insert before Return button
        }

        /// <summary>
        /// Create the large, focused display for the active/viewed player's hand.
        /// </summary>
        private VerticalStackLayout CreateActiveHandDisplay(Player player)
        {
            var content = new VerticalStackLayout
            {
                Spacing = 5,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            // Player info header - more compact
            var headerStack = new HorizontalStackLayout
            {
                Spacing = 4,
                HorizontalOptions = LayoutOptions.Center
            };

            var playerIcon = new FluentIcon
            {
                Icon = (FluentIcons.Common.Icon)FluentIcons.Common.Symbol.Person,
                IconVariant = player.IsHuman ?
                    FluentIcons.Common.IconVariant.Filled :
                    FluentIcons.Common.IconVariant.Regular,
                FontSize = 16,
                ForegroundColor = player.IsHuman ?
                    Color.FromArgb("#3B82F6") :
                    Color.FromArgb("#64748B")
            };
            headerStack.Add(playerIcon);

            headerStack.Add(new Label
            {
                Text = $"{player.Name} (P{player.SeatPosition})",
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                VerticalOptions = LayoutOptions.Center
            });

            content.Add(headerStack);

            // Handle multiple hands (splits) - more compact
            if (player.Hands.Count > 1)
            {
                // Add hand tabs
                var tabsStack = new HorizontalStackLayout
                {
                    Spacing = 5,
                    HorizontalOptions = LayoutOptions.Center
                };

                for (int i = 0; i < player.Hands.Count; i++)
                {
                    var handIndex = i;
                    var hand = player.Hands[i];

                    // Determine tab color and status based on hand state
                    Color backgroundColor;
                    Color strokeColor;
                    string statusText;

                    // Check if this is the currently active hand for play
                    bool isActivePlayHand = player.SeatPosition == ViewModel.ActivePlayerPosition &&
                                           handIndex == ViewModel.ViewedHandIndex &&
                                           hand.Status == HandStatus.Active;

                    if (isActivePlayHand)
                    {
                        // Green for currently active hand being played
                        backgroundColor = Color.FromArgb("#10B981");
                        strokeColor = Color.FromArgb("#059669");
                        statusText = "ACTIVE";
                    }
                    else if (hand.NeedsSecondCard)
                    {
                        // Gray for waiting hand (needs second card)
                        backgroundColor = Color.FromArgb("#6B7280");
                        strokeColor = Color.FromArgb("#4B5563");
                        statusText = "WAIT";
                    }
                    else if (hand.Status == HandStatus.Busted)
                    {
                        // Red for busted
                        backgroundColor = Color.FromArgb("#EF4444");
                        strokeColor = Color.FromArgb("#DC2626");
                        statusText = "BUST";
                    }
                    else if (hand.Status == HandStatus.Standing)
                    {
                        // Blue for standing
                        backgroundColor = Color.FromArgb("#3B82F6");
                        strokeColor = Color.FromArgb("#2563EB");
                        statusText = hand.TotalValue.ToString();
                    }
                    else if (handIndex == ViewModel.ViewedHandIndex)
                    {
                        // Orange for viewed but not active
                        backgroundColor = Color.FromArgb("#F59E0B");
                        strokeColor = Color.FromArgb("#D97706");
                        statusText = $"H{handIndex + 1}";
                    }
                    else
                    {
                        // Default
                        backgroundColor = Color.FromArgb("#40FFFFFF");
                        strokeColor = Colors.Transparent;
                        statusText = $"H{handIndex + 1}";
                    }

                    var tabBorder = new Border
                    {
                        BackgroundColor = backgroundColor,
                        Stroke = strokeColor,
                        StrokeThickness = 2,
                        Padding = new Thickness(8, 3),
                        StrokeShape = new RoundRectangle { CornerRadius = 4 }
                    };

                    var tabContent = new VerticalStackLayout
                    {
                        Spacing = 1
                    };

                    tabContent.Add(new Label
                    {
                        Text = $"H{handIndex + 1}",
                        FontSize = 9,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Center
                    });

                    tabContent.Add(new Label
                    {
                        Text = statusText,
                        FontSize = 8,
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Center
                    });

                    tabBorder.Content = tabContent;

                    var tapGesture = new TapGestureRecognizer();
                    tapGesture.Tapped += (s, e) => ViewModel.ViewHand(handIndex);
                    tabBorder.GestureRecognizers.Add(tapGesture);

                    tabsStack.Add(tabBorder);
                }

                content.Add(tabsStack);
            }

            // Cards display for current hand
            var handToDisplay = player.Hands.Count > ViewModel.ViewedHandIndex ?
                player.Hands[ViewModel.ViewedHandIndex] :
                (player.Hands.Count > 0 ? player.Hands[0] : null);

            if (handToDisplay != null && handToDisplay.Cards.Count > 0)
            {
                var (cardWidth, cardHeight) = GameTablePage.GetCardSizeForActiveHandArea();
                var cardOverlap = GameTablePage.GetCardOverlap(cardWidth);

                var cardsStack = new HorizontalStackLayout
                {
                    Spacing = -cardOverlap,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                var cardConverter = new Converters.CardToImageConverter();
                foreach (var card in handToDisplay.Cards)
                {
                    var cardImage = new Image
                    {
                        Source = cardConverter.Convert(card, typeof(ImageSource), null!,
                            System.Globalization.CultureInfo.CurrentCulture) as ImageSource,
                        Aspect = Aspect.AspectFit
                    };

                    var cardBorder = new Border
                    {
                        StrokeThickness = 1,
                        Stroke = Colors.Black,
                        WidthRequest = cardWidth,
                        HeightRequest = cardHeight,
                        BackgroundColor = Colors.White,
                        StrokeShape = new RoundRectangle { CornerRadius = 4 },
                        Content = cardImage
                    };

                    cardsStack.Add(cardBorder);
                }

                // Wrap cards in ScrollView for overflow handling
                var cardsScrollView = new ScrollView
                {
                    Orientation = ScrollOrientation.Horizontal,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                    HeightRequest = cardHeight + 10, // Dynamic height with padding
                    Content = cardsStack
                };

                content.Add(cardsScrollView);

                // Hand info (bet, total, status) - more compact
                var infoStack = new HorizontalStackLayout
                {
                    Spacing = 15,
                    HorizontalOptions = LayoutOptions.Center
                };

                // Bet info
                infoStack.Add(new Label
                {
                    Text = $"Bet: ${handToDisplay.Bet:N0}",
                    FontSize = 11,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#10B981"),
                    VerticalOptions = LayoutOptions.Center
                });

                // Total info
                var totalText = $"Total: {handToDisplay.TotalValue}";
                var totalColor = Colors.White;

                if (handToDisplay.IsBusted)
                {
                    totalText = "BUST!";
                    totalColor = Color.FromArgb("#EF4444");
                }
                else if (handToDisplay.IsBlackjack)
                {
                    totalText = "BLACKJACK!";
                    totalColor = Color.FromArgb("#FFD700");
                }

                infoStack.Add(new Label
                {
                    Text = totalText,
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = totalColor,
                    VerticalOptions = LayoutOptions.Center
                });

                content.Add(infoStack);
            }
            else
            {
                // No cards yet
                content.Add(new Label
                {
                    Text = "Waiting for cards...",
                    FontSize = 14,
                    FontAttributes = FontAttributes.Italic,
                    TextColor = Color.FromArgb("#80FFFFFF"),
                    HorizontalOptions = LayoutOptions.Center
                });
            }

            return content;
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
