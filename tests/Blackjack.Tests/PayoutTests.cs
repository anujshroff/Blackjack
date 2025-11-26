using Blackjack.Models;
using Blackjack.Services;
using Blackjack.Tests.Helpers;

namespace Blackjack.Tests;

/// <summary>
/// Unit tests for payout calculations including Blackjack 3:2, Win 1:1, Insurance 2:1, Push, and Even Money.
/// </summary>
public class PayoutTests
{
    private readonly GameRules _gameRules;
    private readonly GameSettings _settings;

    public PayoutTests()
    {
        _settings = new GameSettings();
        _gameRules = new GameRules(_settings);
    }

    #region Blackjack 3:2 Payout Tests

    [Theory]
    [InlineData(10, 25)]       // $10 bet → $25 payout (original $10 + $15 win)
    [InlineData(25, 62.50)]    // $25 bet → $62.50 payout
    [InlineData(100, 250)]     // $100 bet → $250 payout
    public void CalculateBlackjackPayout_ReturnsThreeToTwo(decimal bet, decimal expectedPayout)
    {
        var payout = _gameRules.CalculateBlackjackPayout(bet);

        Assert.Equal(expectedPayout, payout);
    }

    [Fact]
    public void CalculateBlackjackPayout_SmallBet_ReturnsCorrectPayout()
    {
        // $5 bet at 3:2 = $5 + $7.50 = $12.50
        var payout = _gameRules.CalculateBlackjackPayout(5m);

        Assert.Equal(12.50m, payout);
    }

    #endregion

    #region Win 1:1 Payout Tests

    [Theory]
    [InlineData(10, 20)]   // $10 bet → $20 payout (original $10 + $10 win)
    [InlineData(100, 200)] // $100 bet → $200 payout
    public void CalculateWinPayout_ReturnsOneToOne(decimal bet, decimal expectedPayout)
    {
        var payout = GameRules.CalculateWinPayout(bet);

        Assert.Equal(expectedPayout, payout);
    }

    [Fact]
    public void CalculateWinPayout_OddAmount_ReturnsCorrectPayout()
    {
        // $15 bet → $30 payout
        var payout = GameRules.CalculateWinPayout(15m);

        Assert.Equal(30m, payout);
    }

    #endregion

    #region Insurance 2:1 Payout Tests

    [Theory]
    [InlineData(5, 15)]   // $5 insurance bet → $15 payout (original $5 + $10 win)
    [InlineData(50, 150)] // $50 insurance bet → $150 payout
    public void CalculateInsurancePayout_ReturnsTwoToOne(decimal insuranceBet, decimal expectedPayout)
    {
        var payout = _gameRules.CalculateInsurancePayout(insuranceBet);

        Assert.Equal(expectedPayout, payout);
    }

    [Fact]
    public void CalculateInsurancePayout_MinimumInsurance_ReturnsCorrectPayout()
    {
        // $2.50 insurance bet → $7.50 payout
        var payout = _gameRules.CalculateInsurancePayout(2.50m);

        Assert.Equal(7.50m, payout);
    }

    #endregion

    #region Push Payout Tests

