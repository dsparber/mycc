using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCC.Core.Rates.Repositories;
using MyCC.Core.Types;
using Newtonsoft.Json;
using PCLCrypto;

namespace MyCC.Core.Settings
{
    public static class ApplicationSettings
    {
        public static void Migrate()
        {
            if (LastCoreVersion < new Version(1, 0, 2))
            {
                DefaultStartupPage = StartupPage.TableView;
            }
        }

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

        public static Currency.Model.Currency StartupCurrencyAssets
        {
            get
            {
                var json = Settings.Get(Settings.KeyBaseCurrency, JsonConvert.SerializeObject(Currency.Model.Currency.Btc));
                Currency.Model.Currency currency;

                try
                {
                    currency = JsonConvert.DeserializeObject<Currency.Model.Currency>(json);
                }
                catch
                {
                    currency = Currency.Model.Currency.Btc;
                    StartupCurrencyAssets = currency;
                }
                return currency;
            }
            set
            {
                Settings.Set(Settings.KeyBaseCurrency, JsonConvert.SerializeObject(value));
            }
        }

        public static Currency.Model.Currency StartupCurrencyRates
        {
            get
            {
                var json = Settings.Get(Settings.KeyRatePageCurrency, JsonConvert.SerializeObject(Currency.Model.Currency.Btc));

                Currency.Model.Currency currency;

                try
                {
                    currency = JsonConvert.DeserializeObject<Currency.Model.Currency>(json);
                }
                catch
                {
                    currency = Currency.Model.Currency.Btc;
                    StartupCurrencyRates = currency;
                }
                return currency;
            }
            set
            {
                Settings.Set(Settings.KeyRatePageCurrency, JsonConvert.SerializeObject(value));
            }
        }

        public static List<Currency.Model.Currency> WatchedCurrencies
        {
            get
            {
                var currencies = new List<Currency.Model.Currency>();

                var defaultValue = JsonConvert.SerializeObject(currencies);

                var json = Settings.Get(Settings.KeyWatchedCurrencies, defaultValue);

                List<Currency.Model.Currency> data;

                try
                {
                    data = JsonConvert.DeserializeObject<List<Currency.Model.Currency>>(json);
                }
                catch
                {
                    data = currencies;
                    WatchedCurrencies = data;
                }

                data.RemoveAll(c => c.IsCryptoCurrency && CurrencyBlacklist.Contains(c.Code));
                return data.Contains(null) ? currencies : data.OrderBy(c => c.Code).ToList();
            }
            set
            {
                Settings.Set(Settings.KeyWatchedCurrencies, JsonConvert.SerializeObject(value));
            }
        }

