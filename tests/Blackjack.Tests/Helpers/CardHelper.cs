using Blackjack.Models;

namespace Blackjack.Tests.Helpers;

/// <summary>
/// Helper class for creating cards easily in unit tests.
/// </summary>
public static class CardHelper
{
    /// <summary>
    /// Creates a card with the specified rank, suit, and explicit rank name.
    /// </summary>
    public static Card Create(Rank rank, Suit suit, string rankName)
    {
        return new Card(suit, rank, rankName);
    }

    /// <summary>
    /// Creates a card with the specified rank and suit.
    /// Note: For face cards (value 10), use the specific helper methods to ensure correct rank name.
    /// Due to duplicate enum values, this method uses a lookup table for face cards.
    /// </summary>
    public static Card Create(Rank rank, Suit suit = Suit.Hearts)
    {
        // Use a lookup table because switch/case on enum with duplicate values doesn't work
        var rankNames = new Dictionary<Rank, string>
        {
            { Rank.Two, "Two" },
            { Rank.Three, "Three" },
            { Rank.Four, "Four" },
            { Rank.Five, "Five" },
            { Rank.Six, "Six" },
            { Rank.Seven, "Seven" },
            { Rank.Eight, "Eight" },
            { Rank.Nine, "Nine" },
            { Rank.Ace, "Ace" }
        };

        // For face cards with value 10, we can't distinguish - default to "Ten"
        // Use specific methods (King(), Queen(), Jack(), Ten()) for accurate names
        string rankName = rankNames.TryGetValue(rank, out var name) ? name : "Ten";
        return new Card(suit, rank, rankName);
    }

    /// <summary>
    /// Creates an Ace card.
    /// </summary>
    public static Card Ace(Suit suit = Suit.Hearts) => Create(Rank.Ace, suit, "Ace");

    /// <summary>
    /// Creates a King card.
    /// </summary>
    public static Card King(Suit suit = Suit.Hearts) => Create(Rank.King, suit, "King");

    /// <summary>
    /// Creates a Queen card.
    /// </summary>
    public static Card Queen(Suit suit = Suit.Hearts) => Create(Rank.Queen, suit, "Queen");

    /// <summary>
    /// Creates a Jack card.
    /// </summary>
    public static Card Jack(Suit suit = Suit.Hearts) => Create(Rank.Jack, suit, "Jack");

    /// <summary>
    /// Creates a Ten card.
    /// </summary>
    public static Card Ten(Suit suit = Suit.Hearts) => Create(Rank.Ten, suit, "Ten");

    /// <summary>
    /// Creates a Nine card.
    /// </summary>
    public static Card Nine(Suit suit = Suit.Hearts) => Create(Rank.Nine, suit, "Nine");

    /// <summary>
    /// Creates an Eight card.
    /// </summary>
    public static Card Eight(Suit suit = Suit.Hearts) => Create(Rank.Eight, suit, "Eight");

    /// <summary>
    /// Creates a Seven card.
    /// </summary>
    public static Card Seven(Suit suit = Suit.Hearts) => Create(Rank.Seven, suit, "Seven");

    /// <summary>
    /// Creates a Six card.
    /// </summary>
    public static Card Six(Suit suit = Suit.Hearts) => Create(Rank.Six, suit, "Six");

    /// <summary>
    /// Creates a Five card.
    /// </summary>
    public static Card Five(Suit suit = Suit.Hearts) => Create(Rank.Five, suit, "Five");

    /// <summary>
    /// Creates a Four card.
    /// </summary>
    public static Card Four(Suit suit = Suit.Hearts) => Create(Rank.Four, suit, "Four");

    /// <summary>
    /// Creates a Three card.
    /// </summary>
    public static Card Three(Suit suit = Suit.Hearts) => Create(Rank.Three, suit, "Three");

    /// <summary>
    /// Creates a Two card.
    /// </summary>
    public static Card Two(Suit suit = Suit.Hearts) => Create(Rank.Two, suit, "Two");

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
