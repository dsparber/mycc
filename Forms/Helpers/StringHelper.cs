using System;
using System.Globalization;
using System.Linq;
using MyCC.Forms.Resources;

namespace MyCC.Forms.Helpers
{
    public static class StringHelper
    {
        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.First().ToString().ToUpper() + string.Join("", input.Skip(1));
        }

        public static string LastUpdateString(this DateTime dateTime)
        {
            var format = CultureInfo.CurrentCulture.DateTimeFormat;
            var dateTimeString = dateTime.Subtract(DateTime.MinValue).Days == 0 ? I18N.Never : dateTime.ToString(DateTime.Now.Subtract(dateTime).Days > 0 ? format.ShortDatePattern : format.ShortTimePattern);
            return $"{I18N.LastUpdate}: {dateTimeString}";
        }
    }
}
