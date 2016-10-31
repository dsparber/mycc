﻿using Plugin.Settings;
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

