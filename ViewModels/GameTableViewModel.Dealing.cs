using Blackjack.Models;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// Card dealing-related methods for GameTableViewModel.
    /// </summary>
    public partial class GameTableViewModel
    {
        /// <summary>
        /// Deals the initial two cards to each player and dealer.
        /// Follows casino dealing order: 1 card to each player, 1 to dealer (up), 
        /// 2nd card to each player, 2nd card to dealer (hole card, face-down).
        /// </summary>
        private async Task DealInitialCards()
        {
            if (_deck == null)
            {
                GameMessage = "Error: Deck not initialized";
                return;
            }

            // Check if deck needs shuffling
            if (_deck.NeedsReshuffle)
            {
                GameMessage = "Shuffling deck...";
                _deck.Shuffle();
                await Task.Delay(1000);
            }

            // Clear dealer cards
            DealerCards.Clear();
            Dealer?.ClearHand();

            // Save bet amounts before clearing hands, then restore them
            var playerBets = new Dictionary<int, decimal>();
            foreach (var player in Players.Where(p => p.IsActive))
            {
                // Save the bet amount if player has hands
                if (player.Hands.Count > 0)
                {
                    playerBets[player.SeatPosition] = player.Hands[0].Bet;
                }

                // Clear hands to prepare for dealing
                player.ClearHands();

                // Restore bet amount to the new hand
                if (playerBets.TryGetValue(player.SeatPosition, out decimal betAmount))
                {
                    player.Hands[0].Bet = betAmount;
                }
            }

            // First card to each player (positions 1-7)
            GameMessage = "Dealing first card to players...";
            foreach (var player in Players.Where(p => p.IsActive).OrderBy(p => p.SeatPosition))
            {
                var card = _deck.DealCard();
                if (card != null)
                {
                    player.Hands[0].AddCard(card);
                    GameMessage = $"Dealing to {player.Name}...";
                    await Task.Delay(400);
                }
            }

            // First card to dealer (up card)
            GameMessage = "Dealing to Dealer...";
            var dealerUpCard = _deck.DealCard();
            if (dealerUpCard != null && Dealer != null)
            {
                Dealer.AddCard(dealerUpCard);
                DealerCards.Add(dealerUpCard);
                DealerTotal = dealerUpCard.Rank == Rank.Ace ? "A" : dealerUpCard.Value.ToString();
                await Task.Delay(400);
            }

            // Second card to each player
            GameMessage = "Dealing second card to players...";
            foreach (var player in Players.Where(p => p.IsActive).OrderBy(p => p.SeatPosition))
            {
                var card = _deck.DealCard();
                if (card != null)
                {
                    player.Hands[0].AddCard(card);
                    GameMessage = $"Dealing to {player.Name}...";
                    await Task.Delay(400);
                }
            }

            // Second card to dealer (hole card, face-down)
            GameMessage = "Dealing hole card to Dealer...";
            var dealerHoleCard = _deck.DealCard();
            if (dealerHoleCard != null && Dealer != null)
            {
                Dealer.AddCard(dealerHoleCard);
                DealerCards.Add(dealerHoleCard); // Add to collection but UI will show face-down
                DealerHoleCardFaceDown = true;
                await Task.Delay(400);
            }

            // Notify UI to refresh player cards (important: cards were added to hands, not to Players collection)
            OnPropertyChanged(nameof(Players));

            // Notify UI to refresh dealer cards
            OnPropertyChanged(nameof(DealerCards));

            // Small delay to let UI update before checking for blackjack
            await Task.Delay(200);

            // Check for dealer blackjack
            await CheckDealerBlackjack();
        }
    }
}
