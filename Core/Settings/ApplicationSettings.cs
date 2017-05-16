using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.Repositories;
using MyCC.Core.Types;
using Newtonsoft.Json.Linq;
using PCLCrypto;

namespace MyCC.Core.Settings
{
    public static class ApplicationSettings
    {
        private static Version _lastVersion;

        public static Version LastCoreVersion
        {
            get
            {
                if (_lastVersion != null) return _lastVersion;

                if (FirstLaunch) return Constants.CoreVersion;

                var persitedValue = new Version(Settings.Get(Settings.KeyAppVersion, "0.0.0"));
                Settings.Set(Settings.KeyAppVersion, Constants.CoreVersion.ToString());

                _lastVersion = persitedValue;
                return _lastVersion;
            }
        }

        private static bool? _firstLaunch;

        public static bool FirstLaunch
        {
            get
            {
                if (_firstLaunch.HasValue) return _firstLaunch.Value;

                var persitedValue = Settings.Get(Settings.KeyFirstLaunch, true);
                if (persitedValue)
                {
                    Settings.Set(Settings.KeyFirstLaunch, false);
                }
                _firstLaunch = persitedValue;
                return _firstLaunch.Value;
            }
        }

        public static bool DataLoaded;

        public static string StartupCurrencyAssets
        {
            get { return Settings.Get(Settings.KeyBaseCurrency, CurrencyConstants.Btc.Id); }
            set { Settings.Set(Settings.KeyBaseCurrency, value); }
        }

        public static string StartupCurrencyRates
        {
            get { return Settings.Get(Settings.KeyRatePageCurrency, CurrencyConstants.Btc.Id); }
            set { Settings.Set(Settings.KeyRatePageCurrency, value); }
        }

        public static IEnumerable<string> WatchedCurrencies
        {
            get
            {
                return
                    Settings.Get(Settings.KeyWatchedCurrencies, string.Empty)
                        .Split(',')
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Distinct();
            }
            set
            {
                Settings.Set(Settings.KeyWatchedCurrencies,
                    string.Join(",", value.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct()));
            }
        }

        public static IEnumerable<string> DisabledCurrencyIds
        {
            get
            {
                return
                    Settings.Get(Settings.KeyDisabledCurrencies, string.Empty)
                        .Split(',')
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Distinct();
            }
            set
            {
                Settings.Set(Settings.KeyDisabledCurrencies,
                    string.Join(",", value.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct()));
            }
        }

