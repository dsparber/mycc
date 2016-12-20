using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyCryptos.Core.Account.Helper
{
	public static class AccountStringHelper
	{
		public static Tuple<string, Currency.Model.Currency, string> Parse(this string qrCodeText, IEnumerable<Currency.Model.Currency> supportedCurrencies)
		{
			var i1 = Math.Max(0, qrCodeText.IndexOf(':') + 1);
			var i2 = qrCodeText.IndexOf('?');
			i2 = i2 > 0 ? i2 - 1 : qrCodeText.Length;

			var address = qrCodeText.Substring(i1, i2 - i1);
			var currencyString = i1 != 0 ? qrCodeText.Split(':')[0] : null;
			var argsText = qrCodeText.Contains('?') ? qrCodeText.Split('?')[1] : null;

			var currency = supportedCurrencies.FirstOrDefault(c => c.Name.EqualsIgnore(currencyString) || c.Code.EqualsIgnore(currencyString));

			var args = argsText?.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
			   .Select(part => part.Split('='))
			   .ToDictionary(split => split[0], split => split[1]);

			var name = args?.GetValueOrDefault("label", null);
			name = name != null ? Uri.UnescapeDataString(name) : null;

			return Tuple.Create(address, currency, name);
		}

		private static bool EqualsIgnore(this string s1, string s2)
		{
			if (s1 == null || s2 == null)
			{
				return false;
			}

			var t1 = Regex.Replace(s1, @"\s+", string.Empty);
			var t2 = Regex.Replace(s2, @"\s+", string.Empty);

			return t1.Equals(t2, StringComparison.CurrentCultureIgnoreCase);
		}

		private static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			TValue value;
			return dictionary.TryGetValue(key, out value) ? value : defaultValue;
		}
	}
}
