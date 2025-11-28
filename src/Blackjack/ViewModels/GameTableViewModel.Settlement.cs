using Blackjack.Models;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// Settlement and round management logic for GameTableViewModel.
    /// </summary>
    public partial class GameTableViewModel
    {
        /// <summary>
        /// Settles all player hands by comparing to dealer hand and paying out winnings.
        /// </summary>
        private async Task SettleAllHands()
        {
            if (Dealer == null || _gameRules == null)
            {
                return;
            }

            GameMessage = "Settling hands...";
            await Task.Delay(1000);

            foreach (var player in Players.Where(p => p.IsActive))
            {
                // Update viewed player to show current player being settled
                ViewedPlayerPosition = player.SeatPosition;
                OnPropertyChanged(nameof(ViewedPlayerPosition));

                foreach (var hand in player.Hands)
                {
                    // Skip hands that were already settled (blackjack payout or even money)
                    if (hand.Status == HandStatus.Blackjack ||
                        (hand.Status == HandStatus.Won && hand.IsBlackjack))
                    {
                        GameMessage = $"{player.Name}: Already paid";
                        await Task.Delay(500);
                        continue;
                    }

                    // Check if insurance was taken
                    bool insuranceTaken = _insuranceBets.ContainsKey(player.SeatPosition);
                    decimal insuranceBet = insuranceTaken ? _insuranceBets[player.SeatPosition] : 0;

                    // Settle the hand using GameRules service
                    decimal payout = _gameRules.SettleHand(hand, Dealer, insuranceTaken, insuranceBet);

                    // Update player bankroll
                    player.Bankroll += payout;

                    // Update human player display if applicable
                    if (player.IsHuman)
                    {
                        PlayerBankroll = player.Bankroll;

                        // Save bankroll to persistent storage
                        Services.BankrollService.SaveBankroll(PlayerBankroll);
                    }

                    // Display result
                    string resultMessage = hand.Status switch
                    {
                        HandStatus.Blackjack => $"{player.Name}: Blackjack! Wins ${payout:N0}",
                        HandStatus.Won => $"{player.Name}: Wins ${payout:N0}",
                        HandStatus.Lost => $"{player.Name}: Loses",
                        HandStatus.Push => $"{player.Name}: Push - Bet returned",
                        HandStatus.Busted => $"{player.Name}: Busted",
                        _ => $"{player.Name}: {hand.Status}"
                    };

                    GameMessage = resultMessage;
                    await Task.Delay(800);
                }
            }

            // Show human player's summary after all hands are settled
            ViewedPlayerPosition = HumanPlayerPosition;
            OnPropertyChanged(nameof(ViewedPlayerPosition));

            // Start new round
            await StartNewRound();
        }

        /// <summary>
        /// Prepares for a new round by checking shuffle point and returning to betting phase.
        /// </summary>
        private async Task StartNewRound()
        {
            GameMessage = "Round complete.";
            await Task.Delay(1000);

            // Check if any players are bankrupt
            foreach (var player in Players.Where(p => p.IsActive && !p.IsHuman))
            {
                if (player.Bankroll < Settings.TableMinimum)
                {
                    player.IsActive = false;
                    GameMessage = $"{player.Name} leaves the table (insufficient funds)";
                    await Task.Delay(800);
                }
            }

            // Check if human player is bankrupt
            var humanPlayer = Players[HumanPlayerPosition - 1];
            if (humanPlayer.Bankroll < Settings.TableMinimum)
            {
                GameMessage = $"Game Over! You're out of funds. Final bankroll: ${humanPlayer.Bankroll:N0}";
                await Task.Delay(3000);

                // Return to main menu
                await Shell.Current.GoToAsync("//MainMenuPage");
                return;
            }

            // Check if shoe needs shuffling
            if (_deck != null && _deck.NeedsReshuffle)
            {
                GameMessage = "Shuffling deck...";
                _deck.Shuffle();
                await Task.Delay(1500);
            }

            // Reset for new round (keep cards visible until next deal)
            CurrentPhase = GamePhase.Betting;
            IsBetting = true;
            CurrentBet = 0;
            CanConfirmBet = false;
            // Note: Cards remain visible until player bets and hits Deal

            // Clear insurance tracking
            _insuranceBets.Clear();
            _playersDecidedInsurance.Clear();

            GameMessage = "Place your bet for next round";
        }
    }
}
