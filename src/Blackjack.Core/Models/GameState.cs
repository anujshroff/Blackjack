namespace Blackjack.Models
{
    /// <summary>
    /// Represents the current phase of the game.
    /// </summary>
    public enum GamePhase
    {
        Betting,         // Players place bets
        Dealing,         // Initial cards are being dealt
        InsuranceOffer,  // Dealer shows Ace, offering insurance
        PlayerActions,   // Players take their turns (Hit, Stand, Double, Split)
        DealerAction,    // Dealer reveals hole card and plays
        Settlement,      // Winnings/losses are calculated and distributed
        Shuffling        // Deck is being reshuffled
    }

    /// <summary>
    /// Represents the complete state of a Blackjack game.
    /// </summary>
    public class GameState(GameSettings settings)
    {
        public List<Player> Players { get; set; } = [];
        public Dealer Dealer { get; set; } = new Dealer();
        public Deck Shoe { get; set; } = new Deck();
        public GamePhase CurrentPhase { get; set; } = GamePhase.Betting;
        public Player? ActivePlayer { get; set; } = null;
        public GameSettings Settings { get; set; } = settings;

        /// <summary>
        /// Index of the current active hand being played (for split hands).
        /// </summary>
        public int ActiveHandIndex { get; set; } = 0;

        /// <summary>
        /// Round number (increments with each new round).
        /// </summary>
        public int RoundNumber { get; set; } = 1;

        /// <summary>
        /// Adds a player to the game at the specified seat.
        /// </summary>
        /// <param name="player">The player to add.</param>
        /// <returns>True if player was added, false if seat is occupied.</returns>
        public bool AddPlayer(Player player)
        {
            // Check if seat is already occupied
            if (Players.Any(p => p.SeatPosition == player.SeatPosition))
            {
                return false;
            }

            Players.Add(player);

            // Sort players by seat position (1-7)
            Players = [.. Players.OrderBy(p => p.SeatPosition)];

            return true;
        }

        /// <summary>
        /// Gets the next player who needs to act.
        /// </summary>
        /// <returns>The next player, or null if all players have acted.</returns>
        public Player? GetNextPlayer()
        {
            if (ActivePlayer == null)
            {
                // Start with first player
                ActivePlayer = Players.FirstOrDefault(p => p.IsActive && p.Hands.Count != 0);
                ActiveHandIndex = 0;
                return ActivePlayer;
            }

            // Check if current player has more hands to play
            if (ActivePlayer.Hands.Count > ActiveHandIndex + 1)
            {
                ActiveHandIndex++;
                return ActivePlayer;
            }

            // Move to next player
            int currentIndex = Players.IndexOf(ActivePlayer);
            for (int i = currentIndex + 1; i < Players.Count; i++)
            {
                if (Players[i].IsActive && Players[i].Hands.Count != 0)
                {
                    ActivePlayer = Players[i];
                    ActiveHandIndex = 0;
                    return ActivePlayer;
                }
            }

            // No more players
            ActivePlayer = null;
            ActiveHandIndex = 0;
            return null;
        }

        /// <summary>
        /// Gets the currently active hand being played.
        /// </summary>
        public Hand? GetActiveHand()
        {
            if (ActivePlayer == null || ActiveHandIndex >= ActivePlayer.Hands.Count)
            {
                return null;
            }

            return ActivePlayer.Hands[ActiveHandIndex];
        }

        /// <summary>
        /// Advances to the next game phase.
        /// </summary>
        public void AdvancePhase()
        {
            CurrentPhase = CurrentPhase switch
            {
                GamePhase.Betting => GamePhase.Dealing,
                GamePhase.Dealing => GamePhase.InsuranceOffer,
                GamePhase.InsuranceOffer => GamePhase.PlayerActions,
                GamePhase.PlayerActions => GamePhase.DealerAction,
                GamePhase.DealerAction => GamePhase.Settlement,
                GamePhase.Settlement => Shoe.NeedsReshuffle ? GamePhase.Shuffling : GamePhase.Betting,
                GamePhase.Shuffling => GamePhase.Betting,
                _ => GamePhase.Betting
            };
        }

        /// <summary>
        /// Resets the game state for a new round.
        /// </summary>
        public void StartNewRound()
        {
            // Clear all hands
            foreach (var player in Players)
            {
                player.ClearHands();
            }
            Dealer.ClearHand();

            // Reset state
            CurrentPhase = GamePhase.Betting;
            ActivePlayer = null;
            ActiveHandIndex = 0;
            RoundNumber++;
        }

        /// <summary>
        /// Gets all active players (not bankrupt, have placed bets).
        /// </summary>
        public List<Player> GetActivePlayers()
        {
            return [.. Players.Where(p => p.IsActive && !p.IsBankrupt)];
        }
    }
}
