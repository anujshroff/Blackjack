using Blackjack.Models;
using Blackjack.Tests.Helpers;

namespace Blackjack.Tests;

/// <summary>
/// Unit tests for Hand evaluation logic including TotalValue, IsSoft, IsBlackjack, IsBusted, and IsPair.
/// </summary>
public class HandTests
{
    #region TotalValue Tests

    [Fact]
    public void TotalValue_SimpleHand_ReturnsCorrectSum()
    {
        // 7 + 5 = 12
        var hand = CardHelper.CreateHand(Rank.Seven, Rank.Five);

        Assert.Equal(12, hand.TotalValue);
    }

    [Fact]
    public void TotalValue_FaceCards_ReturnsTwenty()
    {
        // K + Q = 20
        var hand = CardHelper.CreateHand(Rank.King, Rank.Queen);

        Assert.Equal(20, hand.TotalValue);
    }

    [Fact]
    public void TotalValue_SingleAceAs11_ReturnsSeventeen()
    {
        // A + 6 = 17 (Ace counted as 11)
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Six);

        Assert.Equal(17, hand.TotalValue);
    }

    [Fact]
    public void TotalValue_AceAs1WhenWouldBust_ReturnsSixteen()
    {
        // A + 6 + 9 = 16 (Ace counted as 1 to avoid bust)
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Six, Rank.Nine);

        Assert.Equal(16, hand.TotalValue);
    }

    [Fact]
    public void TotalValue_TwoAces_ReturnsTwelve()
    {
        // A + A = 12 (one Ace as 11, one as 1)
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Ace);

        Assert.Equal(12, hand.TotalValue);
    }

    [Fact]
    public void TotalValue_TwoAcesAndNine_ReturnsTwentyOne()
    {
        // A + A + 9 = 21 (one Ace as 11, one as 1, plus 9)
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Ace, Rank.Nine);

        Assert.Equal(21, hand.TotalValue);
    }

    [Fact]
    public void TotalValue_ThreeAces_ReturnsThirteen()
    {
        // A + A + A = 13 (one Ace as 11, two as 1)
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Ace, Rank.Ace);

        Assert.Equal(13, hand.TotalValue);
    }

    [Fact]
    public void TotalValue_Blackjack_ReturnsTwentyOne()
    {
        // A + K = 21 (Blackjack)
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.King);

        Assert.Equal(21, hand.TotalValue);
    }

    #endregion

    #region IsSoft Tests

    [Theory]
    [InlineData(Rank.Ace, Rank.Two, 13)]   // A + 2 = 13 soft
    [InlineData(Rank.Ace, Rank.Six, 17)]   // A + 6 = 17 soft
    [InlineData(Rank.Ace, Rank.Seven, 18)] // A + 7 = 18 soft
    public void IsSoft_SoftHands_ReturnsTrue(Rank card1, Rank card2, int expectedTotal)
    {
        var hand = CardHelper.CreateHand(card1, card2);

        Assert.True(hand.IsSoft);
        Assert.Equal(expectedTotal, hand.TotalValue);
    }

    [Fact]
    public void IsSoft_AfterBustProtection_ReturnsFalse()
    {
        // A + 6 + 8 = 15 hard (Ace must be counted as 1)
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Six, Rank.Eight);

        Assert.False(hand.IsSoft);
        Assert.Equal(15, hand.TotalValue);
    }

    [Fact]
    public void IsSoft_MultiAceScenario_ReturnsTrue()
    {
        // A + A + 3 = 15 soft (one Ace as 11, one as 1, plus 3)
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Ace, Rank.Three);

        Assert.True(hand.IsSoft);
        Assert.Equal(15, hand.TotalValue);
    }

    [Fact]
    public void IsSoft_HardHand_ReturnsFalse()
    {
        // 10 + 7 = 17 hard (no Ace)
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Seven);

        Assert.False(hand.IsSoft);
    }

    #endregion

    #region IsBlackjack Tests

    [Theory]
    [InlineData(Rank.Ace, Rank.King)]   // A + 10-value = Blackjack
    public void IsBlackjack_TrueBlackjack_ReturnsTrue(Rank card1, Rank card2)
    {
        var hand = CardHelper.CreateHand(card1, card2);

        Assert.True(hand.IsBlackjack);
        Assert.Equal(21, hand.TotalValue);
    }

    [Fact]
    public void IsBlackjack_ThreeCardTwentyOne_ReturnsFalse()
    {
        // 7 + 7 + 7 = 21 but NOT blackjack
        var hand = CardHelper.CreateHand(Rank.Seven, Rank.Seven, Rank.Seven);

        Assert.False(hand.IsBlackjack);
        Assert.Equal(21, hand.TotalValue);
    }

    [Fact]
    public void IsBlackjack_AceWithTwoFives_ReturnsFalse()
    {
        // A + 5 + 5 = 21 but NOT blackjack (3 cards)
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Five, Rank.Five);

        Assert.False(hand.IsBlackjack);
        Assert.Equal(21, hand.TotalValue);
    }

    [Fact]
    public void IsBlackjack_TwoTens_ReturnsFalse()
    {
        // 10 + 10 = 20 (not blackjack, not 21)
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Ten);

        Assert.False(hand.IsBlackjack);
        Assert.Equal(20, hand.TotalValue);
    }

    [Fact]
    public void IsBlackjack_SplitAceWithTen_ReturnsFalse()
    {
        // Split Ace + 10 = 21 but NOT blackjack (from split)
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.King);
        hand.IsFromSplit = true;  // Mark as coming from a split

        Assert.False(hand.IsBlackjack);
        Assert.Equal(21, hand.TotalValue);
    }

    [Fact]
    public void IsBlackjack_SplitTensWithAce_ReturnsFalse()
    {
        // Split 10 + Ace = 21 but NOT blackjack (from split)
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Ace);
        hand.IsFromSplit = true;  // Mark as coming from a split

        Assert.False(hand.IsBlackjack);
        Assert.Equal(21, hand.TotalValue);
    }

    [Fact]
    public void IsBlackjack_NaturalAceKing_NotFromSplit_ReturnsTrue()
    {
        // Natural A + K = 21 (blackjack, not from split)
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.King);
        hand.IsFromSplit = false;  // Explicitly not from split

        Assert.True(hand.IsBlackjack);
        Assert.Equal(21, hand.TotalValue);
    }

    #endregion

    #region IsBusted Tests

    [Fact]
    public void IsBusted_JustBusted_ReturnsTrue()
    {
        // 10 + 6 + 7 = 23 (busted)
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Six, Rank.Seven);

        Assert.True(hand.IsBusted);
        Assert.Equal(23, hand.TotalValue);
    }

    [Fact]
    public void IsBusted_ExactlyTwentyOne_ReturnsFalse()
    {
        // 10 + 6 + 5 = 21 (not busted)
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Six, Rank.Five);

        Assert.False(hand.IsBusted);
        Assert.Equal(21, hand.TotalValue);
    }

    [Fact]
    public void IsBusted_AceSavesFromBust_ReturnsFalse()
    {
        // 10 + 6 + A = 17 (Ace counted as 1 to avoid bust)
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Six, Rank.Ace);

        Assert.False(hand.IsBusted);
        Assert.Equal(17, hand.TotalValue);
    }

    [Fact]
    public void IsBusted_UnderTwentyOne_ReturnsFalse()
    {
        // 5 + 5 = 10 (not busted)
        var hand = CardHelper.CreateHand(Rank.Five, Rank.Five);

        Assert.False(hand.IsBusted);
    }

    #endregion

    #region IsPair Tests

    [Fact]
    public void IsPair_EightsAgainstEights_ReturnsTrue()
    {
        // 8 + 8 = pair
        var hand = CardHelper.CreateHand(Rank.Eight, Rank.Eight);

        Assert.True(hand.IsPair);
    }

    [Fact]
    public void IsPair_KingsAgainstKings_ReturnsTrue()
    {
        // K + K = pair
        var hand = CardHelper.CreateHand(Rank.King, Rank.King);

        Assert.True(hand.IsPair);
    }

    [Fact]
    public void IsPair_MixedTenValues_ReturnsTrue()
    {
        // K + Q = pair (both 10-value)
        var hand = CardHelper.CreateHand(Rank.King, Rank.Queen);

        Assert.True(hand.IsPair);
    }

    [Fact]
    public void IsPair_TenAndJack_ReturnsTrue()
    {
        // 10 + J = pair (both 10-value)
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Jack);

        Assert.True(hand.IsPair);
    }

    [Fact]
    public void IsPair_ThreeCards_ReturnsFalse()
    {
        // 8 + 8 + 2 = not a pair (more than 2 cards)
        var hand = CardHelper.CreateHand(Rank.Eight, Rank.Eight, Rank.Two);

        Assert.False(hand.IsPair);
    }

    [Fact]
    public void IsPair_DifferentValues_ReturnsFalse()
    {
        // 7 + 8 = not a pair
        var hand = CardHelper.CreateHand(Rank.Seven, Rank.Eight);

        Assert.False(hand.IsPair);
    }

    [Fact]
    public void IsPair_AceAndKing_ReturnsFalse()
    {
        // A + K = not a pair (different values: 11 vs 10)
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.King);

        Assert.False(hand.IsPair);
    }

    #endregion
}
