using Xamarin.Forms;

namespace MyCC.Forms.Constants
{
    public static class AppConstants
    {
        public static readonly Color ThemeColor = Color.FromHex("#3498db");
        // #34495e, #f39c12, #3498db

        public const int AndroidFontSize = 17;
        public const double FontFactorSmall = 0.75;
        public static readonly Color FontColor = Color.FromHex("222");
        public static readonly Color BackgroundColor = (Device.OS == TargetPlatform.Android) ? Color.FromHex("EEE") : Color.FromHex("FFF");
        public static readonly Color TableBackgroundColor = (Device.OS == TargetPlatform.Android) ? Color.FromHex("EEE") : Color.FromRgb(238, 239, 243);
        public static readonly Color TableSectionColor = Color.FromRgb(107, 107, 112);
        public const int TableSectionFontSize = 14;
        public const double OpacityDisabledField = 0.5;
        public static readonly Color FontColorLight = FontColor.MultiplyAlpha(OpacityDisabledField);

        public const int LabelWidth = 100;
    }
}