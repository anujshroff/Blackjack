namespace Blackjack.Models
{
    /// <summary>
    /// Represents a 6-deck shoe used in casino Blackjack.
    /// </summary>
    public class Deck
    {
        private List<Card> _cards;
        private int _cardsDealt;
        private const int NUMBER_OF_DECKS = 6;
        private const int CARDS_PER_DECK = 52;
        private const int TOTAL_CARDS = NUMBER_OF_DECKS * CARDS_PER_DECK; // 312 cards
        private const double SHUFFLE_PENETRATION = 0.75; // Shuffle after 75% dealt (~234 cards)

        public int CardsRemaining => _cards.Count;
        public int CardsDealt => _cardsDealt;
        public bool NeedsReshuffle => _cardsDealt >= (int)(TOTAL_CARDS * SHUFFLE_PENETRATION);

        public Deck()
        {
            _cards = new List<Card>();
            _cardsDealt = 0;
            InitializeShoe();
            Shuffle();
        }

        /// <summary>
        /// Initializes the 6-deck shoe with all 312 cards.
        /// </summary>
        private void InitializeShoe()
        {
            _cards.Clear();

            // Create 6 decks
            for (int deck = 0; deck < NUMBER_OF_DECKS; deck++)
            {
                // For each deck, create all 52 cards
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                    {
                        _cards.Add(new Card(suit, rank));
                    }
                }
            }
        }

        /// <summary>
        /// Shuffles the deck using the Fisher-Yates shuffle algorithm.
        /// </summary>
        public void Shuffle()
        {
            Random random = new Random();
            int n = _cards.Count;

            // Fisher-Yates shuffle
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                Card temp = _cards[i];
                _cards[i] = _cards[j];
                _cards[j] = temp;
            }

            _cardsDealt = 0;
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
