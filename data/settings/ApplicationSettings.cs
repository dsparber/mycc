using System;
using MyCryptos.models;
using enums;
using Newtonsoft.Json;

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
				var json = Settings.Get(Settings.KEY_BASE_CURRENCY, JsonConvert.SerializeObject(Currency.BTC));
				var currency = JsonConvert.DeserializeObject<Currency>(json);
				return currency;
			}
			set
			{
				Settings.Set(Settings.KEY_BASE_CURRENCY, JsonConvert.SerializeObject(value));
			}
		}

		public static SortOrder SortOrder
		{
			get
			{
				var defaultValue = SortOrder.ALPHABETICAL.ToString();
				var stringValue = Settings.Get(Settings.KEY_SORT_ORDER, defaultValue);
				return (SortOrder)Enum.Parse(typeof(SortOrder), stringValue);
			}
			set
			{
				Settings.Set(Settings.KEY_SORT_ORDER, value.ToString());
			}
		}

		public static SortDirection SortDirection
		{
			get
			{
				var defaultValue = SortDirection.ASCENDING.ToString();
				var stringValue = Settings.Get(Settings.KEY_SORT_DIRECTION, defaultValue);
				return (SortDirection)Enum.Parse(typeof(SortDirection), stringValue);
			}
			set
			{
				Settings.Set(Settings.KEY_SORT_DIRECTION, value.ToString());
			}
		}

		public static bool AutoRefreshOnStartup
		{
			get
			{
				return Settings.Get(Settings.KEY_AUTO_REFRESH_ON_STARTUP, true);
			}
			set
			{
				Settings.Set(Settings.KEY_AUTO_REFRESH_ON_STARTUP, value);
			}
		}
	}
}