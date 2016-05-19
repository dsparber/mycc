using System;
using System.Globalization;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MyCryptos
{
	public static class PermanentSettings
	{
		private const string CultureInfoKey = "culture_info";
		private static readonly string CultureInfoDefault = "de-DE";

		private const string CurrencyKey = "int_key";
		private static readonly String CurrencyDefault = Currency.EUR.Abbreviation;

		private static ISettings AppSettings {
			get {
				return CrossSettings.Current;
			}
		}

		public static CultureInfo CultureInfo {
			get { return new CultureInfo (AppSettings.GetValueOrDefault<string> (CultureInfoKey, CultureInfoDefault)); }
			set { AppSettings.AddOrUpdateValue<string> (CultureInfoKey, value.Name); }
		}

		public static Currency ReferenceCurrency {
			get { return new Currency ("", AppSettings.GetValueOrDefault<string> (CurrencyKey, CurrencyDefault)); }
			set { AppSettings.AddOrUpdateValue<string> (CurrencyKey, value.Abbreviation); }
		}
	}
}

