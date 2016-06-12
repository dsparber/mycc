using System;
using System.Globalization;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MyCryptos
{
	public static class Settings
	{
		private const string CultureInfoKey = "culture_info_key";
		private static readonly string CultureInfoDefault = "de-DE";

		private const string CurrencyKey = "currency_key";
		private static readonly String CurrencyDefault = Currency.EUR.Abbreviation;

		private const string UpdateTimeCurrencyListKey = "update_time_currency_list_key";
		private static readonly int UpdateTimeCurrencyListDefault = 3 * 60 * 1000; // Millis

		private const string UpdateTimeExchangeRateKey = "update_time_exchange_rate_key";
		private static readonly int UpdateTimeExchangeRateDefault = 2 * 60 * 1000; // Millis

		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		public static CultureInfo CultureInfo
		{
			get { return new CultureInfo(AppSettings.GetValueOrDefault<string>(CultureInfoKey, CultureInfoDefault)); }
			set { AppSettings.AddOrUpdateValue<string>(CultureInfoKey, value.Name); }
		}

		public static Currency ReferenceCurrency
		{
			get { return new Currency("", AppSettings.GetValueOrDefault<string>(CurrencyKey, CurrencyDefault)); }
			set { AppSettings.AddOrUpdateValue<string>(CurrencyKey, value.Abbreviation); }
		}

		public static int UpdateTimeCurrencyList
		{
			get { return AppSettings.GetValueOrDefault<int>(UpdateTimeCurrencyListKey, UpdateTimeCurrencyListDefault); }
			set { AppSettings.AddOrUpdateValue<int>(UpdateTimeCurrencyListKey, value); }
		}

		public static int UpdateTimeExchangeRate
		{
			get { return AppSettings.GetValueOrDefault<int>(UpdateTimeExchangeRateKey, UpdateTimeExchangeRateDefault); }
			set { AppSettings.AddOrUpdateValue<int>(UpdateTimeExchangeRateKey, value); }
		}
	}
}

