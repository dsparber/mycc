using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace data.settings
{
	public static class Settings
	{
		public const string KEY_BASE_CURRENCY = "currency";

		public const string KEY_FIRST_LAUNCH = "first-launch";

		public const string KEY_SORT_ORDER = "sort-order";

		public const string KEY_SORT_DIRECTION = "sort-direction";

		public const string KEY_AUTO_REFRESH_ON_STARTUP = "auto-refresh-on-startup";

		public const string KEY_REFERENCE_CURRENCIES = "reference-currencies";

		public const string KEY_PIN = "pin";

		public const string KEY_PIN_LENGTH = "pin-length";

		public const string KEY_PIN_SET = "pin-set";

		public const string KEY_FINGERPRINT_SET = "fingerprint-set";

		public const string KEY_SHOW_GRAPH_ON_STARTUP = "show-graph-on-startup";

		public static T Get<T>(string key, T defaultValue)
		{
			return AppSettings.GetValueOrDefault(key, defaultValue);
		}

		public static void Set<T>(string key, T value)
		{
			AppSettings.AddOrUpdateValue(key, value);
		}

		static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}
	}
}

