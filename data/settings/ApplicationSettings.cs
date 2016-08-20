using System;
namespace data.settings
{
	public static class ApplicationSettings
	{
		public static bool FirstLaunch
		{
			get
			{
				var firstLaunch = Settings.Get(Settings.KEY_FIRST_LAUNCH, true);
				if (firstLaunch)
				{
					Settings.Set(Settings.KEY_FIRST_LAUNCH, false);
				}
				return firstLaunch;
			}
		}
	}
}