using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MyCryptos.Core.settings
{
    public static class Settings
    {
        public const string KeyBaseCurrency = "currency";

        public const string KeyFirstLaunch = "first-launch";

        public const string KeySortOrder = "sort-order";

        public const string KeySortDirection = "sort-direction";

        public const string KeyAutoRefreshOnStartup = "auto-refresh-on-startup";

        public const string KeyReferenceCurrencies = "reference-currencies";

        public const string KeyPin = "pin";

        public const string KeyPinLength = "pin-length";

        public const string KeyFingerprintSet = "fingerprint-set";

        public const string DefaultPage = "default-page";

        public static T Get<T>(string key, T defaultValue)
        {
            return AppSettings.GetValueOrDefault(key, defaultValue);
        }

        public static void Set<T>(string key, T value)
        {
            AppSettings.AddOrUpdateValue(key, value);
        }

        private static ISettings AppSettings => CrossSettings.Current;
    }
}

