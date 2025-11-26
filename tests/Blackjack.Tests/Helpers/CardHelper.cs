using Blackjack.Models;

namespace Blackjack.Tests.Helpers;

/// <summary>
/// Helper class for creating cards easily in unit tests.
/// </summary>
public static class CardHelper
{
    /// <summary>
    /// Creates a card with the specified rank and suit.
    /// </summary>
    public static Card Create(Rank rank, Suit suit = Suit.Hearts)
    {
        return new Card(suit, rank);
    }

    /// <summary>
    /// Creates an Ace card.
    /// </summary>
    public static Card Ace(Suit suit = Suit.Hearts) => Create(Rank.Ace, suit);

    /// <summary>
    /// Creates a King card.
    /// </summary>
    public static Card King(Suit suit = Suit.Hearts) => Create(Rank.King, suit);

    /// <summary>
    /// Creates a Queen card.
    /// </summary>
    public static Card Queen(Suit suit = Suit.Hearts) => Create(Rank.Queen, suit);

    /// <summary>
    /// Creates a Jack card.
    /// </summary>
    public static Card Jack(Suit suit = Suit.Hearts) => Create(Rank.Jack, suit);

    /// <summary>
    /// Creates a Ten card.
    /// </summary>
    public static Card Ten(Suit suit = Suit.Hearts) => Create(Rank.Ten, suit);

    /// <summary>
    /// Creates a Nine card.
    /// </summary>
    public static Card Nine(Suit suit = Suit.Hearts) => Create(Rank.Nine, suit);

    /// <summary>
    /// Creates an Eight card.
    /// </summary>
    public static Card Eight(Suit suit = Suit.Hearts) => Create(Rank.Eight, suit);

    /// <summary>
    /// Creates a Seven card.
    /// </summary>
    public static Card Seven(Suit suit = Suit.Hearts) => Create(Rank.Seven, suit);

    /// <summary>
    /// Creates a Six card.
    /// </summary>
    public static Card Six(Suit suit = Suit.Hearts) => Create(Rank.Six, suit);

    /// <summary>
    /// Creates a Five card.
    /// </summary>
    public static Card Five(Suit suit = Suit.Hearts) => Create(Rank.Five, suit);

    /// <summary>
    /// Creates a Four card.
    /// </summary>
    public static Card Four(Suit suit = Suit.Hearts) => Create(Rank.Four, suit);

    /// <summary>
    /// Creates a Three card.
    /// </summary>
    public static Card Three(Suit suit = Suit.Hearts) => Create(Rank.Three, suit);

    /// <summary>
    /// Creates a Two card.
    /// </summary>
    public static Card Two(Suit suit = Suit.Hearts) => Create(Rank.Two, suit);

    /// <summary>
    /// Creates a hand with the specified cards.
    /// </summary>
    public static Hand CreateHand(params Card[] cards)
    {
        var hand = new Hand();
        foreach (var card in cards)
        {
            hand.AddCard(card);
        }
        return hand;
    }

    /// <summary>
    /// Creates a hand with the specified ranks (all Hearts by default).
    /// </summary>
    public static Hand CreateHand(params Rank[] ranks)
    {
        var hand = new Hand();
        foreach (var rank in ranks)
        {
            hand.AddCard(Create(rank));
        }
        return hand;
    }
}
