namespace Blackjack.Services
{
    /// <summary>
    /// Handles AI player betting logic with varied and realistic betting patterns.
    /// </summary>
    public class AIBettingService(Models.GameSettings settings)
    {
        private readonly Random _random = new();
        private readonly Models.GameSettings _settings = settings;

        /// <summary>
        /// Generates a bet amount for an AI player based on table limits and bankroll.
        /// </summary>
        /// <param name="bankroll">The AI player's current bankroll.</param>
        /// <returns>A valid bet amount.</returns>
        public decimal GenerateBet(decimal bankroll)
        {
            // If AI is bankrupt, return 0
            if (bankroll <= 0)
            {
                return 0;
            }

            // If AI can't afford minimum bet, return 0
            if (bankroll < _settings.TableMinimum)
            {
                return 0;
            }

            // Calculate maximum affordable bet (within table limits)
            decimal maxAffordable = Math.Min(bankroll, _settings.TableMaximum);

            // AI betting strategy: bet between 1x-3x table minimum (with randomization)
            decimal baseBet = _settings.TableMinimum;

            // 60% chance to bet exactly table minimum
            // 30% chance to bet 2x table minimum
            // 10% chance to bet 3x table minimum
            double roll = _random.NextDouble();

            decimal targetBet;
            if (roll < 0.6)
            {
                targetBet = baseBet; // 1x minimum
            }
            else if (roll < 0.9)
            {
                targetBet = baseBet * 2; // 2x minimum
            }
            else
            {
                targetBet = baseBet * 3; // 3x minimum
            }

            // Ensure bet doesn't exceed what AI can afford
            targetBet = Math.Min(targetBet, maxAffordable);

            // Round to nearest chip denomination for realism
            targetBet = RoundToNearestChipDenomination(targetBet);

            // Final validation
            if (targetBet < _settings.TableMinimum)
            {
                targetBet = _settings.TableMinimum;
            }

            if (targetBet > _settings.TableMaximum)
            {
                targetBet = _settings.TableMaximum;
            }

            if (targetBet > bankroll)
            {
                targetBet = _settings.TableMinimum;
            }

            return targetBet;
        }

        /// <summary>
        /// Rounds a bet amount to the nearest common chip denomination ($1, $5, $25, $100, $500).
        /// </summary>
        private static decimal RoundToNearestChipDenomination(decimal amount)
        {
            decimal[] denominations = [1m, 5m, 25m, 100m, 500m];

            // Find the closest denomination
            decimal closest = denominations[0];
            decimal minDifference = Math.Abs(amount - closest);

            foreach (var denom in denominations)
            {
                decimal difference = Math.Abs(amount - denom);
                if (difference < minDifference)
                {
                    minDifference = difference;
                    closest = denom;
                }
            }

            // Round to multiples of the closest denomination
            if (closest >= 100m)
            {
                // For $100+, round to nearest $100
                return Math.Round(amount / 100m) * 100m;
            }
            else if (closest >= 25m)
            {
                // For $25+, round to nearest $25
                return Math.Round(amount / 25m) * 25m;
            }
            else if (closest >= 5m)
            {
                // For $5+, round to nearest $5
                return Math.Round(amount / 5m) * 5m;
            }
            else
            {
                // For small amounts, round to nearest $1
                return Math.Round(amount);
            }
        }
    }
}
