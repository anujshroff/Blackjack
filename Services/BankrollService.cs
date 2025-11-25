namespace Blackjack.Services
{
    /// <summary>
    /// Service for persisting and loading player bankroll using MAUI Preferences API.
    /// Provides cross-platform storage that works on Windows, Android, and iOS.
    /// </summary>
    public class BankrollService
    {
        private const string BankrollKey = "PlayerBankroll";

        /// <summary>
        /// Loads the saved bankroll from persistent storage.
        /// </summary>
        /// <param name="defaultBankroll">The default bankroll to return if no saved value exists.</param>
        /// <returns>The saved bankroll or the default value if none exists.</returns>
        public static decimal LoadBankroll(decimal defaultBankroll)
        {
            string savedValue = Preferences.Get(BankrollKey, string.Empty);

            if (string.IsNullOrEmpty(savedValue))
            {
                return defaultBankroll;
            }

            if (decimal.TryParse(savedValue, out decimal bankroll))
            {
                return bankroll;
            }

            return defaultBankroll;
        }

        /// <summary>
        /// Saves the player's bankroll to persistent storage.
        /// </summary>
        /// <param name="bankroll">The bankroll amount to save.</param>
        public static void SaveBankroll(decimal bankroll)
        {
            Preferences.Set(BankrollKey, bankroll.ToString());
        }

        /// <summary>
        /// Resets the saved bankroll by removing it from persistent storage.
        /// The next load will return the default starting bankroll.
        /// </summary>
        public static void ResetBankroll()
        {
            Preferences.Remove(BankrollKey);
        }

        /// <summary>
        /// Checks if a saved bankroll exists in persistent storage.
        /// </summary>
        /// <returns>True if a saved bankroll exists, false otherwise.</returns>
        public static bool HasSavedBankroll()
        {
            string savedValue = Preferences.Get(BankrollKey, string.Empty);
            return !string.IsNullOrEmpty(savedValue);
        }

        /// <summary>
        /// Gets the saved bankroll without a default fallback.
        /// Returns null if no bankroll is saved.
        /// </summary>
        /// <returns>The saved bankroll or null if none exists.</returns>
        public static decimal? GetSavedBankroll()
        {
            string savedValue = Preferences.Get(BankrollKey, string.Empty);

            if (string.IsNullOrEmpty(savedValue))
            {
                return null;
            }

            if (decimal.TryParse(savedValue, out decimal bankroll))
            {
                return bankroll;
            }

            return null;
        }
    }
}
