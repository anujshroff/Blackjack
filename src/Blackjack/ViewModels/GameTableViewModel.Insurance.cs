using Blackjack.Models;
using CommunityToolkit.Mvvm.Input;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// Insurance and dealer blackjack check-related methods for GameTableViewModel.
    /// </summary>
    public partial class GameTableViewModel
    {
        /// <summary>
        /// Command to take insurance against dealer blackjack.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanInsure))]
        private async Task Insurance()
        {
            var humanPlayer = Players[HumanPlayerPosition - 1];
            var hand = humanPlayer.Hands[0];
            decimal insuranceCost = hand.Bet / 2;

            // Validate player has enough bankroll
            if (humanPlayer.Bankroll < insuranceCost)
            {
                GameMessage = "Insufficient funds for insurance";
                await Task.Delay(1000);
                return;
            }

            // Deduct insurance cost from bankroll
            humanPlayer.Bankroll -= insuranceCost;
            PlayerBankroll = humanPlayer.Bankroll;

            // Track the insurance bet
            _insuranceBets[HumanPlayerPosition] = insuranceCost;

            // Force UI update for player display
            OnPropertyChanged(nameof(PlayerBankroll));
            OnPropertyChanged(nameof(Players));

            // Disable buttons and mark decision made
            CanInsure = false;
            _awaitingHumanInsuranceDecision = false;
            _playersDecidedInsurance.Add(HumanPlayerPosition);

            GameMessage = $"Insurance taken for ${insuranceCost:N0} - Bankroll: ${PlayerBankroll:N0}";
            await Task.Delay(1000);
        }

        /// <summary>
        /// Command to take even money when player has blackjack and dealer shows Ace.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanEvenMoney))]
        private async Task EvenMoney()
        {
            var humanPlayer = Players[HumanPlayerPosition - 1];
            var hand = humanPlayer.Hands[0];

            // Pay 1:1 immediately
            decimal payout = Services.GameRules.CalculateEvenMoneyPayout(hand.Bet);
            humanPlayer.Bankroll += payout;
            PlayerBankroll = humanPlayer.Bankroll;

            // Mark hand as settled
            hand.Status = HandStatus.Won;

            // Disable buttons and mark decision made
            CanEvenMoney = false;
            _awaitingHumanInsuranceDecision = false;
            _playersDecidedInsurance.Add(HumanPlayerPosition);

            GameMessage = $"Even Money accepted - Paid ${payout:N0}";
            await Task.Delay(1000);
        }

        /// <summary>
        /// Checks if dealer should peek for blackjack and handles insurance/even money offers.
        /// </summary>
        private async Task CheckDealerBlackjack()
        {
            if (Dealer == null || Dealer.UpCard == null)
            {
                GameMessage = "Error: Dealer not properly initialized";
                return;
            }

            // Check if dealer shows Ace or 10-value card (peek situations)
            bool dealerShowsAce = Dealer.UpCard.Rank == Rank.Ace;
            bool dealerShowsTen = Dealer.UpCard.Value == 10;

            if (!dealerShowsAce && !dealerShowsTen)
            {
                // No peek needed, settle any player blackjacks first
                await SettlePlayerBlackjacks();

                // Then proceed to player actions
                GameMessage = "Player actions will begin...";
                await Task.Delay(500);
                CurrentPhase = GamePhase.PlayerActions;
                await StartPlayerActions();
                return;
            }

            // Dealer shows Ace - offer insurance/even money
            if (dealerShowsAce)
            {
                CurrentPhase = GamePhase.InsuranceOffer;
                await OfferInsuranceToPlayers();
            }

            // Peek at dealer's hole card
            await RevealDealerHoleCardAndSettle();
        }

        /// <summary>
        /// Offers insurance to players without blackjack and even money to players with blackjack.
        /// </summary>
        private async Task OfferInsuranceToPlayers()
        {
            GameMessage = "Dealer showing Ace - Insurance available";
            await Task.Delay(1000);

            // Clear previous insurance decisions
            _insuranceBets.Clear();
            _playersDecidedInsurance.Clear();
            _awaitingHumanInsuranceDecision = false;

            // Check each active player's hand
            bool humanHasBlackjack = false;
            var humanPlayer = Players[HumanPlayerPosition - 1];

            if (humanPlayer.IsActive && humanPlayer.Hands.Count > 0)
            {
                var hand = humanPlayer.Hands[0];

                if (hand.IsBlackjack)
                {
                    // Offer even money
                    humanHasBlackjack = true;
                    CanEvenMoney = true;
                    GameMessage = "You have Blackjack! Take Even Money or risk a push?";
                    _awaitingHumanInsuranceDecision = true;
                }
                else
                {
                    // Offer insurance
                    decimal insuranceCost = hand.Bet / 2;
                    if (humanPlayer.Bankroll >= insuranceCost)
                    {
                        CanInsure = true;
                        GameMessage = $"Insurance available for ${insuranceCost:N0} - Click INS button or wait";
                        _awaitingHumanInsuranceDecision = true;
                    }
                }
            }

            // Process AI decisions with delay
            await ProcessAIInsuranceDecisions();

            // Wait for human player decision if needed
            if (_awaitingHumanInsuranceDecision)
            {
                // Get insurance cost for display
                decimal displayInsuranceCost = 0;
                if (!humanHasBlackjack && humanPlayer.Hands.Count > 0)
                {
                    displayInsuranceCost = humanPlayer.Hands[0].Bet / 2;
                }

                // Wait until human decides with visible countdown
                int secondsRemaining = 10;
                while (_awaitingHumanInsuranceDecision && secondsRemaining > 0)
                {
                    GameMessage = humanHasBlackjack ?
                        $"Even Money? (auto-decline in {secondsRemaining}s)" :
                        $"Insurance? ${displayInsuranceCost:N0} (auto-decline in {secondsRemaining}s)";

                    await Task.Delay(1000);
                    secondsRemaining--;
                }

                // Auto-decline if player didn't respond
                if (_awaitingHumanInsuranceDecision)
                {
                    CanInsure = false;
                    CanEvenMoney = false;
                    _awaitingHumanInsuranceDecision = false;
                    GameMessage = humanHasBlackjack ? "Even Money declined" : "Insurance declined";
                    await Task.Delay(500);
                }
            }

            // Disable insurance buttons
            CanInsure = false;
            CanEvenMoney = false;
        }

        /// <summary>
        /// AI players automatically decline insurance with a realistic delay.
        /// </summary>
        private async Task ProcessAIInsuranceDecisions()
        {
            foreach (var player in Players.Where(p => p.IsActive && !p.IsHuman))
            {
                // AI always declines insurance per Basic Strategy
                await Task.Delay(600); // Realistic thinking delay
                GameMessage = $"{player.Name} declines insurance";
                _playersDecidedInsurance.Add(player.SeatPosition);
                await Task.Delay(300);
            }
        }

        /// <summary>
        /// Reveals dealer hole card and settles hands if dealer has blackjack.
        /// </summary>
        private async Task RevealDealerHoleCardAndSettle()
        {
            if (Dealer == null || _gameRules == null)
            {
                return;
            }

            GameMessage = "Dealer checking for Blackjack...";
            await Task.Delay(1000);

            if (Dealer.HasBlackjack)
            {
                // Dealer has blackjack - reveal hole card
                DealerHoleCardFaceDown = false;
                Dealer.RevealHoleCard();
                DealerTotal = Dealer.Hand.TotalValue.ToString();

                GameMessage = "Dealer has Blackjack!";
                await Task.Delay(1500);

                // Settle all hands and start new round (handles bankruptcy, shuffle, etc.)
                await SettleHandsForDealerBlackjack();
            }
            else
            {
                // Dealer doesn't have blackjack
                GameMessage = "No Blackjack. Play continues...";

                // Collect insurance bets (they lose)
                foreach (var insuranceBet in _insuranceBets)
                {
                    var player = Players[insuranceBet.Key - 1];
                    // Insurance bet already deducted, just note it's lost
                    GameMessage = $"{player.Name} loses insurance bet";
                    await Task.Delay(300);
                }

                await Task.Delay(1000);

                // Settle any player blackjacks that haven't been settled yet (via even money)
                await SettlePlayerBlackjacks();

                // Proceed to player actions
                CurrentPhase = GamePhase.PlayerActions;
                await Task.Delay(500);
                await StartPlayerActions();
            }
        }

        /// <summary>
        /// Settles all hands when dealer has blackjack at initial deal.
        /// </summary>
        private async Task SettleHandsForDealerBlackjack()
        {
            if (_gameRules == null)
            {
                return;
            }

            foreach (var player in Players.Where(p => p.IsActive))
            {
                // Update viewed player to show current player being settled
                ViewedPlayerPosition = player.SeatPosition;
                OnPropertyChanged(nameof(ViewedPlayerPosition));

                var hand = player.Hands[0];
                decimal payout = 0;

                // Skip hands already settled (e.g., even money taken)
                if (hand.Status != HandStatus.Active)
                {
                    // Still check for insurance payout even if hand was settled via even money
                    if (_insuranceBets.TryGetValue(player.SeatPosition, out decimal settledInsuranceBet))
                    {
                        payout = _gameRules.CalculateInsurancePayout(settledInsuranceBet);
                        player.Bankroll += payout;

                        if (player.IsHuman)
                        {
                            PlayerBankroll = player.Bankroll;
                        }

                        GameMessage = $"{player.Name} insurance pays ${payout:N0}";
                        await Task.Delay(500);
                    }
                    continue;
                }

                // Check for insurance payout
                if (_insuranceBets.TryGetValue(player.SeatPosition, out decimal insuranceBet))
                {
                    payout += _gameRules.CalculateInsurancePayout(insuranceBet);
                    GameMessage = $"{player.Name} insurance pays ${payout:N0}";
                    await Task.Delay(500);
                }

                if (hand.IsBlackjack)
                {
                    // Player blackjack vs dealer blackjack = Push
                    payout += hand.Bet; // Return original bet
                    hand.Status = HandStatus.Push;
                    GameMessage = $"{player.Name}: Push (Blackjack vs Blackjack)";
                }
                else
                {
                    // Player loses to dealer blackjack
                    hand.Status = HandStatus.Lost;
                    GameMessage = $"{player.Name}: Lost to Dealer Blackjack";
                }

                // Update bankroll
                player.Bankroll += payout;

                // Update human player display if applicable
                if (player.IsHuman)
                {
                    PlayerBankroll = player.Bankroll;
                }

                await Task.Delay(800);
            }

            // Show human player's summary after all hands are settled
            ViewedPlayerPosition = HumanPlayerPosition;
            OnPropertyChanged(nameof(ViewedPlayerPosition));

            // Start new round (handles bankruptcy check, shuffle check, etc.)
            await StartNewRound();
        }

        /// <summary>
        /// Settles all player blackjacks immediately when dealer doesn't have blackjack.
        /// Pays 3:2 and marks hands as settled.
        /// </summary>
        private async Task SettlePlayerBlackjacks()
        {
            if (_gameRules == null)
            {
                return;
            }

            bool anyBlackjacks = false;

            foreach (var player in Players.Where(p => p.IsActive))
            {
                var hand = player.Hands[0]; // Initial hand only

                // Check if player has blackjack (natural 21)
                if (hand.IsBlackjack && hand.Status == HandStatus.Active)
                {
                    anyBlackjacks = true;

                    // Calculate 3:2 payout
                    decimal payout = _gameRules.CalculateBlackjackPayout(hand.Bet);

                    // Update bankroll
                    player.Bankroll += payout;

                    // Mark hand as settled
                    hand.Status = HandStatus.Blackjack;

                    // Update human player display if applicable
                    if (player.IsHuman)
                    {
                        PlayerBankroll = player.Bankroll;
                    }

                    // Show payout message
                    GameMessage = $"{player.Name}: Blackjack! Paid ${payout:N0}";
                    await Task.Delay(1000);
                }
            }

            if (anyBlackjacks)
            {
                await Task.Delay(500);
            }
        }
    }
}
