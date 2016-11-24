using Xamarin.Forms;

namespace constants
{
	public static class AppConstants
	{
		public static readonly Color ThemeColor = Color.FromHex("#3498db");
		// #34495e, #f39c12, #3498db

		public static readonly int AndroidFontSize = 17;
		public static readonly Color FontColor = Color.FromHex("222");
		public static readonly Color BackgroundColor = (Device.OS == TargetPlatform.Android) ? Color.FromHex("EEE") : Color.FromHex("FFF");
		public static readonly Color TableBackgroundColor = (Device.OS == TargetPlatform.Android) ? Color.FromHex("EEE") : Color.FromRgb(238, 239, 243);
		public static readonly double OpacityDisabledField = 0.5;
		public static readonly Color FontColorLight = FontColor.MultiplyAlpha(OpacityDisabledField);

		public static readonly int LabelWidth = 100;

		public static readonly decimal PieGroupThreshold = 0.05M;
	}
}