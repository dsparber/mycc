using System;
using models;

namespace data.settings
{
	public static class ApplicationSettings
	{
		static bool? firstLaunch;

		public static bool FirstLaunch
		{
			get
			{
				if (!firstLaunch.HasValue)
				{
					var persitedValue = Settings.Get(Settings.KEY_FIRST_LAUNCH, true);
					if (persitedValue)
					{
						Settings.Set(Settings.KEY_FIRST_LAUNCH, false);
					}
					firstLaunch = persitedValue;
				}
				return firstLaunch.Value;
			}
		}

		public static Currency BaseCurrency
		{
			get
			{
				return Settings.Get(Settings.KEY_BASE_CURRENCY, Currency.BTC);
			}
		}
	}
}