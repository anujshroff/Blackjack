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

            // Check if deck needs shuffling (reset brings back all cards)
            if (_deck.NeedsReshuffle)
            {
                GameMessage = "Shuffling deck...";
                _deck.Reset();
                await Task.Delay(1000);
            }

            // Note: Cards are already cleared in ConfirmBet(), no need to clear again
            // Just save bet amounts to restore after dealing starts
            var playerBets = new Dictionary<int, decimal>();
            foreach (var player in Players.Where(p => p.IsActive))
            {
                // Save the bet amount if player has hands
                if (player.Hands.Count > 0)
                {
                    playerBets[player.SeatPosition] = player.Hands[0].Bet;
                }
            }

            // First card to each player (positions 1-7)
            GameMessage = "Dealing first card to players...";
            foreach (var player in Players.Where(p => p.IsActive).OrderBy(p => p.SeatPosition))
            {
                // Update viewed player to show who is receiving the card
                ViewedPlayerPosition = player.SeatPosition;
                OnPropertyChanged(nameof(ViewedPlayerPosition));

                var card = _deck.DealCard();
                if (card != null)
                {
                    player.Hands[0].AddCard(card);

                    // Restore bet amount if needed
                    if (playerBets.TryGetValue(player.SeatPosition, out decimal betAmount))
                    {
                        player.Hands[0].Bet = betAmount;
                    }

                    GameMessage = $"Dealing to {player.Name}...";

                    // Update UI to show the new card
                    OnPropertyChanged(nameof(Players));
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
                OnPropertyChanged(nameof(DealerCards));
                await Task.Delay(400);
            }

            // Second card to each player
            GameMessage = "Dealing second card to players...";
            foreach (var player in Players.Where(p => p.IsActive).OrderBy(p => p.SeatPosition))
            {
                // Update viewed player to show who is receiving the card
                ViewedPlayerPosition = player.SeatPosition;
                OnPropertyChanged(nameof(ViewedPlayerPosition));

                var card = _deck.DealCard();
                if (card != null)
                {
                    player.Hands[0].AddCard(card);
                    GameMessage = $"Dealing to {player.Name}...";

                    // Update UI to show the new card
                    OnPropertyChanged(nameof(Players));
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
                OnPropertyChanged(nameof(DealerCards));
                await Task.Delay(400);
            }

            // Small delay to let UI update before checking for blackjack
            await Task.Delay(200);

            // Check for dealer blackjack
            await CheckDealerBlackjack();
        }
    }
}
