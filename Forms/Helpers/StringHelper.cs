using System;
using System.Globalization;
using MyCC.Forms.Resources;

namespace MyCC.Forms.Helpers
{
    public static class StringHelper
    {
        public static string LastUpdateString(this DateTime dateTime)
        {
            return $"{I18N.LastUpdate}: {dateTime.AsString()}";
        }

        public static string AsString(this DateTime dateTime)
        {
            var format = CultureInfo.CurrentCulture.DateTimeFormat;

            if (dateTime.Date == DateTime.MinValue.Date) return I18N.Never;
            else if (dateTime.Date != DateTime.Today) return dateTime.ToString($"{format.ShortDatePattern} {format.ShortTimePattern}");
            else return dateTime.ToString(format.ShortTimePattern);
        }
    }
}

