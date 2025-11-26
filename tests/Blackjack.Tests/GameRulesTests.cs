using Blackjack.Models;
using Blackjack.Services;
using Blackjack.Tests.Helpers;

namespace Blackjack.Tests;

/// <summary>
/// Unit tests for game rules validation including CanSplit, CanDoubleDown, CanHit, and CanStand.
/// </summary>
public class GameRulesTests
{
    private readonly GameSettings _settings;
    private readonly GameRules _gameRules;

    public GameRulesTests()
    {
        _settings = new GameSettings();
        _gameRules = new GameRules(_settings);
    }

    #region CanSplit Tests

    [Fact]
    public void CanSplit_ValidPairWithSufficientBankroll_ReturnsTrue()
    {
        var hand = CardHelper.CreateHand(Rank.Eight, Rank.Eight);
        hand.Bet = 25m;

        var player = new Player("Test", 1, 1000m);
        player.Hands[0] = hand;

        Assert.True(_gameRules.CanSplit(hand, player));
    }

    [Fact]
    public void CanSplit_MixedTenValues_ReturnsTrue()
    {
        // K + Q are both 10-value, should be splittable
        var hand = CardHelper.CreateHand(Rank.King, Rank.Queen);
        hand.Bet = 25m;

        var player = new Player("Test", 1, 1000m);
        player.Hands[0] = hand;

        Assert.True(_gameRules.CanSplit(hand, player));
    }

