using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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

        public static string CheckIfDecimal(this string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return string.Empty;

            var seperator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];

            if (val.StartsWith(seperator.ToString()))
            {
                val = $"0{val}";
            }

            if (char.IsDigit(val[0]) && (val.Count(x => x == seperator) == 0 || val.Count(x => x == seperator) == 1 &&
                                         $"{val}x".Split(new[] { seperator }, StringSplitOptions.RemoveEmptyEntries)[1].Length <= 9) &&
                Regex.IsMatch(val.Replace(seperator.ToString(), string.Empty), @"^\d+$"))
                return val;


            return val.Remove(val.Length - 1);
        }

        public static string TrimAll(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return string.Join(string.Empty, Regex.Replace(value, @"\t|\n|\r", "").Where(c => c != '\u200B')).Trim();
        }


        public static ITextResolver TextResolver => DependencyService.Get<ITextResolver>();
    }
}

