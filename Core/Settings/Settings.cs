using System.Collections.Generic;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MyCC.Core.Settings
{
    public static class Settings
    {
        private static readonly Dictionary<string, object> Cache = new Dictionary<string, object>();

        public const string KeyAppVersion = "version";

        public const string KeyBaseCurrency = "currency";

        public const string KeyRatePageCurrency = "rate-page-currency";

        public const string KeyFirstLaunch = "first-launch";
        public const string KeyAppInitialised = "app-initialised";


        public const string KeySortOrderTable = "sort-order-table";
        public const string KeySortOrderRates = "sort-order-rates";
        public const string KeySortOrderAccounts = "sort-order-accounts";
        public const string KeySortOrderReferenceValues = "sort-order-reference";

        public const string KeySortDirectionTable = "sort-direction-table";
        public const string KeySortDirectionRates = "sort-direction-rates";
        public const string KeySortDirectionAccounts = "sort-direction-accounts";
        public const string KeySortDirectionReferenceValues = "sort-direction-reference";

        public const string KeyAutoRefreshOnStartup = "auto-refresh-on-startup";

        public const string KeyWatchedCurrencies = "watched-currencies";
        public const string KeyDisabledCurrencies = "disabled-currencies";
        public const string KeyMainCurrencies = "main-currencies";
        public const string KeyFurtherCurrencies = "further-currencies";

        public const string KeyPin = "pin";

        public const string KeyPinLength = "pin-length";

        public const string KeyFingerprintSet = "fingerprint-set";

        public const string KeyLockByShaking = "lock_by_shaking";

        public const string DefaultPage = "default-page";
        public const string KeyAssetsColumnHideWhenSmall = "pref_assets_column_to_hide_if_small";

        public const string RoundMoney = "round-money";

        public const string PreferredBitcoinRepository = "preferred-bitcoin-repository";


        public static T Get<T>(string key, T defaultValue)
        {
            if (Cache.ContainsKey(key))
            {
                return (T)Cache[key];
            }

            T value;
            try
            {
                value = AppSettings.GetValueOrDefault(key, defaultValue);
            }
            catch
            {
                value = defaultValue;
            }

            if (Cache.ContainsKey(key)) return value;

            try
            {
                Cache.Add(key, value);
            }
            catch
            {
                /* Was added by another thread */
            }
            return value;
        }

        public static void ClearCache()
        {
            Cache.Clear();
        }

        public static void Set<T>(string key, T value)
        {
            if (Cache.ContainsKey(key) && Equals(value, Cache[key])) return;

            AppSettings.AddOrUpdateValue(key, value);

            if (!Cache.ContainsKey(key))
            {
                Cache.Add(key, value);
            }
            else
            {
                Cache[key] = value;
            }
        }

        private static ISettings AppSettings => CrossSettings.Current;
    }
}

