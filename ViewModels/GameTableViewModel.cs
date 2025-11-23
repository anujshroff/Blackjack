using Blackjack.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// ViewModel for the Game Table page.
    /// Manages the blackjack game state, player positions, and game actions.
    /// </summary>
    [QueryProperty(nameof(HumanPosition), "HumanPosition")]
    [QueryProperty(nameof(AICount), "AICount")]
    [QueryProperty(nameof(AIPositions), "AIPositions")]
    public partial class GameTableViewModel : ViewModelBase
    {
        private int _humanPosition;
        private int _aiCount;
        private string _aiPositions = "";
        private bool _isInitialized = false;
        private bool _hasReceivedAIPositions = false;

        /// <summary>
        /// Receives the human player position from navigation parameters.
        /// </summary>
        public int HumanPosition
        {
            get => _humanPosition;
            set
            {
                _humanPosition = value;
                TryInitialize();
            }
        }

        /// <summary>
        /// Receives the AI player count from navigation parameters.
        /// </summary>
        public int AICount
        {
            get => _aiCount;
            set
            {
                _aiCount = value;
                TryInitialize();
            }
        }

        /// <summary>
        /// Receives the exact AI player positions from navigation parameters.
        /// </summary>
        public string AIPositions
        {
            get => _aiPositions;
            set
            {
                _aiPositions = value ?? "";
                _hasReceivedAIPositions = true;
                TryInitialize();
            }
        }

        /// <summary>
        /// Attempts to initialize once all navigation parameters are received.
        /// </summary>
        private void TryInitialize()
        {
            // Wait until we have both human position AND AI positions have been set
            if (!_isInitialized && _humanPosition > 0 && _hasReceivedAIPositions)
            {
                _isInitialized = true;
                Initialize(_humanPosition, _aiPositions);
            }
        }

        /// <summary>
        /// The dealer at the table.
        /// </summary>
        [ObservableProperty]
        private Dealer? dealer;

        /// <summary>
        /// Dealer's visible cards.
        /// </summary>
        public ObservableCollection<Card> DealerCards { get; } = [];

        /// <summary>
        /// Indicates if the dealer's hole card should be face-down.
        /// </summary>
        [ObservableProperty]
        private bool dealerHoleCardFaceDown = true;

        /// <summary>
        /// Dealer's hand total value (or "?" if hole card is hidden).
        /// </summary>
        [ObservableProperty]
        private string dealerTotal = "--";

        /// <summary>
        /// Collection of all 7 player positions at the table.
        /// </summary>
        public ObservableCollection<Player> Players { get; } = [];

        /// <summary>
        /// The seat position of the human player (1-7).
        /// </summary>
        [ObservableProperty]
        private int humanPlayerPosition;

        /// <summary>
        /// The number of AI players at the table (0-6).
        /// </summary>
        [ObservableProperty]
        private int aiPlayerCount;

        /// <summary>
        /// Current game phase.
        /// </summary>
        [ObservableProperty]
        private GamePhase currentPhase = GamePhase.Betting;

        /// <summary>
        /// The position of the player whose turn it is (1-7).
        /// </summary>
        [ObservableProperty]
        private int? activePlayerPosition;

        /// <summary>
        /// The human player's current bankroll.
        /// </summary>
        [ObservableProperty]
        private decimal playerBankroll = 5000m;

        /// <summary>
        /// The human player's current bet amount.
        /// </summary>
        [ObservableProperty]
        private decimal currentBet;

        /// <summary>
        /// Game settings including table minimum and maximum bets.
        /// </summary>
        [ObservableProperty]
        private GameSettings settings = new();

        /// <summary>
        /// Indicates if the betting interface should be visible.
        /// </summary>
        [ObservableProperty]
        private bool isBetting = true;

        /// <summary>
        /// Indicates if the Confirm Bet button should be enabled.
        /// </summary>
        [ObservableProperty]
        private bool canConfirmBet;

        /// <summary>
        /// Message to display to the user about game state.
        /// </summary>
        [ObservableProperty]
        private string gameMessage = "Place your bet to begin";

        /// <summary>
        /// Indicates if Hit action is available.
        /// </summary>
        [ObservableProperty]
        private bool canHit;

        /// <summary>
        /// Indicates if Stand action is available.
        /// </summary>
        [ObservableProperty]
        private bool canStand;

        /// <summary>
        /// Indicates if Double Down action is available.
        /// </summary>
        [ObservableProperty]
        private bool canDouble;

        /// <summary>
        /// Indicates if Split action is available.
        /// </summary>
        [ObservableProperty]
        private bool canSplit;

        /// <summary>
        /// Indicates if Insurance action is available.
        /// </summary>
        [ObservableProperty]
        private bool canInsure;

        /// <summary>
        /// Indicates if Even Money action is available.
        /// </summary>
        [ObservableProperty]
        private bool canEvenMoney;

        /// <summary>
        /// Indicates whether the ViewModel has been initialized with navigation parameters.
        /// </summary>
        [ObservableProperty]
        private bool isInitialized;

        public GameTableViewModel()
        {
            Title = "Blackjack Table";

            // Initialize dealer
            Dealer = new Dealer();

            // Initialize 7 empty player positions
            for (int i = 1; i <= 7; i++)
            {
                Players.Add(new Player(
                    name: "",
                    seatPosition: i,
                    startingBankroll: 0m,
                    isHuman: false)
                {
                    IsActive = false
                });
            }
        }

        /// <summary>
        /// Initialize the game table with configuration from seat selection.
        /// </summary>
        public void Initialize(int humanPosition, string aiPositionsString)
        {
            HumanPlayerPosition = humanPosition;

            // Parse AI positions from comma-separated string
            var aiPositionsList = new List<int>();
            if (!string.IsNullOrEmpty(aiPositionsString))
            {
                aiPositionsList = [.. aiPositionsString
                    .Split(',')
                    .Where(s => int.TryParse(s, out _))
                    .Select(int.Parse)];
            }

            AiPlayerCount = aiPositionsList.Count;

            // Clear all players first
            foreach (var p in Players)
            {
                p.IsActive = false;
                p.Name = "";
                p.IsHuman = false;
                p.Bankroll = 0m;
            }

            // Mark the human player position
            var humanPlayer = Players[humanPosition - 1];
            humanPlayer.Name = "You";
            humanPlayer.IsHuman = true;
            humanPlayer.IsActive = true;
            humanPlayer.Bankroll = PlayerBankroll;

            // Mark AI player positions at exact seats
            int aiNumber = 1;
            foreach (var aiPosition in aiPositionsList)
            {
                var aiPlayer = Players[aiPosition - 1];
                aiPlayer.Name = $"AI Player {aiNumber}";
                aiPlayer.IsHuman = false;
                aiPlayer.IsActive = true;
                aiPlayer.Bankroll = 5000m; // AI starts with same bankroll
                aiNumber++;
            }

            GameMessage = $"Welcome! You're at Position {humanPosition}. Place your bet to begin.";

            // Signal that initialization is complete
            IsInitialized = true;

            // Add test cards for dealer (for testing card display)
            AddTestDealerCards();
        }

        /// <summary>
        /// Adds test cards to dealer for testing card display.
        /// This will be replaced with actual game logic in Phase 3.
        /// </summary>
        private void AddTestDealerCards()
        {
            // Clear any existing cards
            DealerCards.Clear();

            // Add a visible Ace of Spades
            DealerCards.Add(new Card(Suit.Spades, Rank.Ace));

            // Add a face-down hole card (King of Hearts)
            DealerCards.Add(new Card(Suit.Hearts, Rank.King));

            // Show "?" for total since hole card is hidden
            DealerTotal = "?";
            DealerHoleCardFaceDown = true;
        }

        /// <summary>
        /// Command to request an additional card (Hit).
        /// Phase 3 will implement the actual logic.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanHit))]
        private async Task Hit()
        {
            await Shell.Current.DisplayAlertAsync("Coming Soon",
                "Hit action will be implemented in Phase 3: Game Flow",
                "OK");
        }

        /// <summary>
        /// Command to keep current hand and end turn (Stand).
        /// Phase 3 will implement the actual logic.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanStand))]
        private async Task Stand()
        {
            await Shell.Current.DisplayAlertAsync("Coming Soon",
                "Stand action will be implemented in Phase 3: Game Flow",
                "OK");
        }

        /// <summary>
        /// Command to double the bet and receive one more card (Double Down).
        /// Phase 3 will implement the actual logic.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanDouble))]
        private async Task DoubleDown()
        {
            await Shell.Current.DisplayAlertAsync("Coming Soon",
                "Double Down action will be implemented in Phase 3: Game Flow",
                "OK");
        }

        /// <summary>
        /// Command to split a pair into two hands (Split).
        /// Phase 3 will implement the actual logic.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanSplit))]
        private async Task Split()
        {
            await Shell.Current.DisplayAlertAsync("Coming Soon",
                "Split action will be implemented in Phase 3: Game Flow",
                "OK");
        }

        /// <summary>
        /// Command to take insurance against dealer blackjack.
        /// Phase 3 will implement the actual logic.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanInsure))]
        private async Task Insurance()
        {
            await Shell.Current.DisplayAlertAsync("Coming Soon",
                "Insurance action will be implemented in Phase 3: Game Flow",
                "OK");
        }

        /// <summary>
        /// Command to take even money when player has blackjack and dealer shows Ace.
        /// Phase 3 will implement the actual logic.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanEvenMoney))]
        private async Task EvenMoney()
        {
            await Shell.Current.DisplayAlertAsync("Coming Soon",
                "Even Money action will be implemented in Phase 3: Game Flow",
                "OK");
        }

        /// <summary>
        /// Command to add a chip to the current bet.
        /// </summary>
        [RelayCommand]
        private void AddChip(object parameter)
        {
            // Convert parameter to decimal
            if (parameter == null || !decimal.TryParse(parameter.ToString(), out decimal amount))
            {
                GameMessage = "Invalid chip amount";
                return;
            }

            // Validate that adding this chip won't exceed limits
            var newBet = CurrentBet + amount;

            if (newBet > PlayerBankroll)
            {
                GameMessage = $"Insufficient funds! You have ${PlayerBankroll:N0}";
                return;
            }

            if (newBet > Settings.TableMaximum)
            {
                GameMessage = $"Maximum bet is ${Settings.TableMaximum:N0}";
                return;
            }

            // Add chip to bet
            CurrentBet = newBet;
            GameMessage = $"Current bet: ${CurrentBet:N0}";

            // Update CanConfirmBet based on table minimum
            CanConfirmBet = CurrentBet >= Settings.TableMinimum && CurrentBet <= Settings.TableMaximum;
        }

        /// <summary>
        /// Command to clear the current bet.
        /// </summary>
        [RelayCommand]
        private void ClearBet()
        {
            CurrentBet = 0;
            CanConfirmBet = false;
            GameMessage = "Place your bet to begin";
        }

        /// <summary>
        /// Command to confirm the bet and start the round.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanConfirmBet))]
        private async Task ConfirmBet()
        {
            // Validate bet one more time
            if (CurrentBet < Settings.TableMinimum)
            {
                GameMessage = $"Minimum bet is ${Settings.TableMinimum:N0}";
                return;
            }

            if (CurrentBet > Settings.TableMaximum)
            {
                GameMessage = $"Maximum bet is ${Settings.TableMaximum:N0}";
                return;
            }

            if (CurrentBet > PlayerBankroll)
            {
                GameMessage = "Insufficient funds!";
                return;
            }

            // Deduct bet from bankroll
            PlayerBankroll -= CurrentBet;

            // Update player's bankroll in the model
            var humanPlayer = Players[HumanPlayerPosition - 1];
            humanPlayer.Bankroll = PlayerBankroll;

            // Transition to dealing phase
            CurrentPhase = GamePhase.Dealing;
            IsBetting = false;
            GameMessage = $"Bet confirmed: ${CurrentBet:N0}. Dealing cards...";

            // Placeholder for Phase 3: Deal cards
            await Task.Delay(1000); // Simulate dealing delay
            GameMessage = "Card dealing will be implemented in Phase 3: Game Flow";
        }

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
