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
			return $"{I18N.LastUpdate}: {dateTime.AsString()}";
		}

		public static string AsString(this DateTime dateTime)
		{
			var format = CultureInfo.CurrentCulture.DateTimeFormat;

			if (dateTime.Date == DateTime.MinValue.Date) return I18N.Never;
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

	}
}

