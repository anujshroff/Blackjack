namespace Blackjack.Services
{
    /// <summary>
    /// Enforces and validates Blackjack game rules including H17, splits, doubles, insurance, and payouts.
    /// </summary>
    public class GameRules
    {
        private readonly Models.GameSettings _settings;

        public GameRules(Models.GameSettings settings)
        {
            _settings = settings;
        }

        #region Action Validation

        /// <summary>
        /// Checks if a player can hit on the given hand.
        /// </summary>
        public bool CanHit(Models.Hand hand)
        {
            // Can hit if not busted and not standing
            return !hand.IsBusted && 
                   hand.Status != Models.HandStatus.Busted && 
                   hand.Status != Models.HandStatus.Standing &&
                   hand.Status != Models.HandStatus.Blackjack;
        }

        /// <summary>
        /// Checks if a player can stand on the given hand.
        /// </summary>
        public bool CanStand(Models.Hand hand)
        {
            // Can always stand on an active hand
            return hand.Status == Models.HandStatus.Active && !hand.IsBusted;
        }

        /// <summary>
        /// Checks if a player can double down on the given hand.
        /// </summary>
        public bool CanDoubleDown(Models.Hand hand, Models.Player player, bool isSplitAce = false)
        {
            // Cannot double on split Aces
            if (isSplitAce)
                return false;

            // Must be exactly 2 cards (first action only)
            if (hand.Cards.Count != 2)
                return false;

            // Must have enough bankroll to double the bet
            if (player.Bankroll < hand.Bet)
                return false;

            // Hand must be active and not busted
            return hand.Status == Models.HandStatus.Active && !hand.IsBusted;
        }

        /// <summary>
        /// Checks if a player can split the given hand.
        /// </summary>
        public bool CanSplit(Models.Hand hand, Models.Player player)
        {
            // Must be a pair (2 cards of same value)
            if (!hand.IsPair)
                return false;

            // Check current split count (count how many hands the player has)
            int currentSplitCount = player.Hands.Count - 1; // -1 because original hand doesn't count as a split
            
            // Can split up to 3 times (4 hands total)
            if (currentSplitCount >= _settings.MaxSplits)
                return false;

            // Must have enough bankroll for the additional bet
            if (player.Bankroll < hand.Bet)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if insurance can be offered.
        /// </summary>
        public bool CanOfferInsurance(Models.Dealer dealer, Models.Player player, Models.Hand hand)
        {
            // Dealer must show an Ace
            if (dealer.UpCard?.Rank != Models.Rank.Ace)
                return false;

            // Player must have enough bankroll for insurance (half the bet)
            decimal insuranceCost = hand.Bet / 2;
            if (player.Bankroll < insuranceCost)
                return false;

            // Can only offer before any player actions
            return hand.Cards.Count == 2;
        }

        /// <summary>
        /// Checks if even money can be offered.
        /// </summary>
        public bool CanOfferEvenMoney(Models.Hand playerHand, Models.Dealer dealer)
        {
            // Player must have blackjack
            if (!playerHand.IsBlackjack)
                return false;

            // Dealer must show an Ace
            if (dealer.UpCard?.Rank != Models.Rank.Ace)
                return false;

            return true;
        }

        #endregion

        #region Split Rules

        /// <summary>
        /// Checks if a hand was created from splitting Aces.
        /// </summary>
        public bool IsSplitAceHand(Models.Hand hand)
        {
            // A split Ace hand has exactly 2 cards, one of which is an Ace from the split
            // This would need to be tracked elsewhere (e.g., a flag on Hand)
            // For now, we'll check if hand has an Ace and was likely from a split
            return hand.Cards.Count == 2 && 
                   hand.Cards.Any(c => c.Rank == Models.Rank.Ace) &&
                   !hand.IsBlackjack; // Split Aces with 10 don't count as blackjack
        }

        /// <summary>
        /// Executes a split operation on a hand.
        /// </summary>
        public (Models.Hand newHand1, Models.Hand newHand2) ExecuteSplit(Models.Hand originalHand, Models.Player player)
        {
            if (!CanSplit(originalHand, player))
            {
                throw new InvalidOperationException("Cannot split this hand.");
            }

            // Create two new hands
            var hand1 = new Models.Hand();
            var hand2 = new Models.Hand();

            // Split the cards
            hand1.AddCard(originalHand.Cards[0]);
            hand2.AddCard(originalHand.Cards[1]);

            // Each hand gets the same bet
            hand1.Bet = originalHand.Bet;
            hand2.Bet = originalHand.Bet;

            // Deduct the additional bet from player's bankroll
            player.Bankroll -= originalHand.Bet;

            return (hand1, hand2);
        }

        #endregion

        #region Payout Calculations

        /// <summary>
        /// Calculates the payout for a blackjack (3:2).
        /// </summary>
        public decimal CalculateBlackjackPayout(decimal bet)
        {
            // 3:2 payout means player gets back bet + (bet * 1.5)
            return bet + (bet * _settings.BlackjackPayout);
        }

        /// <summary>
        /// Calculates the payout for a regular win (1:1).
        /// </summary>
        public decimal CalculateWinPayout(decimal bet)
        {
            // 1:1 payout means player gets back bet + bet
            return bet * 2;
        }

        /// <summary>
        /// Calculates the payout for insurance (2:1).
        /// </summary>
        public decimal CalculateInsurancePayout(decimal insuranceBet)
        {
            // 2:1 payout means player gets back insurance bet + (insurance bet * 2)
            return insuranceBet + (insuranceBet * _settings.InsurancePayout);
        }

        /// <summary>
        /// Returns the bet on a push (tie).
        /// </summary>
        public decimal CalculatePushPayout(decimal bet)
        {
            // Return original bet
            return bet;
        }

        /// <summary>
        /// Calculates even money payout (1:1 on blackjack).
        /// </summary>
        public decimal CalculateEvenMoneyPayout(decimal bet)
        {
            // Even money is 1:1, so player gets back bet + bet
            return bet * 2;
        }

        /// <summary>
        /// Settles a hand by comparing player and dealer hands, returning the payout amount.
        /// </summary>
        public decimal SettleHand(Models.Hand playerHand, Models.Dealer dealer, bool insuranceTaken = false, decimal insuranceBet = 0)
        {
            decimal payout = 0;

            // Handle insurance payout if dealer has blackjack
            if (insuranceTaken && dealer.HasBlackjack)
            {
                payout += CalculateInsurancePayout(insuranceBet);
            }

            // If player busted, they lose (no payout)
            if (playerHand.IsBusted)
            {
                playerHand.Status = Models.HandStatus.Lost;
                return payout;
            }

            // If dealer busted, player wins
            if (dealer.Hand.IsBusted)
            {
                if (playerHand.IsBlackjack)
                {
                    payout += CalculateBlackjackPayout(playerHand.Bet);
                    playerHand.Status = Models.HandStatus.Blackjack;
                }
                else
                {
                    payout += CalculateWinPayout(playerHand.Bet);
                    playerHand.Status = Models.HandStatus.Won;
                }
                return payout;
            }

            // Compare hand values
            int playerTotal = playerHand.TotalValue;
            int dealerTotal = dealer.Hand.TotalValue;

            if (playerHand.IsBlackjack && !dealer.HasBlackjack)
            {
                // Player blackjack beats dealer non-blackjack
                payout += CalculateBlackjackPayout(playerHand.Bet);
                playerHand.Status = Models.HandStatus.Blackjack;
            }
            else if (dealer.HasBlackjack && !playerHand.IsBlackjack)
            {
                // Dealer blackjack beats player non-blackjack
                playerHand.Status = Models.HandStatus.Lost;
            }
            else if (playerTotal > dealerTotal)
            {
                // Player total is higher
                payout += CalculateWinPayout(playerHand.Bet);
                playerHand.Status = Models.HandStatus.Won;
            }
            else if (playerTotal < dealerTotal)
            {
                // Dealer total is higher
                playerHand.Status = Models.HandStatus.Lost;
            }
            else
            {
                // Push (tie)
                payout += CalculatePushPayout(playerHand.Bet);
                playerHand.Status = Models.HandStatus.Push;
            }

            return payout;
        }

        #endregion

        #region Even Money Logic

        /// <summary>
        /// Handles even money acceptance (immediate 1:1 payout for blackjack vs dealer Ace).
        /// </summary>
        public decimal AcceptEvenMoney(Models.Hand playerHand)
        {
            if (!playerHand.IsBlackjack)
            {
                throw new InvalidOperationException("Even money can only be taken on blackjack.");
            }

            // Pay 1:1 immediately
            return CalculateEvenMoneyPayout(playerHand.Bet);
        }

        #endregion

        #region Validation Helpers

        /// <summary>
        /// Validates that a bet is within table limits.
        /// </summary>
        public bool IsValidBet(decimal amount)
        {
            return _settings.IsValidBet(amount);
        }

        /// <summary>
        /// Gets the minimum bet amount.
        /// </summary>
        public decimal GetMinimumBet()
        {
            return _settings.TableMinimum;
        }

        /// <summary>
        /// Gets the maximum bet amount.
        /// </summary>
        public decimal GetMaximumBet()
        {
            return _settings.TableMaximum;
        }

        #endregion
    }
}
