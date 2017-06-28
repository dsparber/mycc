using System.Globalization;
using Xamarin.Forms;
using MyCC.Core.Resources;
using MyCC.Core.Settings;
using MyCC.Forms.Resources;

namespace MyCC.Forms.Constants
{
    public static class AppConstants
    {
        public static readonly Color ThemeColor = /*Color.FromHex("#3498db");*/ Color.FromHex("ba9043");
        // #34495e, #f39c12, #3498db

        public static readonly Color LaunchscreenTextColor = Color.FromHex("4E4E4E");
        public static readonly Color LaunchscreenBackground = Color.FromRgb(238, 239, 243);

        public const int AndroidFontSize = 17;
        public const double FontFactorSmall = 0.75;
        public static readonly Color BorderColor = Color.FromHex("ccc");
        public static readonly Color FontColor = Color.FromHex("222");
        public static readonly Color BackgroundColor = Color.FromHex("FFF");
        public static readonly Color TableBackgroundColor = Color.FromRgb(238, 239, 243);
        public static readonly Color TableSectionColor = Color.FromRgb(107, 107, 112);
        public const int TableSectionFontSize = 14;
        public static readonly Color FontColorLight = Color.FromRgb(107, 107, 112);
        public static readonly Color WarningColor = Color.FromHex("ff5722");

        public const int LabelWidth = 100;

        public static readonly CoreVersion AppVersion = new CoreVersion(1, 1, 3);

        public static string DevelopedBy
        {
            get
            {
                if (I18N.Culture == null) I18N.Culture = CultureInfo.CurrentCulture;
                return string.Format(I18N.DevelopedBy, ConstantNames.Developer);
            }
        }
    }

}
