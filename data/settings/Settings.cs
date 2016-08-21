using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace data.settings
{
	public static class Settings
	{
		public const string KEY_BASE_CURRENCY = "currency";

		public const string KEY_FIRST_LAUNCH = "first-launch";

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

