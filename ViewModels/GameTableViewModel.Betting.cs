using Blackjack.Models;
using CommunityToolkit.Mvvm.Input;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// Betting-related commands and methods for GameTableViewModel.
    /// </summary>
    public partial class GameTableViewModel
    {
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

            // Update player's bankroll and bet in the model
            var humanPlayer = Players[HumanPlayerPosition - 1];
            humanPlayer.Bankroll = PlayerBankroll;
            humanPlayer.Hands[0].Bet = CurrentBet;

            // Hide betting UI
            IsBetting = false;
            GameMessage = "Processing bets...";

            // Place AI bets
            await PlaceAIBets();

            // Transition to dealing phase
            CurrentPhase = GamePhase.Dealing;
            GameMessage = "All bets placed. Dealing cards...";

            // Deal initial cards
            await DealInitialCards();
        }

        /// <summary>
        /// Places bets for all AI players at the table.
        /// </summary>
        private async Task PlaceAIBets()
        {
            if (_aiBettingService == null)
            {
                return;
            }

            foreach (var player in Players.Where(p => p.IsActive && !p.IsHuman))
            {
                // Generate bet amount for AI player
                decimal betAmount = _aiBettingService.GenerateBet(player.Bankroll);

                if (betAmount > 0)
                {
                    // Place bet using Player model method
                    bool success = player.PlaceBet(betAmount);

                    if (success)
                    {
                        GameMessage = $"{player.Name} bets ${betAmount:N0}";
                        await Task.Delay(300); // Brief delay for realism
                    }
                }
                else
                {
                    // AI is bankrupt or can't afford minimum
                    player.IsActive = false;
                    GameMessage = $"{player.Name} is sitting out (insufficient funds)";
                    await Task.Delay(300);
                }
            }
        }
    }
}
