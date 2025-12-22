namespace Blackjack.Models
{
    /// <summary>
    /// Represents a multi-deck shoe used in casino Blackjack.
    /// </summary>
    public class Deck
    {
        private readonly List<Card> _cards;
        private int _cardsDealt;
        private readonly int _numberOfDecks;
        private const int CARDS_PER_DECK = 52;
        private readonly int _totalCards;
        private readonly double SHUFFLE_PENETRATION = 0.75;

        /// <summary>
        /// Event fired when the deck state changes (card dealt, shuffled, or reset).
        /// </summary>
        public event Action? DeckChanged;

        public int CardsRemaining => _cards.Count;
        public int CardsDealt => _cardsDealt;
        public int NumberOfDecks => _numberOfDecks;
        public int TotalCards => _totalCards;
        public bool NeedsReshuffle => _cardsDealt >= (int)(_totalCards * SHUFFLE_PENETRATION);
        
        /// <summary>
        /// Number of cards that can be dealt before a reshuffle is needed.
        /// </summary>
        public int CardsUntilReshuffle => Math.Max(0, (int)(_totalCards * SHUFFLE_PENETRATION) - _cardsDealt);

        /// <summary>
        /// Creates a new deck shoe with the specified number of decks.
        /// </summary>
        /// <param name="numberOfDecks">Number of decks in the shoe (default: 6)</param>
        public Deck(int numberOfDecks = 6)
        {
            _numberOfDecks = numberOfDecks;
            _totalCards = _numberOfDecks * CARDS_PER_DECK;
            _cards = [];
            _cardsDealt = 0;
            InitializeShoe();
            Shuffle();
        }

        /// <summary>
        /// Initializes the shoe with all cards from the configured number of decks.
        /// </summary>
        private void InitializeShoe()
        {
            _cards.Clear();

            // Create decks based on configuration
            for (int deck = 0; deck < _numberOfDecks; deck++)
            {
                // For each deck, create all 52 cards
                foreach (Suit suit in Enum.GetValues<Suit>())
                {
                    foreach (Rank rank in Enum.GetValues<Rank>())
                    {
                        _cards.Add(new Card(suit, rank));
                    }
                }
            }
        }

        /// <summary>
        /// Shuffles the deck using the Fisher-Yates shuffle algorithm.
        /// Uses Random.Shared for thread-safe, properly-seeded randomization.
        /// </summary>
        public void Shuffle()
        {
            int n = _cards.Count;

            // Fisher-Yates shuffle using Random.Shared (thread-safe singleton in .NET 6+)
            for (int i = n - 1; i > 0; i--)
            {
                int j = Random.Shared.Next(i + 1);
                (_cards[j], _cards[i]) = (_cards[i], _cards[j]);
            }

            _cardsDealt = 0;
            DeckChanged?.Invoke();
        }

        /// <summary>
        /// Deals a single card from the shoe.
        /// </summary>
        /// <returns>The dealt card, or null if no cards remain.</returns>
        public Card? DealCard()
        {
            if (_cards.Count == 0)
            {
                return null;
            }

            Card card = _cards[0];
            _cards.RemoveAt(0);
            _cardsDealt++;
            DeckChanged?.Invoke();
            return card;
        }

        /// <summary>
        /// Resets the shoe by reinitializing and shuffling all cards.
        /// </summary>
        public void Reset()
        {
            InitializeShoe();
            Shuffle();
        }
    }
}
