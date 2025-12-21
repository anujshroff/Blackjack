using Blackjack.Models;
using CommunityToolkit.Mvvm.Input;

namespace Blackjack.ViewModels
{
    /// <summary>
    /// Player action commands and turn management for GameTableViewModel.
    /// </summary>
    public partial class GameTableViewModel
    {
        /// <summary>
        /// Command to request an additional card (Hit).
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanHit))]
        private async Task Hit()
        {
            if (!ActivePlayerPosition.HasValue || _deck == null)
                return;

            var player = Players[ActivePlayerPosition.Value - 1];
            var hand = player.Hands[_currentHandIndex];

            // Deal one card
            var card = _deck.DealCard();
            if (card != null)
            {
                hand.AddCard(card);
                GameMessage = $"{player.Name} hits - receives {card.Rank} of {card.Suit}";

                // Notify UI to update
                OnPropertyChanged(nameof(Players));
                await Task.Delay(500);

                // Check for bust
                if (hand.IsBusted)
                {
                    hand.Status = HandStatus.Busted;
                    GameMessage = $"{player.Name} busts with {hand.TotalValue}!";
                    await Task.Delay(1000);

                    // Move to next hand or player
                    await MoveToNextPlayerOrHand();
                    return;
                }

                // Update available actions
                UpdateAvailableActions();
            }
        }

        /// <summary>
        /// Command to keep current hand and end turn (Stand).
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanStand))]
        private async Task Stand()
        {
            if (!ActivePlayerPosition.HasValue)
                return;

            var player = Players[ActivePlayerPosition.Value - 1];
            var hand = player.Hands[_currentHandIndex];

            // Mark hand as standing
            hand.Status = HandStatus.Standing;
            GameMessage = $"{player.Name} stands with {hand.TotalValue}";
            await Task.Delay(800);

            // Move to next hand or player
            await MoveToNextPlayerOrHand();
        }

        /// <summary>
        /// Command to double the bet and receive one more card (Double Down).
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanDouble))]
        private async Task DoubleDown()
        {
            if (!ActivePlayerPosition.HasValue || _deck == null)
                return;

            var player = Players[ActivePlayerPosition.Value - 1];
            var hand = player.Hands[_currentHandIndex];

            // Validate bankroll
            if (player.Bankroll < hand.Bet)
            {
                GameMessage = "Insufficient funds to double down";
                await Task.Delay(1000);
                return;
            }

            // Deduct additional bet
            player.Bankroll -= hand.Bet;
            hand.Bet *= 2;

            // Update human player display if applicable
            if (player.IsHuman)
            {
                PlayerBankroll = player.Bankroll;
            }

            GameMessage = $"{player.Name} doubles down - bet now ${hand.Bet:N0}";
            await Task.Delay(800);

            // Deal exactly one card
            var card = _deck.DealCard();
            if (card != null)
            {
                hand.AddCard(card);
                GameMessage = $"{player.Name} receives {card.Rank} of {card.Suit}";

                // Notify UI to update
                OnPropertyChanged(nameof(Players));
                await Task.Delay(500);

                // Check for bust
                if (hand.IsBusted)
                {
                    hand.Status = HandStatus.Busted;
                    GameMessage = $"{player.Name} busts with {hand.TotalValue}!";
                }
                else
                {
                    hand.Status = HandStatus.Standing;
                    GameMessage = $"{player.Name} stands with {hand.TotalValue}";
                }

                await Task.Delay(1000);
            }

            // Automatically move to next hand or player
            await MoveToNextPlayerOrHand();
        }

        /// <summary>
        /// Command to split a pair into two hands (Split).
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanSplit))]
        private async Task Split()
        {
            if (!ActivePlayerPosition.HasValue || _deck == null)
                return;

            var player = Players[ActivePlayerPosition.Value - 1];
            var hand = player.Hands[_currentHandIndex];

            // Validate can split
            if (!hand.IsPair || player.Hands.Count >= 4)
            {
                GameMessage = "Cannot split this hand";
                await Task.Delay(1000);
                return;
            }

            // Validate bankroll
            if (player.Bankroll < hand.Bet)
            {
                GameMessage = "Insufficient funds to split";
                await Task.Delay(1000);
                return;
            }

            // Deduct bet for second hand
            player.Bankroll -= hand.Bet;

            // Update human player display if applicable
            if (player.IsHuman)
            {
                PlayerBankroll = player.Bankroll;
            }

            // Split the pair
            var card1 = hand.Cards[0];
            var card2 = hand.Cards[1];
            bool isSplittingAces = card1.Rank == Rank.Ace;

            // Clear original hand and add first card back
            hand.Cards.Clear();
            hand.AddCard(card1);
            hand.HandIndex = 0;
            hand.IsFromSplit = true;  // Mark as split hand (not eligible for blackjack payout)

            // Create second hand with second card
            var newHand = new Hand
            {
                Bet = hand.Bet,
                Status = HandStatus.Active,
                HandIndex = 1,
                NeedsSecondCard = true,  // Mark as waiting for second card
                IsFromSplit = true  // Mark as split hand (not eligible for blackjack payout)
            };
            newHand.AddCard(card2);
            player.Hands.Insert(_currentHandIndex + 1, newHand);

            GameMessage = $"{player.Name} splits {card1.Rank}s";
            await Task.Delay(800);

            // Deal one card to first hand only
            var newCard1 = _deck.DealCard();
            if (newCard1 != null)
            {
                hand.AddCard(newCard1);
                GameMessage = $"First hand receives {newCard1.Rank} of {newCard1.Suit}";
                await Task.Delay(600);
            }

            // Notify UI to update
            OnPropertyChanged(nameof(Players));

            // Special rule: Split Aces get only one card each and auto-stand
            if (isSplittingAces)
            {
                // Deal second card to second hand for aces
                var newCard2 = _deck.DealCard();
                if (newCard2 != null)
                {
                    newHand.AddCard(newCard2);
                    newHand.NeedsSecondCard = false;
                    GameMessage = $"Second hand receives {newCard2.Rank} of {newCard2.Suit}";
                    await Task.Delay(600);
                }

                hand.Status = HandStatus.Standing;
                newHand.Status = HandStatus.Standing;
                GameMessage = $"{player.Name} - Split Aces complete";
                await Task.Delay(1000);

                // Skip both hands, move to next player
                _currentHandIndex = player.Hands.Count;
                await MoveToNextPlayerOrHand();
            }
            else
            {
                // Continue playing first hand
                GameMessage = $"{player.Name} - Playing first hand";
                await Task.Delay(500);
                UpdateAvailableActions();
            }
        }

        /// <summary>
        /// Starts the player action phase, beginning with the first active player.
        /// </summary>
        private async Task StartPlayerActions()
        {
            // Find first active player with at least one active hand
            var firstPlayer = Players
                .Where(p => p.IsActive && GameTableViewModel.HasActiveHands(p))
                .OrderBy(p => p.SeatPosition)
                .FirstOrDefault();

            if (firstPlayer == null)
            {
                // No players with active hands, move to dealer action
                ActivePlayerPosition = null;
                CanHit = false;
                CanStand = false;
                CanDouble = false;
                CanSplit = false;

                GameMessage = "All players settled. Dealer's turn...";
                await Task.Delay(1500);

                CurrentPhase = GamePhase.DealerAction;
                await ProcessDealerTurn();
                return;
            }

            // Set active player
            ActivePlayerPosition = firstPlayer.SeatPosition;
            _currentHandIndex = 0;

            if (firstPlayer.IsHuman)
            {
                GameMessage = "Your turn!";
                UpdateAvailableActions();
            }
            else
            {
                await ProcessAIPlayerTurn();
            }
        }

        /// <summary>
        /// Moves to the next hand or next player after current hand is complete.
        /// </summary>
        private async Task MoveToNextPlayerOrHand()
        {
            if (!ActivePlayerPosition.HasValue)
                return;

            var currentPlayer = Players[ActivePlayerPosition.Value - 1];

            // Check if there are more hands to play for this player
            _currentHandIndex++;
            if (_currentHandIndex < currentPlayer.Hands.Count)
            {
                var nextHand = currentPlayer.Hands[_currentHandIndex];

                // Skip hands that are already settled (busted, standing, blackjack)
                if (nextHand.Status == HandStatus.Active)
                {
                    // Check if this hand needs its second card (split hand waiting)
                    if (nextHand.NeedsSecondCard && _deck != null)
                    {
                        GameMessage = $"Dealing second card to hand {_currentHandIndex + 1}...";
                        await Task.Delay(500);

                        var card = _deck.DealCard();
                        if (card != null)
                        {
                            nextHand.AddCard(card);
                            nextHand.NeedsSecondCard = false;
                            GameMessage = $"{currentPlayer.Name} - Hand {_currentHandIndex + 1} receives {card.Rank} of {card.Suit}";
                            OnPropertyChanged(nameof(Players));
                            await Task.Delay(600);
                        }
                    }

                    GameMessage = currentPlayer.IsHuman
                        ? $"Playing hand {_currentHandIndex + 1} of {currentPlayer.Hands.Count}"
                        : $"{currentPlayer.Name} - Playing hand {_currentHandIndex + 1}";
                    await Task.Delay(500);

                    // Update viewed hand index to show the current hand in UI
                    ViewedHandIndex = _currentHandIndex;

                    if (currentPlayer.IsHuman)
                    {
                        UpdateAvailableActions();
                    }
                    else
                    {
                        await ProcessAIPlayerTurn();
                    }
                    return;
                }
                else
                {
                    // This hand is already settled, move to next
                    await MoveToNextPlayerOrHand();
                    return;
                }
            }

            // No more hands for this player, find next active player with active hands
            var nextPlayer = Players
                .Where(p => p.IsActive && p.SeatPosition > ActivePlayerPosition.Value && GameTableViewModel.HasActiveHands(p))
                .OrderBy(p => p.SeatPosition)
                .FirstOrDefault();

            if (nextPlayer != null)
            {
                // Move to next player
                ActivePlayerPosition = nextPlayer.SeatPosition;
                _currentHandIndex = 0;

                if (nextPlayer.IsHuman)
                {
                    GameMessage = "Your turn!";
                    UpdateAvailableActions();
                }
                else
                {
                    await ProcessAIPlayerTurn();
                }
            }
            else
            {
                // All players finished, move to dealer action
                ActivePlayerPosition = null;
                CanHit = false;
                CanStand = false;
                CanDouble = false;
                CanSplit = false;

                GameMessage = "All players finished. Dealer's turn...";
                await Task.Delay(1500);

                // Process dealer turn
                CurrentPhase = GamePhase.DealerAction;
                await ProcessDealerTurn();
            }
        }

        /// <summary>
        /// Updates which action buttons are available based on current hand state.
        /// </summary>
        private void UpdateAvailableActions()
        {
            // Reset all actions
            CanHit = false;
            CanStand = false;
            CanDouble = false;
            CanSplit = false;

            if (!ActivePlayerPosition.HasValue)
                return;

            var player = Players[ActivePlayerPosition.Value - 1];
            if (!player.IsHuman || _currentHandIndex >= player.Hands.Count)
                return;

            var hand = player.Hands[_currentHandIndex];

            // Only enable actions if hand is active
            if (hand.Status != HandStatus.Active)
                return;

            // Hit: Available if not busted and under 21
            CanHit = hand.TotalValue < 21;

            // Stand: Always available
            CanStand = true;

            // Double Down: Only on 2-card hand with sufficient bankroll
            CanDouble = hand.Cards.Count == 2 && player.Bankroll >= hand.Bet;

            // Split: Only on pairs with sufficient bankroll and not exceeding 4 hands
            CanSplit = hand.IsPair &&
                       player.Hands.Count < 4 &&
                       player.Bankroll >= hand.Bet;
        }

        /// <summary>
        /// Helper method to check if a player has any hands that still need actions.
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns>True if player has at least one hand with Active status</returns>
        private static bool HasActiveHands(Player player)
        {
            return player.Hands.Any(h => h.Status == HandStatus.Active);
        }

        /// <summary>
        /// Processes an AI player's turn using Basic Strategy.
        /// </summary>
        private async Task ProcessAIPlayerTurn()
        {
            if (!ActivePlayerPosition.HasValue || _basicStrategy == null || Dealer?.UpCard == null || _deck == null)
                return;

            var player = Players[ActivePlayerPosition.Value - 1];

            while (_currentHandIndex < player.Hands.Count)
            {
                var hand = player.Hands[_currentHandIndex];

                // Skip if hand is already settled
                if (hand.Status != HandStatus.Active)
                {
                    _currentHandIndex++;
                    continue;
                }

                // Check if this hand needs its second card (split hand waiting)
                if (hand.NeedsSecondCard)
                {
                    GameMessage = $"Dealing second card to hand {_currentHandIndex + 1}...";
                    await Task.Delay(500);

                    var secondCard = _deck.DealCard();
                    if (secondCard != null)
                    {
                        hand.AddCard(secondCard);
                        hand.NeedsSecondCard = false;
                        GameMessage = $"{player.Name} - Hand {_currentHandIndex + 1} receives {secondCard.Rank} of {secondCard.Suit}";
                        OnPropertyChanged(nameof(Players));
                        await Task.Delay(600);
                    }
                }

                // Update viewed hand index to show the current hand in UI
                ViewedHandIndex = _currentHandIndex;
                OnPropertyChanged(nameof(ViewedHandIndex));

                // Show thinking message
                GameMessage = $"{player.Name} thinking...";
                await Task.Delay(800);

                // Get recommended action from Basic Strategy
                var recommendedAction = _basicStrategy.GetRecommendedAction(hand, Dealer.UpCard);

                // Execute action with fallback logic
                bool actionComplete = false;
                while (!actionComplete)
                {
                    switch (recommendedAction)
                    {
                        case Services.PlayerAction.Hit:
                            var card = _deck.DealCard();
                            if (card != null)
                            {
                                hand.AddCard(card);
                                GameMessage = $"{player.Name} hits - receives {card.Rank} of {card.Suit} (Total: {hand.TotalValue})";
                                OnPropertyChanged(nameof(Players));
                                await Task.Delay(600);

                                if (hand.IsBusted)
                                {
                                    hand.Status = HandStatus.Busted;
                                    GameMessage = $"{player.Name} busts with {hand.TotalValue}!";
                                    await Task.Delay(1000);
                                    actionComplete = true;
                                }
                                else
                                {
                                    // Always re-evaluate strategy after each hit
                                    recommendedAction = _basicStrategy.GetRecommendedAction(hand, Dealer.UpCard);
                                }
                            }
                            else
                            {
                                actionComplete = true;
                            }
                            break;

                        case Services.PlayerAction.Stand:
                            hand.Status = HandStatus.Standing;
                            GameMessage = $"{player.Name} stands with {hand.TotalValue}";
                            await Task.Delay(800);
                            actionComplete = true;
                            break;

                        case Services.PlayerAction.Double:
                            // Check if can double (sufficient bankroll and 2 cards)
                            if (hand.Cards.Count == 2 && player.Bankroll >= hand.Bet)
                            {
                                player.Bankroll -= hand.Bet;
                                hand.Bet *= 2;
                                GameMessage = $"{player.Name} doubles down - bet now ${hand.Bet:N0}";
                                await Task.Delay(800);

                                var doubleCard = _deck.DealCard();
                                if (doubleCard != null)
                                {
                                    hand.AddCard(doubleCard);
                                    GameMessage = $"{player.Name} receives {doubleCard.Rank} of {doubleCard.Suit} (Total: {hand.TotalValue})";
                                    OnPropertyChanged(nameof(Players));
                                    await Task.Delay(600);

                                    if (hand.IsBusted)
                                    {
                                        hand.Status = HandStatus.Busted;
                                        GameMessage = $"{player.Name} busts with {hand.TotalValue}!";
                                    }
                                    else
                                    {
                                        hand.Status = HandStatus.Standing;
                                        GameMessage = $"{player.Name} stands with {hand.TotalValue}";
                                    }
                                    await Task.Delay(1000);
                                }
                                actionComplete = true;
                            }
                            else
                            {
                                // Can't double, fallback to hit
                                recommendedAction = Services.PlayerAction.Hit;
                            }
                            break;

                        case Services.PlayerAction.Split:
                            // Check if can split
                            if (hand.IsPair && player.Hands.Count < 4 && player.Bankroll >= hand.Bet)
                            {
                                player.Bankroll -= hand.Bet;

                                var card1 = hand.Cards[0];
                                var card2 = hand.Cards[1];
                                bool isSplittingAces = card1.Rank == Rank.Ace;

                                hand.Cards.Clear();
                                hand.AddCard(card1);
                                hand.HandIndex = _currentHandIndex;
                                hand.IsFromSplit = true;  // Mark as split hand (not eligible for blackjack payout)

                                var newHand = new Hand
                                {
                                    Bet = hand.Bet,
                                    Status = HandStatus.Active,
                                    HandIndex = _currentHandIndex + 1,
                                    NeedsSecondCard = true,  // Mark as waiting for second card
                                    IsFromSplit = true  // Mark as split hand (not eligible for blackjack payout)
                                };
                                newHand.AddCard(card2);
                                player.Hands.Insert(_currentHandIndex + 1, newHand);

                                GameMessage = $"{player.Name} splits {card1.Rank}s";
                                await Task.Delay(800);

                                // Deal one card to first hand only
                                var newCard1 = _deck.DealCard();
                                if (newCard1 != null)
                                {
                                    hand.AddCard(newCard1);
                                    GameMessage = $"First hand receives {newCard1.Rank} of {newCard1.Suit}";
                                    await Task.Delay(600);
                                }

                                OnPropertyChanged(nameof(Players));

                                if (isSplittingAces)
                                {
                                    // Deal second card to second hand for aces
                                    var newCard2 = _deck.DealCard();
                                    if (newCard2 != null)
                                    {
                                        newHand.AddCard(newCard2);
                                        newHand.NeedsSecondCard = false;
                                        GameMessage = $"Second hand receives {newCard2.Rank} of {newCard2.Suit}";
                                        await Task.Delay(600);
                                    }

                                    hand.Status = HandStatus.Standing;
                                    newHand.Status = HandStatus.Standing;
                                    GameMessage = $"{player.Name} - Split Aces complete";
                                    await Task.Delay(1000);
                                    actionComplete = true;
                                }
                                else
                                {
                                    // Continue with first hand, re-evaluate
                                    recommendedAction = _basicStrategy.GetRecommendedAction(hand, Dealer.UpCard);
                                }
                            }
                            else
                            {
                                // Can't split, get action excluding split option to avoid infinite loop
                                recommendedAction = _basicStrategy.GetRecommendedAction(hand, Dealer.UpCard, excludeSplit: true);
                            }
                            break;
                    }
                }

                // Notify UI after completing this hand to update status display
                OnPropertyChanged(nameof(Players));

                // Move to next hand
                _currentHandIndex++;

                // Update viewed hand index if moving to next hand for this player
                if (_currentHandIndex < player.Hands.Count)
                {
                    ViewedHandIndex = _currentHandIndex;
                    OnPropertyChanged(nameof(ViewedHandIndex));
                }
            }

            // All hands for this AI player complete, move to next player
            await MoveToNextPlayerOrHand();
        }
    }
}
