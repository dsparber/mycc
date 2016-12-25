using System.Collections.Generic;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MyCryptos.Core.settings
{
	public static class Settings
	{
		private static Dictionary<string, object> cache = new Dictionary<string, object>();

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