    [Theory]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(100)]
    public void CalculatePushPayout_ReturnsOriginalBet(decimal bet)
    {
        var payout = GameRules.CalculatePushPayout(bet);

        Assert.Equal(bet, payout);
    }

    #endregion

    #region Even Money Payout Tests

    [Fact]
    public void CalculateEvenMoneyPayout_TenDollarBlackjack_ReturnsTwenty()
    {
        // $10 blackjack with even money → $20 payout (1:1)
        var payout = GameRules.CalculateEvenMoneyPayout(10m);

        Assert.Equal(20m, payout);
    }

    [Theory]
    [InlineData(25, 50)]
    [InlineData(100, 200)]
    public void CalculateEvenMoneyPayout_ReturnsOneToOne(decimal bet, decimal expectedPayout)
    {
        var payout = GameRules.CalculateEvenMoneyPayout(bet);

        Assert.Equal(expectedPayout, payout);
    }

    [Fact]
    public void AcceptEvenMoney_WithBlackjack_ReturnsOneToOnePayout()
    {
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.King);
        hand.Bet = 50m;

        var payout = GameRules.AcceptEvenMoney(hand);

        Assert.Equal(100m, payout); // $50 * 2 = $100
    }

    [Fact]
    public void AcceptEvenMoney_WithoutBlackjack_ThrowsException()
    {
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Seven); // 17, not blackjack
        hand.Bet = 50m;

        Assert.Throws<InvalidOperationException>(() => GameRules.AcceptEvenMoney(hand));
    }

    #endregion

    #region Settlement Integration Tests

    [Fact]
    public void SettleHand_PlayerBlackjackDealerNot_PaysThreeToTwo()
    {
        var playerHand = CardHelper.CreateHand(Rank.Ace, Rank.King);
        playerHand.Bet = 20m;

        var dealer = new Dealer();
        dealer.AddCard(CardHelper.Ten());
        dealer.AddCard(CardHelper.Eight()); // Dealer has 18

        var payout = _gameRules.SettleHand(playerHand, dealer);

        Assert.Equal(50m, payout); // $20 * 2.5 = $50
        Assert.Equal(HandStatus.Blackjack, playerHand.Status);
    }

    [Fact]
    public void SettleHand_PlayerWinsRegular_PaysOneToOne()
    {
        var playerHand = CardHelper.CreateHand(Rank.Ten, Rank.Nine); // 19
        playerHand.Bet = 10m;

        var dealer = new Dealer();
        dealer.AddCard(CardHelper.Ten());
        dealer.AddCard(CardHelper.Seven()); // Dealer has 17

        var payout = _gameRules.SettleHand(playerHand, dealer);

        Assert.Equal(20m, payout); // $10 * 2 = $20
        Assert.Equal(HandStatus.Won, playerHand.Status);
    }

    [Fact]
    public void SettleHand_Push_ReturnsOriginalBet()
    {
        var playerHand = CardHelper.CreateHand(Rank.Ten, Rank.Eight); // 18
        playerHand.Bet = 25m;

        var dealer = new Dealer();
        dealer.AddCard(CardHelper.Ten());
        dealer.AddCard(CardHelper.Eight()); // Dealer has 18

        var payout = _gameRules.SettleHand(playerHand, dealer);

        Assert.Equal(25m, payout); // Original bet returned
        Assert.Equal(HandStatus.Push, playerHand.Status);
    }

    [Fact]
    public void SettleHand_PlayerBusted_PaysNothing()
    {
        var playerHand = CardHelper.CreateHand(Rank.Ten, Rank.Six, Rank.Seven); // 23 (busted)
        playerHand.Bet = 50m;

        var dealer = new Dealer();
        dealer.AddCard(CardHelper.Ten());
        dealer.AddCard(CardHelper.Seven());

        var payout = _gameRules.SettleHand(playerHand, dealer);

        Assert.Equal(0m, payout);
        Assert.Equal(HandStatus.Lost, playerHand.Status);
    }

    [Fact]
    public void SettleHand_DealerBusted_PlayerWins()
    {
        var playerHand = CardHelper.CreateHand(Rank.Ten, Rank.Seven); // 17
        playerHand.Bet = 30m;

        var dealer = new Dealer();
        dealer.AddCard(CardHelper.Ten());
        dealer.AddCard(CardHelper.Six());
        dealer.AddCard(CardHelper.Eight()); // Dealer has 24 (busted)

        var payout = _gameRules.SettleHand(playerHand, dealer);

        Assert.Equal(60m, payout); // $30 * 2 = $60
        Assert.Equal(HandStatus.Won, playerHand.Status);
    }

    [Fact]
    public void SettleHand_WithInsurance_DealerBlackjack_PaysInsurance()
    {
        var playerHand = CardHelper.CreateHand(Rank.Ten, Rank.Seven); // 17
        playerHand.Bet = 20m;

        var dealer = new Dealer();
        dealer.AddCard(CardHelper.Ace());
        dealer.AddCard(CardHelper.King()); // Dealer has blackjack

        // Insurance bet is half the original bet
        decimal insuranceBet = 10m;
        var payout = _gameRules.SettleHand(playerHand, dealer, insuranceTaken: true, insuranceBet: insuranceBet);

        // Player loses main bet, but insurance pays 2:1
        // Insurance payout = $10 + ($10 * 2) = $30
        Assert.Equal(30m, payout);
        Assert.Equal(HandStatus.Lost, playerHand.Status);
    }

    #endregion
}
