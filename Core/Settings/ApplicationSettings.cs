using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCC.Core.Types;
using Newtonsoft.Json;
using PCLCrypto;

namespace MyCC.Core.Settings
{
    public static class ApplicationSettings
    {
        private static MyCC.Core.Settings.Version _lastVersion;
        public static MyCC.Core.Settings.Version VersionLastLaunch
        {
            get
            {
                if (_lastVersion != null) return _lastVersion;

                if (!FirstLaunch)
                {
                    var persitedValue = new MyCC.Core.Settings.Version(Settings.Get(Settings.KeyAppVersion, "0.0.0"));
                    Settings.Set(Settings.KeyAppVersion, Constants.AppVersion.ToString());


                    _lastVersion = persitedValue;
                    return _lastVersion;
                }
                return Constants.AppVersion;
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

        public static Currency.Model.Currency BaseCurrency
        {
            get
            {
                var json = Settings.Get(Settings.KeyBaseCurrency, JsonConvert.SerializeObject(Currency.Model.Currency.Btc));
                var currency = JsonConvert.DeserializeObject<Currency.Model.Currency>(json);
                return currency;
            }
            set
            {
                Settings.Set(Settings.KeyBaseCurrency, JsonConvert.SerializeObject(value));
            }
        }

        public static Currency.Model.Currency SelectedRatePageCurrency
        {
            get
            {
                var json = Settings.Get(Settings.KeyRatePageCurrency, JsonConvert.SerializeObject(Currency.Model.Currency.Btc));
                var currency = JsonConvert.DeserializeObject<Currency.Model.Currency>(json);
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
                var data = JsonConvert.DeserializeObject<List<Currency.Model.Currency>>(json);
                return data.OrderBy(c => c.Code).ToList();
            }
            set
            {
                Settings.Set(Settings.KeyWatchedCurrencies, JsonConvert.SerializeObject(value));
            }
        }


        public static string Pin
        {
            set
            {
                PinLength = string.IsNullOrEmpty(value) ? -1 : value.Length;
                Settings.Set(Settings.KeyPin, Hash(value ?? string.Empty));
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
                var data = JsonConvert.DeserializeObject<List<Currency.Model.Currency>>(json);
                return data.OrderBy(c => c.Code).ToList();
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
                var data = JsonConvert.DeserializeObject<List<Currency.Model.Currency>>(json);
                data.RemoveAll(MainCurrencies.Contains);
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
                var defaultValue = SortOrder.Alphabetical;
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
                var defaultValue = SortOrder.Alphabetical;
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
                var defaultValue = SortOrder.Alphabetical;
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
                var defaultValue = SortOrder.ByUnits;
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

        public static bool RoundMoney
        {
            get
            {
                return Settings.Get(Settings.RoundMoney, false);
            }
            set
            {
                Settings.Set(Settings.RoundMoney, value);
            }
        }

        public static StartupPage DefaultPage
        {
            get
            {
                return Settings.Get(Settings.DefaultPage, StartupPage.TableView);
            }
            set
            {
                Settings.Set(Settings.DefaultPage, value);
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