namespace Blackjack.Services
{
    /// <summary>
    /// Service for persisting and loading game settings using MAUI Preferences API.
    /// Provides cross-platform storage that works on Windows, Android, and iOS.
    /// </summary>
    public class SettingsService
    {
        private const string TableMinimumKey = "TableMinimum";
        private const string TableMaximumKey = "TableMaximum";
        private const string StartingBankrollKey = "StartingBankroll";
        private const string NumberOfDecksKey = "NumberOfDecks";

        // Default values
        public const decimal DefaultTableMinimum = 5m;
        public const decimal DefaultTableMaximum = 500m;
        public const decimal DefaultStartingBankroll = 1000m;
        public const int DefaultNumberOfDecks = 6;

        /// <summary>
        /// Loads the table minimum from persistent storage.
        /// </summary>
        public static decimal LoadTableMinimum()
        {
            string savedValue = Preferences.Get(TableMinimumKey, string.Empty);
            if (decimal.TryParse(savedValue, out decimal value))
            {
                return value;
            }
            return DefaultTableMinimum;
        }

        /// <summary>
        /// Saves the table minimum to persistent storage.
        /// </summary>
        public static void SaveTableMinimum(decimal value)
        {
            Preferences.Set(TableMinimumKey, value.ToString());
        }

        /// <summary>
        /// Loads the table maximum from persistent storage.
        /// </summary>
        public static decimal LoadTableMaximum()
        {
            string savedValue = Preferences.Get(TableMaximumKey, string.Empty);
            if (decimal.TryParse(savedValue, out decimal value))
            {
                return value;
            }
            return DefaultTableMaximum;
        }

        /// <summary>
        /// Saves the table maximum to persistent storage.
        /// </summary>
        public static void SaveTableMaximum(decimal value)
        {
            Preferences.Set(TableMaximumKey, value.ToString());
        }

        /// <summary>
        /// Loads the starting bankroll from persistent storage.
        /// </summary>
        public static decimal LoadStartingBankroll()
        {
            string savedValue = Preferences.Get(StartingBankrollKey, string.Empty);
            if (decimal.TryParse(savedValue, out decimal value))
            {
                return value;
            }
            return DefaultStartingBankroll;
        }

        /// <summary>
        /// Saves the starting bankroll to persistent storage.
        /// </summary>
        public static void SaveStartingBankroll(decimal value)
        {
            Preferences.Set(StartingBankrollKey, value.ToString());
        }

        /// <summary>
        /// Loads the number of decks from persistent storage.
        /// </summary>
        public static int LoadNumberOfDecks()
        {
            return Preferences.Get(NumberOfDecksKey, DefaultNumberOfDecks);
        }

        /// <summary>
        /// Saves the number of decks to persistent storage.
        /// </summary>
        public static void SaveNumberOfDecks(int value)
        {
            Preferences.Set(NumberOfDecksKey, value);
        }

        /// <summary>
        /// Saves all settings at once.
        /// </summary>
        public static void SaveAllSettings(decimal tableMinimum, decimal tableMaximum, decimal startingBankroll, int numberOfDecks)
        {
            SaveTableMinimum(tableMinimum);
            SaveTableMaximum(tableMaximum);
            SaveStartingBankroll(startingBankroll);
            SaveNumberOfDecks(numberOfDecks);
        }

        /// <summary>
        /// Resets all settings to their default values.
        /// </summary>
        public static void ResetToDefaults()
        {
            Preferences.Remove(TableMinimumKey);
            Preferences.Remove(TableMaximumKey);
            Preferences.Remove(StartingBankrollKey);
            Preferences.Remove(NumberOfDecksKey);
        }
    }
}
