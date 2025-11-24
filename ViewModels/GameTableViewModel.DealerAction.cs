using Blackjack.Models;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// Dealer action logic for GameTableViewModel.
    /// </summary>
    public partial class GameTableViewModel
    {
        /// <summary>
        /// Processes the dealer's turn according to H17 rules.
        /// Dealer hits on soft 17, stands on hard 17 and all 18+.
        /// </summary>
        private async Task ProcessDealerTurn()
        {
            if (Dealer == null || _deck == null)
            {
                GameMessage = "Error: Dealer or deck not initialized";
                return;
            }

            // Check if all players busted - if so, skip dealer turn
            bool allPlayersBusted = Players
                .Where(p => p.IsActive)
                .All(p => p.Hands.All(h => h.Status == HandStatus.Busted));

            if (allPlayersBusted)
            {
                GameMessage = "All players busted. Dealer wins!";
                await Task.Delay(1500);
                await StartNewRound();
                return;
            }

            // Reveal dealer's hole card
            GameMessage = "Dealer revealing hole card...";
            await Task.Delay(1000);

            DealerHoleCardFaceDown = false;
            Dealer.RevealHoleCard();
            DealerTotal = Dealer.Hand.TotalValue.ToString();

            // Notify UI to update dealer cards display
            OnPropertyChanged(nameof(DealerCards));
            OnPropertyChanged(nameof(DealerTotal));

            GameMessage = $"Dealer has {DealerTotal}";
            await Task.Delay(1000);

            // Dealer draws cards according to H17 rules
            while (Dealer.ShouldHit())
            {
                GameMessage = "Dealer hits...";
                await Task.Delay(800);

                var card = _deck.DealCard();
                if (card != null)
                {
                    Dealer.AddCard(card);
                    DealerCards.Add(card);
                    DealerTotal = Dealer.Hand.TotalValue.ToString();

                    // Notify UI
                    OnPropertyChanged(nameof(DealerCards));
                    OnPropertyChanged(nameof(DealerTotal));

                    GameMessage = $"Dealer draws {card.Rank} of {card.Suit} - Total: {DealerTotal}";
                    await Task.Delay(1000);

                    // Check for bust
                    if (Dealer.Hand.IsBusted)
                    {
                        GameMessage = $"Dealer busts with {DealerTotal}!";
                        await Task.Delay(1500);
                        break;
                    }
                }
            }

            // Dealer stands
            if (!Dealer.Hand.IsBusted)
            {
                GameMessage = $"Dealer stands with {DealerTotal}";
                await Task.Delay(1500);
            }

            // Settle all hands
            CurrentPhase = GamePhase.Settlement;
            await SettleAllHands();
        }
    }
}
