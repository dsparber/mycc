using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyCC.Ui.Android.Helpers
{
    public static class TextEditHelper
    {
        public static string CheckIfDecimal(string val)
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
    }
}