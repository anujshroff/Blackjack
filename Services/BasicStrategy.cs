namespace Blackjack.Services
{
    /// <summary>
    /// Represents the recommended action from Basic Strategy.
    /// </summary>
    public enum PlayerAction
    {
        Hit,        // H - Take another card
        Stand,      // S - Keep current hand
        Double,     // D - Double bet and take one card (fallback to Hit if not allowed)
        Split       // P - Split pair into two hands
    }

    /// <summary>
    /// Implements optimal Basic Strategy for Blackjack with H17 rules (dealer hits soft 17).
    /// Based on mathematically proven optimal play for 6-deck shoe.
    /// </summary>
    public class BasicStrategy
    {
        // Hard totals strategy: Dictionary<(playerTotal, dealerValue), action>
        private readonly Dictionary<(int, int), PlayerAction> _hardTotalsStrategy;
        
        // Soft totals strategy: Dictionary<(playerTotal, dealerValue), action>
        private readonly Dictionary<(int, int), PlayerAction> _softTotalsStrategy;
        
        // Pair splitting strategy: Dictionary<(pairValue, dealerValue), action>
        private readonly Dictionary<(int, int), PlayerAction> _pairStrategy;

        public BasicStrategy()
        {
            _hardTotalsStrategy = InitializeHardTotalsStrategy();
            _softTotalsStrategy = InitializeSoftTotalsStrategy();
            _pairStrategy = InitializePairStrategy();
        }

        /// <summary>
        /// Gets the recommended action for a given hand against dealer's up card.
        /// </summary>
        public PlayerAction GetRecommendedAction(Models.Hand playerHand, Models.Card dealerUpCard)
        {
            int dealerValue = GetDealerValue(dealerUpCard);
            
            // Check for pairs first (must be exactly 2 cards of same value)
            if (playerHand.IsPair)
            {
                return GetPairAction(playerHand.Cards[0].Value, dealerValue);
            }
            
            // Check for soft hands (Ace counted as 11)
            if (playerHand.IsSoft)
            {
                return GetSoftTotalAction(playerHand.TotalValue, dealerValue);
            }
            
            // Default to hard totals
            return GetHardTotalAction(playerHand.TotalValue, dealerValue);
        }

        /// <summary>
        /// Gets recommended action for hard totals.
        /// </summary>
        private PlayerAction GetHardTotalAction(int total, int dealerValue)
        {
            if (_hardTotalsStrategy.TryGetValue((total, dealerValue), out var action))
            {
                return action;
            }
            
            // Fallback logic
            if (total >= 17) return PlayerAction.Stand;
            if (total <= 11) return PlayerAction.Hit;
            return PlayerAction.Hit;
        }

        /// <summary>
        /// Gets recommended action for soft totals.
        /// </summary>
        private PlayerAction GetSoftTotalAction(int total, int dealerValue)
        {
            if (_softTotalsStrategy.TryGetValue((total, dealerValue), out var action))
            {
                return action;
            }
            
            // Fallback: soft 19-21 stand, otherwise hit
            return total >= 19 ? PlayerAction.Stand : PlayerAction.Hit;
        }

        /// <summary>
        /// Gets recommended action for pairs.
        /// </summary>
        private PlayerAction GetPairAction(int pairValue, int dealerValue)
        {
            if (_pairStrategy.TryGetValue((pairValue, dealerValue), out var action))
            {
                return action;
            }
            
            // Fallback: don't split
            int total = pairValue * 2;
            return total >= 17 ? PlayerAction.Stand : PlayerAction.Hit;
        }

        /// <summary>
        /// Converts dealer's card to value for strategy lookup (Ace = 11).
        /// </summary>
        private int GetDealerValue(Models.Card card)
        {
            // For strategy tables, Ace is represented as 11
            return card.Rank == Models.Rank.Ace ? 11 : card.Value;
        }

        /// <summary>
        /// Initializes the hard totals strategy table for H17 rules.
        /// </summary>
        private Dictionary<(int, int), PlayerAction> InitializeHardTotalsStrategy()
        {
            var strategy = new Dictionary<(int, int), PlayerAction>();
            
            // Hard 17-20: Always Stand
            for (int total = 17; total <= 20; total++)
            {
                for (int dealer = 2; dealer <= 11; dealer++)
                {
                    strategy[(total, dealer)] = PlayerAction.Stand;
                }
            }
            
            // Hard 16: Stand on 2-6, Hit on 7-A
            for (int dealer = 2; dealer <= 6; dealer++)
                strategy[(16, dealer)] = PlayerAction.Stand;
            for (int dealer = 7; dealer <= 11; dealer++)
                strategy[(16, dealer)] = PlayerAction.Hit;
            
            // Hard 15: Stand on 2-6, Hit on 7-A
            for (int dealer = 2; dealer <= 6; dealer++)
                strategy[(15, dealer)] = PlayerAction.Stand;
            for (int dealer = 7; dealer <= 11; dealer++)
                strategy[(15, dealer)] = PlayerAction.Hit;
            
            // Hard 13-14: Stand on 2-6, Hit on 7-A
            for (int total = 13; total <= 14; total++)
            {
                for (int dealer = 2; dealer <= 6; dealer++)
                    strategy[(total, dealer)] = PlayerAction.Stand;
                for (int dealer = 7; dealer <= 11; dealer++)
                    strategy[(total, dealer)] = PlayerAction.Hit;
            }
            
            // Hard 12: Hit on 2-3, Stand on 4-6, Hit on 7-A
            strategy[(12, 2)] = PlayerAction.Hit;
            strategy[(12, 3)] = PlayerAction.Hit;
            for (int dealer = 4; dealer <= 6; dealer++)
                strategy[(12, dealer)] = PlayerAction.Stand;
            for (int dealer = 7; dealer <= 11; dealer++)
                strategy[(12, dealer)] = PlayerAction.Hit;
            
            // Hard 11: Always Double
            for (int dealer = 2; dealer <= 11; dealer++)
                strategy[(11, dealer)] = PlayerAction.Double;
            
            // Hard 10: Double on 2-9, Hit on 10-A
            for (int dealer = 2; dealer <= 9; dealer++)
                strategy[(10, dealer)] = PlayerAction.Double;
            strategy[(10, 10)] = PlayerAction.Hit;
            strategy[(10, 11)] = PlayerAction.Hit;
            
            // Hard 9: Hit on 2, Double on 3-6, Hit on 7-A
            strategy[(9, 2)] = PlayerAction.Hit;
            for (int dealer = 3; dealer <= 6; dealer++)
                strategy[(9, dealer)] = PlayerAction.Double;
            for (int dealer = 7; dealer <= 11; dealer++)
                strategy[(9, dealer)] = PlayerAction.Hit;
            
            // Hard 5-8: Always Hit
            for (int total = 5; total <= 8; total++)
            {
                for (int dealer = 2; dealer <= 11; dealer++)
                {
                    strategy[(total, dealer)] = PlayerAction.Hit;
                }
            }
            
            return strategy;
        }

        /// <summary>
        /// Initializes the soft totals strategy table for H17 rules.
        /// </summary>
        private Dictionary<(int, int), PlayerAction> InitializeSoftTotalsStrategy()
        {
            var strategy = new Dictionary<(int, int), PlayerAction>();
            
            // Soft 20 (A,9): Always Stand
            for (int dealer = 2; dealer <= 11; dealer++)
                strategy[(20, dealer)] = PlayerAction.Stand;
            
            // Soft 19 (A,8): Stand on all, Double only on 6
            for (int dealer = 2; dealer <= 11; dealer++)
                strategy[(19, dealer)] = PlayerAction.Stand;
            strategy[(19, 6)] = PlayerAction.Double; // Double on 6
            
            // Soft 18 (A,7): Double on 2-6, Stand on 7-8, Hit on 9-A
            for (int dealer = 2; dealer <= 6; dealer++)
                strategy[(18, dealer)] = PlayerAction.Double;
            strategy[(18, 7)] = PlayerAction.Stand;
            strategy[(18, 8)] = PlayerAction.Stand;
            strategy[(18, 9)] = PlayerAction.Hit;
            strategy[(18, 10)] = PlayerAction.Hit;
            strategy[(18, 11)] = PlayerAction.Hit;
            
            // Soft 17 (A,6): Hit on 2, Double on 3-6, Hit on 7-A
            strategy[(17, 2)] = PlayerAction.Hit;
            for (int dealer = 3; dealer <= 6; dealer++)
                strategy[(17, dealer)] = PlayerAction.Double;
            for (int dealer = 7; dealer <= 11; dealer++)
                strategy[(17, dealer)] = PlayerAction.Hit;
            
            // Soft 16 (A,5): Hit on 2-3, Double on 4-6, Hit on 7-A
            strategy[(16, 2)] = PlayerAction.Hit;
            strategy[(16, 3)] = PlayerAction.Hit;
            for (int dealer = 4; dealer <= 6; dealer++)
                strategy[(16, dealer)] = PlayerAction.Double;
            for (int dealer = 7; dealer <= 11; dealer++)
                strategy[(16, dealer)] = PlayerAction.Hit;
            
            // Soft 15 (A,4): Hit on 2-3, Double on 4-6, Hit on 7-A
            strategy[(15, 2)] = PlayerAction.Hit;
            strategy[(15, 3)] = PlayerAction.Hit;
            for (int dealer = 4; dealer <= 6; dealer++)
                strategy[(15, dealer)] = PlayerAction.Double;
            for (int dealer = 7; dealer <= 11; dealer++)
                strategy[(15, dealer)] = PlayerAction.Hit;
            
            // Soft 14 (A,3): Hit on 2-4, Double on 5-6, Hit on 7-A
            for (int dealer = 2; dealer <= 4; dealer++)
                strategy[(14, dealer)] = PlayerAction.Hit;
            strategy[(14, 5)] = PlayerAction.Double;
            strategy[(14, 6)] = PlayerAction.Double;
            for (int dealer = 7; dealer <= 11; dealer++)
                strategy[(14, dealer)] = PlayerAction.Hit;
            
            // Soft 13 (A,2): Hit on 2-4, Double on 5-6, Hit on 7-A
            for (int dealer = 2; dealer <= 4; dealer++)
                strategy[(13, dealer)] = PlayerAction.Hit;
            strategy[(13, 5)] = PlayerAction.Double;
            strategy[(13, 6)] = PlayerAction.Double;
            for (int dealer = 7; dealer <= 11; dealer++)
                strategy[(13, dealer)] = PlayerAction.Hit;
            
            return strategy;
        }

        /// <summary>
        /// Initializes the pair splitting strategy table for H17 rules.
        /// </summary>
        private Dictionary<(int, int), PlayerAction> InitializePairStrategy()
        {
            var strategy = new Dictionary<(int, int), PlayerAction>();
            
            // Pair of Aces (11,11): Always Split
            for (int dealer = 2; dealer <= 11; dealer++)
                strategy[(11, dealer)] = PlayerAction.Split;
            
            // Pair of 10s: Never Split (Always Stand)
            for (int dealer = 2; dealer <= 11; dealer++)
                strategy[(10, dealer)] = PlayerAction.Stand;
            
            // Pair of 9s: Split on 2-6, 8-9; Stand on 7, 10, A
            for (int dealer = 2; dealer <= 6; dealer++)
                strategy[(9, dealer)] = PlayerAction.Split;
            strategy[(9, 7)] = PlayerAction.Stand;
            strategy[(9, 8)] = PlayerAction.Split;
            strategy[(9, 9)] = PlayerAction.Split;
            strategy[(9, 10)] = PlayerAction.Stand;
            strategy[(9, 11)] = PlayerAction.Stand;
            
            // Pair of 8s: Always Split
            for (int dealer = 2; dealer <= 11; dealer++)
                strategy[(8, dealer)] = PlayerAction.Split;
            
            // Pair of 7s: Split on 2-7, Hit on 8-A
            for (int dealer = 2; dealer <= 7; dealer++)
                strategy[(7, dealer)] = PlayerAction.Split;
            for (int dealer = 8; dealer <= 11; dealer++)
                strategy[(7, dealer)] = PlayerAction.Hit;
            
            // Pair of 6s: Split on 2-6, Hit on 7-A
            for (int dealer = 2; dealer <= 6; dealer++)
                strategy[(6, dealer)] = PlayerAction.Split;
            for (int dealer = 7; dealer <= 11; dealer++)
                strategy[(6, dealer)] = PlayerAction.Hit;
            
            // Pair of 5s: Double on 2-9, Hit on 10-A (Never Split)
            for (int dealer = 2; dealer <= 9; dealer++)
                strategy[(5, dealer)] = PlayerAction.Double;
            strategy[(5, 10)] = PlayerAction.Hit;
            strategy[(5, 11)] = PlayerAction.Hit;
            
            // Pair of 4s: Hit on 2-4, Split on 5-6, Hit on 7-A
            for (int dealer = 2; dealer <= 4; dealer++)
                strategy[(4, dealer)] = PlayerAction.Hit;
            strategy[(4, 5)] = PlayerAction.Split;
            strategy[(4, 6)] = PlayerAction.Split;
            for (int dealer = 7; dealer <= 11; dealer++)
                strategy[(4, dealer)] = PlayerAction.Hit;
            
            // Pair of 3s: Split on 2-7, Hit on 8-A
            for (int dealer = 2; dealer <= 7; dealer++)
                strategy[(3, dealer)] = PlayerAction.Split;
            for (int dealer = 8; dealer <= 11; dealer++)
                strategy[(3, dealer)] = PlayerAction.Hit;
            
            // Pair of 2s: Split on 2-7, Hit on 8-A
            for (int dealer = 2; dealer <= 7; dealer++)
                strategy[(2, dealer)] = PlayerAction.Split;
            for (int dealer = 8; dealer <= 11; dealer++)
                strategy[(2, dealer)] = PlayerAction.Hit;
            
            return strategy;
        }
    }
}
