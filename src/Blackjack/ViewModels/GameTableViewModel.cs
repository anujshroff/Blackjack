using Blackjack.Models;
using Blackjack.Services;
using CommunityToolkit.Mvvm.ComponentModel;
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
        private decimal playerBankroll;

        /// <summary>
        /// The human player's current bet amount.
        /// </summary>
        [ObservableProperty]
        private decimal currentBet;

        /// <summary>
        /// Game settings including table minimum and maximum bets.
        /// </summary>
        [ObservableProperty]
        private GameSettings settings = new()
        {
            TableMinimum = SettingsService.LoadTableMinimum(),
            TableMaximum = SettingsService.LoadTableMaximum(),
            StartingBankroll = SettingsService.LoadStartingBankroll(),
            NumberOfDecks = SettingsService.LoadNumberOfDecks()
        };

        /// <summary>
        /// AI betting service for generating AI bets.
        /// </summary>
        private Services.AIBettingService? _aiBettingService;

        /// <summary>
        /// Game rules service for validating actions and calculating payouts.
        /// </summary>
        private Services.GameRules? _gameRules;

        /// <summary>
        /// The 6-deck shoe used for dealing cards.
        /// </summary>
        private Deck? _deck;

        /// <summary>
        /// Tracks insurance bets per player (seat position -> insurance bet amount).
        /// Insurance is only available on initial hand before any player actions.
        /// </summary>
        private readonly Dictionary<int, decimal> _insuranceBets = [];

        /// <summary>
        /// Tracks which players have made their insurance decision.
        /// </summary>
        private readonly HashSet<int> _playersDecidedInsurance = [];

        /// <summary>
        /// Indicates if we're waiting for the human player to decide on insurance/even money.
        /// </summary>
        private bool _awaitingHumanInsuranceDecision = false;

        /// <summary>
        /// Tracks the index of the current hand being played (for split hands).
        /// </summary>
        private int _currentHandIndex = 0;

        /// <summary>
        /// Basic Strategy service for AI decision-making.
        /// </summary>
        private Services.BasicStrategy? _basicStrategy;

        /// <summary>
        /// Bankroll service for persisting player bankroll.
        /// </summary>
        private readonly BankrollService _bankrollService;

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

        /// <summary>
        /// The position of the player currently being viewed (1-7).
        /// Used to show any player's cards when tapped from summary strip.
        /// </summary>
        [ObservableProperty]
        private int? viewedPlayerPosition;

        /// <summary>
        /// Index of the currently viewed hand for split hands (0-based).
        /// </summary>
        [ObservableProperty]
        private int viewedHandIndex = 0;

        /// <summary>
        /// Indicates if we're viewing a non-active player.
        /// Used to show "Return to Active" button.
        /// </summary>
        [ObservableProperty]
        private bool isViewingInactivePlayer;

        public GameTableViewModel(BankrollService bankrollService)
        {
            Title = "Blackjack Table";
            _bankrollService = bankrollService;

            // Initialize dealer with soft 17 rule from settings
            Dealer = new Dealer(Settings.DealerHitsSoft17);

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

            // Load bankroll from persistence (or use default if none saved)
            PlayerBankroll = BankrollService.LoadBankroll(Settings.StartingBankroll);

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
                aiPlayer.Bankroll = Settings.StartingBankroll;
                aiNumber++;
            }

            GameMessage = $"Welcome! You're at Position {humanPosition}. Place your bet to begin.";

            // Initialize AI betting service
            _aiBettingService = new Services.AIBettingService(Settings);

            // Initialize game rules service
            _gameRules = new Services.GameRules(Settings);

            // Initialize deck with number of decks from settings
            _deck = new Deck(Settings.NumberOfDecks);

            // Initialize Basic Strategy service
            _basicStrategy = new Services.BasicStrategy();

            // Signal that initialization is complete
            IsInitialized = true;
        }

        /// <summary>
        /// View a specific player's cards (from summary strip tap).
        /// </summary>
        public void ViewPlayer(int position)
        {
            ViewedPlayerPosition = position;
            ViewedHandIndex = 0; // Reset to first hand

            // Check if viewing inactive player
            IsViewingInactivePlayer = (ActivePlayerPosition.HasValue && position != ActivePlayerPosition.Value);

            // Notify UI to update
            OnPropertyChanged(nameof(ViewedPlayerPosition));
            OnPropertyChanged(nameof(ViewedHandIndex));
            OnPropertyChanged(nameof(IsViewingInactivePlayer));
        }

        /// <summary>
        /// Return to viewing the active player.
        /// </summary>
        public void ReturnToActivePlayer()
        {
            if (ActivePlayerPosition.HasValue)
            {
                ViewedPlayerPosition = ActivePlayerPosition.Value;
                ViewedHandIndex = _currentHandIndex;
                IsViewingInactivePlayer = false;

                // Notify UI to update
                OnPropertyChanged(nameof(ViewedPlayerPosition));
                OnPropertyChanged(nameof(ViewedHandIndex));
                OnPropertyChanged(nameof(IsViewingInactivePlayer));
            }
        }

        /// <summary>
        /// Switch to viewing a different hand (for split hands).
        /// </summary>
        public void ViewHand(int handIndex)
        {
            ViewedHandIndex = handIndex;
            OnPropertyChanged(nameof(ViewedHandIndex));
        }

    }
}