        public static IEnumerable<string> TryToLoadOldCurrencies(string key)
        {
            try
            {
                var json = Settings.Get(key, string.Empty);
                if (string.IsNullOrEmpty(json)) return null;

                var currencies = JObject.Parse(json);
                var ids = new List<string>();
                foreach (var c in currencies)
                {
                    var id = (string)c.Value["Id"];
                    ids.Add(id);
                }
                return ids;
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }


        public static string Pin
        {
            set
            {
                PinLength = string.IsNullOrEmpty(value) ? -1 : value.Length;
                Settings.Set(Settings.KeyPin, string.IsNullOrWhiteSpace(value) ? string.Empty : Hash(value));
            }
        }

        public static bool IsPinValid(string pin) => Settings.Get(Settings.KeyPin, string.Empty).Equals(Hash(pin));

        public static IEnumerable<string> MainCurrencies
        {
            get
            {
                var defaultValue = string.Join(",", new List<Currency> { CurrencyConstants.Btc, CurrencyConstants.Eur, CurrencyConstants.Usd }.Select(c => c.Id));
                return Settings.Get(Settings.KeyMainCurrencies, defaultValue).Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();
            }
            set { Settings.Set(Settings.KeyMainCurrencies, string.Join(",", value.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct())); }
        }

        public static IEnumerable<string> FurtherCurrencies
        {
            get { return Settings.Get(Settings.KeyFurtherCurrencies, string.Empty).Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(); }
            set { Settings.Set(Settings.KeyFurtherCurrencies, string.Join(",", value.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct())); }
        }

        public static IEnumerable<string> AllReferenceCurrencies => MainCurrencies.Concat(FurtherCurrencies).Distinct().ToList();

        public static SortOrder SortOrderAssets
        {
            get { return (SortOrder)Enum.Parse(typeof(SortOrder), Settings.Get(Settings.KeySortOrderTable, SortOrder.Alphabetical.ToString())); }
            set { Settings.Set(Settings.KeySortOrderTable, value.ToString()); }
        }

        public static SortDirection SortDirectionAssets
        {
            get { return (SortDirection)Enum.Parse(typeof(SortOrder), Settings.Get(Settings.KeySortDirectionTable, SortDirection.Ascending.ToString())); }
            set { Settings.Set(Settings.KeySortDirectionTable, value.ToString()); }
        }

        public static SortOrder SortOrderRates
        {
            get
            {
                const SortOrder defaultValue = SortOrder.Alphabetical;
                var stringValue = Settings.Get(Settings.KeySortOrderRates, defaultValue.ToString());
                var enumValue = (SortOrder)Enum.Parse(typeof(SortOrder), stringValue);
                return enumValue;
            }
            set
            {
                Settings.Set(Settings.KeySortOrderRates, value.ToString());
            }
        }

        public static SortDirection SortDirectionRates
        {
            get
            {
                var defaultValue = SortDirection.Ascending.ToString();
                var stringValue = Settings.Get(Settings.KeySortDirectionRates, defaultValue);
                return (SortDirection)Enum.Parse(typeof(SortDirection), stringValue);
            }
            set
            {
                Settings.Set(Settings.KeySortDirectionRates, value.ToString());
            }
        }

        public static SortOrder SortOrderAccounts
        {
            get
            {
                const SortOrder defaultValue = SortOrder.Alphabetical;
                var stringValue = Settings.Get(Settings.KeySortOrderAccounts, defaultValue.ToString());
                var enumValue = (SortOrder)Enum.Parse(typeof(SortOrder), stringValue);
                return enumValue;
            }
            set
            {
                Settings.Set(Settings.KeySortOrderAccounts, value.ToString());
            }
        }

        public static SortDirection SortDirectionAccounts
        {
            get
            {
                var defaultValue = SortDirection.Ascending.ToString();
                var stringValue = Settings.Get(Settings.KeySortDirectionAccounts, defaultValue);
                return (SortDirection)Enum.Parse(typeof(SortDirection), stringValue);
            }
            set
            {
                Settings.Set(Settings.KeySortDirectionAccounts, value.ToString());
            }
        }

        public static SortOrder SortOrderReferenceValues
        {
            get
            {
                const SortOrder defaultValue = SortOrder.ByUnits;
                var stringValue = Settings.Get(Settings.KeySortOrderReferenceValues, defaultValue.ToString());
                var enumValue = (SortOrder)Enum.Parse(typeof(SortOrder), stringValue);
                return enumValue;
            }
            set
            {
                Settings.Set(Settings.KeySortOrderReferenceValues, value.ToString());
            }
        }

        public static SortDirection SortDirectionReferenceValues
        {
            get
            {
                var defaultValue = SortDirection.Ascending.ToString();
                var stringValue = Settings.Get(Settings.KeySortDirectionReferenceValues, defaultValue);
                return (SortDirection)Enum.Parse(typeof(SortDirection), stringValue);
            }
            set
            {
                Settings.Set(Settings.KeySortDirectionReferenceValues, value.ToString());
            }
        }

        public static bool AutoRefreshOnStartup
        {
            get { return Settings.Get(Settings.KeyAutoRefreshOnStartup, true); }
            set { Settings.Set(Settings.KeyAutoRefreshOnStartup, value); }
        }

        public static bool AppInitialised
        {
            get { return Settings.Get(Settings.KeyAppInitialised, false); }
            set { Settings.Set(Settings.KeyAppInitialised, value); }
        }

        public static bool RoundMoney
        {
            get { return false;/* return Settings.Get(Settings.RoundMoney, false);*/}
            set { Settings.Set(Settings.RoundMoney, value); }
        }

        public static int PreferredBitcoinRepository
        {
            get { return Settings.Get(Settings.PreferredBitcoinRepository, (int)RatesRepositories.Kraken); }
            set { Settings.Set(Settings.PreferredBitcoinRepository, value); }
        }

        public static StartupPage DefaultStartupPage
        {
            get { return (StartupPage)Enum.Parse(typeof(StartupPage), Settings.Get(Settings.DefaultPage, StartupPage.RatesView.ToString())); }
            set { Settings.Set(Settings.DefaultPage, value.ToString()); }
        }

        public static int PinLength
        {
            get { return Settings.Get(Settings.KeyPinLength, -1); }
            private set { Settings.Set(Settings.KeyPinLength, value); }
        }

        public static bool IsPinSet => PinLength != -1;

        public static bool IsFingerprintEnabled
        {
            get { return Settings.Get(Settings.KeyFingerprintSet, false); }
            set { Settings.Set(Settings.KeyFingerprintSet, value); }
        }
        public static bool LockByShaking
        {
            get { return Settings.Get(Settings.KeyLockByShaking, false); }
            set { Settings.Set(Settings.KeyLockByShaking, value); }
        }

        private static string ByteToString(IEnumerable<byte> buff)
        {
            var sBuilder = new StringBuilder();
            foreach (var t in buff)
            {
                sBuilder.Append(t.ToString("X2"));
            }
            return sBuilder.ToString().ToLower();
        }

        private static string Hash(string text)
        {
            var keyBytes = Encoding.UTF8.GetBytes(text);

            var algorithm = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha512);
            var hasher = algorithm.CreateHash(keyBytes);
            return ByteToString(hasher.GetValueAndReset());
        }
    }
}