using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Core.Types;
using Newtonsoft.Json.Linq;
using PCLCrypto;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MyCC.Core.Settings
{
    public static class SettingUtils
    {
        private static readonly Dictionary<string, object> Cache = new Dictionary<string, object>();
        private static ISettings AppSettings => CrossSettings.Current;

        public static IEnumerable<string> DefaultStaredReferenceCurrencies
        {
            get
            {
                var localCurrencyCode = RegionInfo.CurrentRegion.ISOCurrencySymbol;
                var defaultCurrencies = new List<string> { CurrencyConstants.Btc.Id, CurrencyConstants.Usd.Id, new Currency(localCurrencyCode, false).Find().Id };
                if (defaultCurrencies.Distinct().Count() == 2)
                {
                    defaultCurrencies.Add(CurrencyConstants.Eur.Id);
                }
                return defaultCurrencies.Distinct();
            }
        }

        public static IEnumerable<string> DefaultFurtherReferenceCurrencies
        {
            get
            {
                if (DefaultStaredReferenceCurrencies.Contains(CurrencyConstants.Eur.Id))
                {
                    return new List<string>();
                }
                return new[] { CurrencyConstants.Eur.Id };
            }
        }

        public static T Get<T>(this string key, T defaultValue)
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

        public static void Set<T>(this string key, T value)
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

        public static string Hash(this string text)
        {
            var keyBytes = Encoding.UTF8.GetBytes(text);

            var algorithm = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha512);
            var hasher = algorithm.CreateHash(keyBytes);
            return ByteToString(hasher.GetValueAndReset());
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

        public static IEnumerable<string> TryToLoadOldCurrencies(this string key)
        {
            var data = key.Get(string.Empty);
            if (string.IsNullOrEmpty(data)) return null;

            try
            {
                var currencies = JArray.Parse(data);
                return currencies.Select(c => (string)c["Id"]);
            }
            catch (Exception e)
            {
                data.LogInfo();
                e.LogError();
                return null;
            }
        }

        public static string TryToLoadOldCurrency(this string key)
        {
            var data = key.Get(string.Empty);
            if (string.IsNullOrEmpty(data)) return null;

            try
            {
                return (string)JObject.Parse(data)["Id"];
            }
            catch (Exception e)
            {
                data.LogInfo();
                e.LogError();
                return null;
            }
        }

        public static StartupPage TryToLoadOldStartupPage(this string key)
        {
            var data = key.Get(string.Empty);
            if (string.IsNullOrEmpty(data)) return StartupPage.RatesView;

            if (!int.TryParse(data, out int value)) return Enum.TryParse(data, out StartupPage page) ? page : StartupPage.RatesView;

            switch (value)
            {
                case 1: return StartupPage.GraphView;
                case 2: return StartupPage.TableView;
                default: return StartupPage.RatesView;
            }
        }

        public static void SetEnum<T>(this string key, T value) => key.Set(value.ToString());
        public static T GetEnum<T>(this string key, T defaultValue) => (T)Enum.Parse(typeof(T), key.Get(defaultValue.ToString()));

        public static IEnumerable<string> GetStrings(this string key, IEnumerable<string> defaultValues = null) => key.Get(string.Join(",", defaultValues ?? new string[] { })).Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s);
        public static void SetStrings(this string key, IEnumerable<string> values) => key.Set(string.Join(",", values.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s)));
    }
}