        public static IEnumerable<string> DisabledCurrencyIds
        {
            get
            {
                return Settings.Get(Settings.KeyDisabledCurrencies, string.Empty).Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();
            }
            set
            {
                Settings.Set(Settings.KeyDisabledCurrencies, string.Join(",", value.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct()));
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

        public static bool IsPinValid(string pin)
        {
            var hash = Settings.Get(Settings.KeyPin, string.Empty);
            return hash.Equals(Hash(pin));
        }

        public static List<Currency.Model.Currency> MainCurrencies
        {
            get
            {
                var currencies = new List<Currency.Model.Currency> { Currency.Model.Currency.Btc, Currency.Model.Currency.Eur, Currency.Model.Currency.Usd };
                var defaultValue = JsonConvert.SerializeObject(currencies);

                var json = Settings.Get(Settings.KeyMainCurrencies, defaultValue);
                List<Currency.Model.Currency> data;

                try
                {
                    data = JsonConvert.DeserializeObject<List<Currency.Model.Currency>>(json);
                }
                catch
                {
                    data = currencies;
                    MainCurrencies = data;
                }

                if (!data.Contains(Currency.Model.Currency.Btc)) data.Add(Currency.Model.Currency.Btc);
                data.RemoveAll(c => c.IsCryptoCurrency && CurrencyBlacklist.Contains(c.Code));

                return data.Count > 0 ? data.OrderBy(c => c.Code).ToList() : currencies;
            }
            set
            {
                Settings.Set(Settings.KeyMainCurrencies, JsonConvert.SerializeObject(value));
            }
        }

        public static List<Currency.Model.Currency> FurtherCurrencies
        {
            get
            {
                var currencies = new List<Currency.Model.Currency>();

                var defaultValue = JsonConvert.SerializeObject(currencies);

                var json = Settings.Get(Settings.KeyFurtherCurrencies, defaultValue);

                List<Currency.Model.Currency> data;

                try
                {
                    data = JsonConvert.DeserializeObject<List<Currency.Model.Currency>>(json);
                }
                catch
                {
                    data = currencies;
                    FurtherCurrencies = data;
                }

                data.RemoveAll(MainCurrencies.Contains);
                data.RemoveAll(c => c.IsCryptoCurrency && CurrencyBlacklist.Contains(c.Code));
                return data.OrderBy(c => c.Code).ToList();
            }
            set
            {
                value.RemoveAll(MainCurrencies.Contains);
                Settings.Set(Settings.KeyFurtherCurrencies, JsonConvert.SerializeObject(value));
            }
        }

        public static List<Currency.Model.Currency> AllReferenceCurrencies => MainCurrencies.Concat(FurtherCurrencies).Distinct().ToList();

        public static SortOrder SortOrderTable
        {
            get
            {
                const SortOrder defaultValue = SortOrder.Alphabetical;
                var stringValue = Settings.Get(Settings.KeySortOrderTable, defaultValue.ToString());
                var enumValue = (SortOrder)Enum.Parse(typeof(SortOrder), stringValue);
                return enumValue;
            }
            set
            {
                Settings.Set(Settings.KeySortOrderTable, value.ToString());
            }
        }

        public static SortDirection SortDirectionTable
        {
            get
            {
                var defaultValue = SortDirection.Ascending.ToString();
                var stringValue = Settings.Get(Settings.KeySortDirectionTable, defaultValue);
                return (SortDirection)Enum.Parse(typeof(SortDirection), stringValue);
            }
            set
            {
                Settings.Set(Settings.KeySortDirectionTable, value.ToString());
            }
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
            get
            {
                return Settings.Get(Settings.KeyAutoRefreshOnStartup, true);
            }
            set
            {
                Settings.Set(Settings.KeyAutoRefreshOnStartup, value);
            }
        }

        public static bool AppInitialised
        {
            get
            {
                return Settings.Get(Settings.KeyAppInitialised, false);
            }
            set
            {
                Settings.Set(Settings.KeyAppInitialised, value);
            }
        }

        public static bool RoundMoney
        {
            get
            {
                return false;
                // return Settings.Get(Settings.RoundMoney, false);
            }
            set
            {
                Settings.Set(Settings.RoundMoney, value);
            }
        }

        public static int PreferredBitcoinRepository
        {
            get
            {
                return Settings.Get(Settings.PreferredBitcoinRepository, (int)RatesRepositories.Kraken);
            }
            set
            {
                Settings.Set(Settings.PreferredBitcoinRepository, value);
            }
        }

        public static StartupPage DefaultStartupPage
        {
            get
            {
                var success = Enum.TryParse(Settings.Get(Settings.DefaultPage, StartupPage.TableView.ToString()), out StartupPage page);
                return success ? page : StartupPage.TableView;
            }
            set
            {
                Settings.Set(Settings.DefaultPage, value.ToString());
            }
        }

        public static int PinLength
        {
            get
            {
                return Settings.Get(Settings.KeyPinLength, -1);
            }
            private set
            {
                Settings.Set(Settings.KeyPinLength, value);
            }
        }

        public static bool IsPinSet => PinLength != -1;

        public static bool IsFingerprintEnabled
        {
            get
            {
                return Settings.Get(Settings.KeyFingerprintSet, false);
            }
            set
            {
                Settings.Set(Settings.KeyFingerprintSet, value);
            }
        }
        public static bool LockByShaking
        {
            get
            {
                return Settings.Get(Settings.KeyLockByShaking, false);
            }
            set
            {
                Settings.Set(Settings.KeyLockByShaking, value);
            }
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

        private static readonly IEnumerable<string> CurrencyBlacklist = new[] { "CAD", "CNY", "EUR", "GBP", "JPY", "UAH", "USD" };
    }
}