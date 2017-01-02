using System.Collections.Generic;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MyCryptos.Core.settings
{
	public static class Settings
	{
		private static Dictionary<string, object> cache = new Dictionary<string, object>();

		public const string KeyBaseCurrency = "currency";

		public const string KeyRatePageCurrency = "rate-page-currency";

		public const string KeyFirstLaunch = "first-launch";

		public const string KeySortOrderTable = "sort-order-table";
		public const string KeySortOrderRates = "sort-order-rates";
		public const string KeySortOrderAccounts = "sort-order-accounts";
		public const string KeySortOrderReferenceValues = "sort-order-reference";

		public const string KeySortDirectionTable = "sort-direction-table";
		public const string KeySortDirectionRates = "sort-direction-rates";
		public const string KeySortDirectionAccounts = "sort-direction-accounts";
		public const string KeySortDirectionReferenceValues = "sort-direction-reference";

		public const string KeyAutoRefreshOnStartup = "auto-refresh-on-startup";

		public const string KeyMainReferenceCurrencies = "main-reference-currencies";

		public const string KeyFurtherReferenceCurrencies = "further-reference-currencies";

		public const string KeyPin = "pin";

		public const string KeyPinLength = "pin-length";

		public const string KeyFingerprintSet = "fingerprint-set";

		public const string DefaultPage = "default-page";

		public const string RoundMoney = "round-money";

		public static T Get<T>(string key, T defaultValue)
		{
			if (cache.ContainsKey(key))
			{
				return (T)cache[key];
			}

			var value = AppSettings.GetValueOrDefault(key, defaultValue);
			cache.Add(key, value);
			return value;
		}

		public static void Set<T>(string key, T value)
		{
			if (!cache.ContainsKey(key) || !Equals(value, cache[key]))
			{
				AppSettings.AddOrUpdateValue(key, value);

				if (!cache.ContainsKey(key))
				{
					cache.Add(key, value);
				}
				else {
					cache[key] = value;
				}
			}
		}

		private static ISettings AppSettings => CrossSettings.Current;
	}
}