    [Fact]
    public void CanSplit_NonPair_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Seven, Rank.Eight);
        hand.Bet = 25m;

        var player = new Player("Test", 1, 1000m);
        player.Hands[0] = hand;

        Assert.False(_gameRules.CanSplit(hand, player));
    }

    [Fact]
    public void CanSplit_MaxSplitsReached_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Eight, Rank.Eight);
        hand.Bet = 25m;

        var player = new Player("Test", 1, 1000m);
        // Simulate 3 splits already done (4 hands total, max splits is 3)
        player.Hands.Clear();
        player.Hands.Add(new Hand());
        player.Hands.Add(new Hand());
        player.Hands.Add(new Hand());
        player.Hands.Add(hand); // 4 hands = 3 splits done

        Assert.False(_gameRules.CanSplit(hand, player));
    }

    [Fact]
    public void CanSplit_InsufficientBankroll_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Eight, Rank.Eight);
        hand.Bet = 100m;

        var player = new Player("Test", 1, 50m); // Not enough for another 100 bet
        player.Hands[0] = hand;

        Assert.False(_gameRules.CanSplit(hand, player));
    }

    [Fact]
    public void CanSplit_AcePair_ReturnsTrue()
    {
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Ace);
        hand.Bet = 25m;

        var player = new Player("Test", 1, 1000m);
        player.Hands[0] = hand;

        Assert.True(_gameRules.CanSplit(hand, player));
    }

    #endregion

    #region CanDoubleDown Tests

    [Fact]
    public void CanDoubleDown_FirstTwoCardsWithSufficientBankroll_ReturnsTrue()
    {
        var hand = CardHelper.CreateHand(Rank.Five, Rank.Six); // 11
        hand.Bet = 25m;

        var player = new Player("Test", 1, 100m);
        player.Hands[0] = hand;

        Assert.True(_gameRules.CanDoubleDown(hand, player));
    }

    [Fact]
    public void CanDoubleDown_AfterSplit_DASEnabled_ReturnsTrue()
    {
        // Default settings have DoubleAfterSplit = true
        var hand = CardHelper.CreateHand(Rank.Eight, Rank.Three); // 11
        hand.Bet = 25m;

        var player = new Player("Test", 1, 100m);
        player.Hands[0] = hand;

        Assert.True(_gameRules.CanDoubleDown(hand, player, isSplitHand: true));
    }

    [Fact]
    public void CanDoubleDown_AfterSplit_DASDisabled_ReturnsFalse()
    {
        var settings = new GameSettings { DoubleAfterSplit = false };
        var gameRules = new GameRules(settings);

        var hand = CardHelper.CreateHand(Rank.Eight, Rank.Three); // 11
        hand.Bet = 25m;

        var player = new Player("Test", 1, 100m);
        player.Hands[0] = hand;

        Assert.False(gameRules.CanDoubleDown(hand, player, isSplitHand: true));
    }

    [Fact]
    public void CanDoubleDown_AfterHit_ReturnsFalse()
    {
        // After a hit, hand has more than 2 cards
        var hand = CardHelper.CreateHand(Rank.Five, Rank.Three, Rank.Two); // 3 cards
        hand.Bet = 25m;

        var player = new Player("Test", 1, 100m);
        player.Hands[0] = hand;

        Assert.False(_gameRules.CanDoubleDown(hand, player));
    }

    [Fact]
    public void CanDoubleDown_AfterSplitAces_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Five);
        hand.Bet = 25m;

        var player = new Player("Test", 1, 100m);
        player.Hands[0] = hand;

        // Split Aces cannot double
        Assert.False(_gameRules.CanDoubleDown(hand, player, isSplitHand: true, isSplitAce: true));
    }

    [Fact]
    public void CanDoubleDown_InsufficientBankroll_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Five, Rank.Six);
        hand.Bet = 100m;

        var player = new Player("Test", 1, 50m); // Not enough to double
        player.Hands[0] = hand;

        Assert.False(_gameRules.CanDoubleDown(hand, player));
    }

    [Fact]
    public void CanDoubleDown_BustedHand_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Six, Rank.Seven); // 23 - busted
        hand.Bet = 25m;

        var player = new Player("Test", 1, 100m);
        player.Hands[0] = hand;

        Assert.False(_gameRules.CanDoubleDown(hand, player));
    }

    #endregion

    #region CanHit Tests

    [Fact]
    public void CanHit_ActiveHand_ReturnsTrue()
    {
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Six); // 16
        hand.Status = HandStatus.Active;

        Assert.True(GameRules.CanHit(hand));
    }

    [Fact]
    public void CanHit_SoftSeventeen_ReturnsTrue()
    {
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.Six); // Soft 17
        hand.Status = HandStatus.Active;

        Assert.True(GameRules.CanHit(hand));
        Assert.True(hand.IsSoft);
    }

    [Fact]
    public void CanHit_BustedHand_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Six, Rank.Seven); // 23 - busted

        Assert.False(GameRules.CanHit(hand));
    }

    [Fact]
    public void CanHit_StandingHand_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Seven);
        hand.Status = HandStatus.Standing;

        Assert.False(GameRules.CanHit(hand));
    }

    [Fact]
    public void CanHit_BlackjackHand_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.King);
        hand.Status = HandStatus.Blackjack;

        Assert.False(GameRules.CanHit(hand));
    }

    [Fact]
    public void CanHit_WonHand_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Nine);
        hand.Status = HandStatus.Won;

        // Won hands are no longer active
        Assert.False(GameRules.CanHit(hand));
    }

    #endregion

    #region CanStand Tests

    [Fact]
    public void CanStand_ActiveNonBustedHand_ReturnsTrue()
    {
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Six);
        hand.Status = HandStatus.Active;

        Assert.True(GameRules.CanStand(hand));
    }

    [Fact]
    public void CanStand_AnyActiveHand_ReturnsTrue()
    {
        var hand = CardHelper.CreateHand(Rank.Two, Rank.Three); // Even low value
        hand.Status = HandStatus.Active;

        Assert.True(GameRules.CanStand(hand));
    }

    [Fact]
    public void CanStand_BustedHand_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Six, Rank.Eight); // 24 - busted

        Assert.False(GameRules.CanStand(hand));
    }

    [Fact]
    public void CanStand_AlreadyStanding_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Seven);
        hand.Status = HandStatus.Standing;

        Assert.False(GameRules.CanStand(hand));
    }

    [Fact]
    public void CanStand_BlackjackHand_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Ace, Rank.King);
        hand.Status = HandStatus.Blackjack;

        // Blackjack hands don't need to stand - they auto-win
        Assert.False(GameRules.CanStand(hand));
    }

    [Fact]
    public void CanStand_LostHand_ReturnsFalse()
    {
        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Six);
        hand.Status = HandStatus.Lost;

        Assert.False(GameRules.CanStand(hand));
    }

    #endregion

    #region Insurance Validation Tests

    [Fact]
    public void CanOfferInsurance_DealerShowsAce_ReturnsTrue()
    {
        var dealer = new Dealer();
        dealer.AddCard(CardHelper.Ace());
        dealer.AddCard(CardHelper.King());

        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Seven);
        hand.Bet = 20m;

        var player = new Player("Test", 1, 100m);
        player.Hands[0] = hand;

        Assert.True(GameRules.CanOfferInsurance(dealer, player, hand));
    }

    [Fact]
    public void CanOfferInsurance_DealerShowsTen_ReturnsFalse()
    {
        var dealer = new Dealer();
        dealer.AddCard(CardHelper.Ten());
        dealer.AddCard(CardHelper.Ace());

        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Seven);
        hand.Bet = 20m;

        var player = new Player("Test", 1, 100m);
        player.Hands[0] = hand;

        Assert.False(GameRules.CanOfferInsurance(dealer, player, hand));
    }

    [Fact]
    public void CanOfferInsurance_InsufficientBankroll_ReturnsFalse()
    {
        var dealer = new Dealer();
        dealer.AddCard(CardHelper.Ace());
        dealer.AddCard(CardHelper.King());

        var hand = CardHelper.CreateHand(Rank.Ten, Rank.Seven);
        hand.Bet = 100m; // Insurance would cost $50

        var player = new Player("Test", 1, 25m); // Only $25 left
        player.Hands[0] = hand;

        Assert.False(GameRules.CanOfferInsurance(dealer, player, hand));
    }

    #endregion

    #region Even Money Validation Tests

    [Fact]
    public void CanOfferEvenMoney_PlayerBlackjackDealerAce_ReturnsTrue()
    {
        var playerHand = CardHelper.CreateHand(Rank.Ace, Rank.King);

        var dealer = new Dealer();
        dealer.AddCard(CardHelper.Ace());
        dealer.AddCard(CardHelper.Seven());

        Assert.True(GameRules.CanOfferEvenMoney(playerHand, dealer));
    }

    [Fact]
    public void CanOfferEvenMoney_PlayerNotBlackjack_ReturnsFalse()
    {
        var playerHand = CardHelper.CreateHand(Rank.Ten, Rank.King); // 20, not blackjack

        var dealer = new Dealer();
        dealer.AddCard(CardHelper.Ace());
        dealer.AddCard(CardHelper.Seven());

        Assert.False(GameRules.CanOfferEvenMoney(playerHand, dealer));
    }

    [Fact]
    public void CanOfferEvenMoney_DealerNotShowingAce_ReturnsFalse()
    {
        var playerHand = CardHelper.CreateHand(Rank.Ace, Rank.King);

        var dealer = new Dealer();
        dealer.AddCard(CardHelper.Ten()); // Not an Ace
        dealer.AddCard(CardHelper.Seven());

        Assert.False(GameRules.CanOfferEvenMoney(playerHand, dealer));
    }

    #endregion
}
