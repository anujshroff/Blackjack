using Blackjack.Models;
using Blackjack.Services;
using Blackjack.Tests.Helpers;

namespace Blackjack.Tests;

/// <summary>
/// Unit tests for Basic Strategy recommendations including hard totals, soft totals, and pairs.
/// </summary>
public class BasicStrategyTests
{
    private readonly BasicStrategy _strategy;

    public BasicStrategyTests()
    {
        _strategy = new BasicStrategy();
    }

    #region Hard Totals Tests

    [Fact]
    public void HardTotal_EightVsSix_ReturnsHit()
    {
        var hand = CardHelper.CreateHand(Rank.Five, Rank.Three); // 8
        var dealerUpCard = CardHelper.Six();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Hit, action);
    }

    [Theory]
    [InlineData(2)]   // Dealer shows 2
    [InlineData(6)]   // Dealer shows 6
    [InlineData(10)]  // Dealer shows 10
    [InlineData(11)]  // Dealer shows Ace
    public void HardTotal_ElevenVsAny_ReturnsDouble(int dealerValue)
    {
        var hand = CardHelper.CreateHand(Rank.Six, Rank.Five); // 11
        var dealerUpCard = CreateDealerCard(dealerValue);

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Double, action);
    }

    [Fact]
    public void HardTotal_TwelveVsTwo_ReturnsHit()
    {
        var hand = CardHelper.CreateHand(Rank.Seven, Rank.Five); // 12
        var dealerUpCard = CardHelper.Two();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Hit, action);
    }

    [Fact]
    public void HardTotal_TwelveVsFour_ReturnsStand()
    {
        var hand = CardHelper.CreateHand(Rank.Seven, Rank.Five); // 12
        var dealerUpCard = CardHelper.Four();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Stand, action);
    }

    [Fact]
    public void HardTotal_SixteenVsTen_ReturnsHit()
    {
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Six); // 16
        var dealerUpCard = CardHelper.Ten();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Hit, action);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(7)]
    [InlineData(10)]
    [InlineData(11)]
    public void HardTotal_SeventeenPlusVsAny_ReturnsStand(int dealerValue)
    {
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Seven); // 17
        var dealerUpCard = CreateDealerCard(dealerValue);

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Stand, action);
    }

    [Fact]
    public void HardTotal_TenVsNine_ReturnsDouble()
    {
        var hand = CardHelper.CreateHand(Rank.Six, Rank.Four); // 10
        var dealerUpCard = CardHelper.Nine();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Double, action);
    }

    [Fact]
    public void HardTotal_TenVsTen_ReturnsHit()
    {
        var hand = CardHelper.CreateHand(Rank.Six, Rank.Four); // 10
        var dealerUpCard = CardHelper.Ten();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Hit, action);
    }

    #endregion

    #region Soft Totals Tests

    [Fact]
    public void SoftTotal_ThirteenVsFive_ReturnsDouble()
    {
        // A + 2 = 13 soft
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Two);
        var dealerUpCard = CardHelper.Five();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Double, action);
    }

    [Fact]
    public void SoftTotal_SeventeenVsThree_ReturnsDouble()
    {
        // A + 6 = 17 soft
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Six);
        var dealerUpCard = CardHelper.Three();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Double, action);
    }

    [Fact]
    public void SoftTotal_EighteenVsNine_ReturnsHit()
    {
        // A + 7 = 18 soft
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Seven);
        var dealerUpCard = CardHelper.Nine();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Hit, action);
    }

    [Fact]
    public void SoftTotal_EighteenVsTwo_ReturnsDouble()
    {
        // A + 7 = 18 soft
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Seven);
        var dealerUpCard = CardHelper.Two();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Double, action);
    }

    [Fact]
    public void SoftTotal_NineteenVsSix_ReturnsDouble()
    {
        // A + 8 = 19 soft
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Eight);
        var dealerUpCard = CardHelper.Six();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Double, action);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(7)]
    [InlineData(10)]
    [InlineData(11)]
    public void SoftTotal_NineteenVsOthers_ReturnsStand(int dealerValue)
    {
        // A + 8 = 19 soft - Stand on all except 6 (where we double)
        if (dealerValue == 6) return; // Skip 6, that's a double

        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Eight);
        var dealerUpCard = CreateDealerCard(dealerValue);

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Stand, action);
    }

    [Fact]
    public void SoftTotal_SeventeenVsTwo_ReturnsHit()
    {
        // A + 6 = 17 soft - Hit vs 2
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Six);
        var dealerUpCard = CardHelper.Two();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Hit, action);
    }

    #endregion

    #region Pair Tests

    [Theory]
    [InlineData(2)]
    [InlineData(6)]
    [InlineData(10)]
    [InlineData(11)]
    public void Pair_AcesVsAny_ReturnsSplit(int dealerValue)
    {
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Ace);
        var dealerUpCard = CreateDealerCard(dealerValue);

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Split, action);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(6)]
    [InlineData(10)]
    [InlineData(11)]
    public void Pair_EightsVsAny_ReturnsSplit(int dealerValue)
    {
        var hand = CardHelper.CreateHand(Rank.Eight, Rank.Eight);
        var dealerUpCard = CreateDealerCard(dealerValue);

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Split, action);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(6)]
    [InlineData(10)]
    [InlineData(11)]
    public void Pair_TensVsAny_ReturnsStand(int dealerValue)
    {
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Ten);
        var dealerUpCard = CreateDealerCard(dealerValue);

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Stand, action);
    }

    [Fact]
    public void Pair_FivesVsNine_ReturnsDouble()
    {
        // 5-5 vs 9 = Double (treat as 10, not a split)
        var hand = CardHelper.CreateHand(Rank.Five, Rank.Five);
        var dealerUpCard = CardHelper.Nine();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Double, action);
    }

    [Fact]
    public void Pair_NinesVsSeven_ReturnsStand()
    {
        // 9-9 vs 7 = Stand
        var hand = CardHelper.CreateHand(Rank.Nine, Rank.Nine);
        var dealerUpCard = CardHelper.Seven();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Stand, action);
    }

    [Fact]
    public void Pair_FoursVsFive_ReturnsSplit()
    {
        // 4-4 vs 5 = Split
        var hand = CardHelper.CreateHand(Rank.Four, Rank.Four);
        var dealerUpCard = CardHelper.Five();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Split, action);
    }

    [Fact]
    public void Pair_TwosVsEight_ReturnsHit()
    {
        // 2-2 vs 8 = Hit
        var hand = CardHelper.CreateHand(Rank.Two, Rank.Two);
        var dealerUpCard = CardHelper.Eight();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Hit, action);
    }

    [Fact]
    public void Pair_SixesVsSix_ReturnsSplit()
    {
        // 6-6 vs 6 = Split
        var hand = CardHelper.CreateHand(Rank.Six, Rank.Six);
        var dealerUpCard = CardHelper.Six();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Split, action);
    }

    [Fact]
    public void Pair_SixesVsSeven_ReturnsHit()
    {
        // 6-6 vs 7 = Hit
        var hand = CardHelper.CreateHand(Rank.Six, Rank.Six);
        var dealerUpCard = CardHelper.Seven();

        var action = _strategy.GetRecommendedAction(hand, dealerUpCard);

        Assert.Equal(PlayerAction.Hit, action);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a dealer up card based on numeric value (2-11, where 11 = Ace).
    /// </summary>
    private static Card CreateDealerCard(int value)
    {
        return value switch
        {
            2 => CardHelper.Two(),
            3 => CardHelper.Three(),
            4 => CardHelper.Four(),
            5 => CardHelper.Five(),
            6 => CardHelper.Six(),
            7 => CardHelper.Seven(),
            8 => CardHelper.Eight(),
            9 => CardHelper.Nine(),
            10 => CardHelper.Ten(),
            11 => CardHelper.Ace(),
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
    }

    #endregion
}
