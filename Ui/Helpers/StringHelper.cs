using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyCC.Ui.Helpers
{
    public static class StringHelper
    {
        public static string AsString(this DateTime dateTime, string textNever)
        {
            var format = CultureInfo.CurrentCulture.DateTimeFormat;

            if (dateTime.Date == DateTime.MinValue.Date) return textNever;
            else if (dateTime.Date != DateTime.Today) return dateTime.ToString($"{format.ShortDatePattern} {format.ShortTimePattern}");
            else return dateTime.ToString(format.ShortTimePattern);
        }

        public static string MiddleTruncate(this string text, int charactersToShowCount = 5)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            var firstPart = string.Empty;
            var lastPart = string.Empty;

            if (text.Length > 2 * charactersToShowCount)
            {
                firstPart = text.Substring(0, charactersToShowCount);
                lastPart = text.Substring(text.Length - charactersToShowCount);
            }
            else if (text.Length > 2)
            {
                firstPart = text.Substring(0, text.Length / 2);
                lastPart = text.Substring(text.Length / 2 + 1);
            }

            return $"{firstPart}...{lastPart}";
        }

        public static ITextResolver TextResolver => DependencyService.Get<ITextResolver>();
    }
}

