using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCryptos.Core.Types;
using Newtonsoft.Json;
using PCLCrypto;

namespace MyCryptos.Core.settings
{
	public static class ApplicationSettings
	{
		private static bool? firstLaunch;
		public static bool FirstLaunch
		{
			get
			{
				if (firstLaunch.HasValue) return firstLaunch.Value;

				var persitedValue = Settings.Get(Settings.KeyFirstLaunch, true);
				if (persitedValue)
				{
					Settings.Set(Settings.KeyFirstLaunch, false);
				}
				firstLaunch = persitedValue;
				return firstLaunch.Value;
			}
		}

		public static Currency.Model.Currency BaseCurrency
		{
			get
			{
				var json = Settings.Get(Settings.KeyBaseCurrency, JsonConvert.SerializeObject(Currency.Model.Currency.Btc));
				var currency = JsonConvert.DeserializeObject<Currency.Model.Currency>(json);
				return currency;
			}
			set
			{
				Settings.Set(Settings.KeyBaseCurrency, JsonConvert.SerializeObject(value));
			}
		}

		public static Currency.Model.Currency SelectedRatePageCurrency
		{
			get
			{
				var json = Settings.Get(Settings.KeyRatePageCurrency, JsonConvert.SerializeObject(Currency.Model.Currency.Btc));
				var currency = JsonConvert.DeserializeObject<Currency.Model.Currency>(json);
				return currency;
			}
			set
			{
				Settings.Set(Settings.KeyRatePageCurrency, JsonConvert.SerializeObject(value));
			}
		}

		public static string Pin
		{
			set
			{
				PinLength = string.IsNullOrEmpty(value) ? -1 : value.Length;
				Settings.Set(Settings.KeyPin, Hash(value ?? string.Empty));
			}
		}

		public static bool IsPinValid(string pin)
		{
			var hash = Settings.Get(Settings.KeyPin, string.Empty);
			return hash.Equals(Hash(pin));
		}

		public static List<Currency.Model.Currency> MainReferenceCurrencies
		{
			get
			{
				var currencies = new List<Currency.Model.Currency> { Currency.Model.Currency.Btc, Currency.Model.Currency.Eur, Currency.Model.Currency.Usd };

				var defaultValue = JsonConvert.SerializeObject(currencies);

				var json = Settings.Get(Settings.KeyMainReferenceCurrencies, defaultValue);
				var data = JsonConvert.DeserializeObject<List<Currency.Model.Currency>>(json);
				data.RemoveAll(c => c.Equals(BaseCurrency));
				data.Add(BaseCurrency);
				return data.OrderBy(c => c.Code).ToList();
			}
			set
			{
				Settings.Set(Settings.KeyMainReferenceCurrencies, JsonConvert.SerializeObject(value));
			}
		}

		public static List<Currency.Model.Currency> FurtherReferenceCurrencies
		{
			get
			{
				var currencies = new List<Currency.Model.Currency>();

				var defaultValue = JsonConvert.SerializeObject(currencies);

				var json = Settings.Get(Settings.KeyFurtherReferenceCurrencies, defaultValue);
				var data = JsonConvert.DeserializeObject<List<Currency.Model.Currency>>(json);
				data.RemoveAll(MainReferenceCurrencies.Contains);
				return data.OrderBy(c => c.Code).ToList();
			}
			set
			{
				Settings.Set(Settings.KeyFurtherReferenceCurrencies, JsonConvert.SerializeObject(value));
			}
		}

		public static List<Currency.Model.Currency> AllReferenceCurrencies => MainReferenceCurrencies.Concat(FurtherReferenceCurrencies).ToList();

		public static SortOrder SortOrderTable
		{
			get
			{
				var defaultValue = SortOrder.Alphabetical;
				var stringValue = Settings.Get(Settings.KeySortOrderTable, defaultValue.ToString());
				var enumValue = (SortOrder)Enum.Parse(typeof(SortOrder), stringValue);
				return enumValue;
			}
			set
			{
				Settings.Set(Settings.KeySortOrderTable, value.ToString());
			}
		}

		public static SortDirection SortDirectionTable
		{
			get
			{
				var defaultValue = SortDirection.Ascending.ToString();
				var stringValue = Settings.Get(Settings.KeySortDirectionTable, defaultValue);
				return (SortDirection)Enum.Parse(typeof(SortDirection), stringValue);
			}
			set
			{
				Settings.Set(Settings.KeySortDirectionTable, value.ToString());
			}
		}

		public static SortOrder SortOrderRates
		{
			get
			{
				var defaultValue = SortOrder.Alphabetical;
				var stringValue = Settings.Get(Settings.KeySortOrderRates, defaultValue.ToString());
				var enumValue = (SortOrder)Enum.Parse(typeof(SortOrder), stringValue);
				return enumValue;
			}
			set
			{
				Settings.Set(Settings.KeySortOrderRates, value.ToString());
			}
		}

		public static SortDirection SortDirectionRates
		{
			get
			{
				var defaultValue = SortDirection.Ascending.ToString();
				var stringValue = Settings.Get(Settings.KeySortDirectionRates, defaultValue);
				return (SortDirection)Enum.Parse(typeof(SortDirection), stringValue);
			}
			set
			{
				Settings.Set(Settings.KeySortDirectionRates, value.ToString());
			}
		}

		public static SortOrder SortOrderAccounts
		{
			get
			{
				var defaultValue = SortOrder.Alphabetical;
				var stringValue = Settings.Get(Settings.KeySortOrderAccounts, defaultValue.ToString());
				var enumValue = (SortOrder)Enum.Parse(typeof(SortOrder), stringValue);
				return enumValue;
			}
			set
			{
				Settings.Set(Settings.KeySortOrderAccounts, value.ToString());
			}
		}

		public static SortDirection SortDirectionAccounts
		{
			get
			{
				var defaultValue = SortDirection.Ascending.ToString();
				var stringValue = Settings.Get(Settings.KeySortDirectionAccounts, defaultValue);
				return (SortDirection)Enum.Parse(typeof(SortDirection), stringValue);
			}
			set
			{
				Settings.Set(Settings.KeySortDirectionAccounts, value.ToString());
			}
		}

		public static SortOrder SortOrderReferenceValues
		{
			get
			{
				var defaultValue = SortOrder.Alphabetical;
				var stringValue = Settings.Get(Settings.KeySortOrderReferenceValues, defaultValue.ToString());
				var enumValue = (SortOrder)Enum.Parse(typeof(SortOrder), stringValue);
				return enumValue;
			}
			set
			{
				Settings.Set(Settings.KeySortOrderReferenceValues, value.ToString());
			}
		}

		public static SortDirection SortDirectionReferenceValues
		{
			get
			{
				var defaultValue = SortDirection.Ascending.ToString();
				var stringValue = Settings.Get(Settings.KeySortDirectionReferenceValues, defaultValue);
				return (SortDirection)Enum.Parse(typeof(SortDirection), stringValue);
			}
			set
			{
				Settings.Set(Settings.KeySortDirectionReferenceValues, value.ToString());
			}
		}

		public static bool AutoRefreshOnStartup
		{
			get
			{
				return Settings.Get(Settings.KeyAutoRefreshOnStartup, true);
			}
			set
			{
				Settings.Set(Settings.KeyAutoRefreshOnStartup, value);
			}
		}

		public static bool RoundMoney
		{
			get
			{
				return Settings.Get(Settings.RoundMoney, false);
			}
			set
			{
				Settings.Set(Settings.RoundMoney, value);
			}
		}

		public static StartupPage DefaultPage
		{
			get
			{
				return Settings.Get(Settings.DefaultPage, StartupPage.TableView);
			}
			set
			{
				Settings.Set(Settings.DefaultPage, value);
			}
		}

		public static int PinLength
		{
			get
			{
				return Settings.Get(Settings.KeyPinLength, -1);
			}
			private set
			{
				Settings.Set(Settings.KeyPinLength, value);
			}
		}

		public static bool IsPinSet => PinLength != -1;

		public static bool IsFingerprintEnabled
		{
			get
			{
				return Settings.Get(Settings.KeyFingerprintSet, false);
			}
			set
			{
				Settings.Set(Settings.KeyFingerprintSet, value);
			}
		}

		private static string ByteToString(IEnumerable<byte> buff)
		{
			var sBuilder = new StringBuilder();
			foreach (var t in buff)
			{
				sBuilder.Append(t.ToString("X2"));
			}
			return sBuilder.ToString().ToLower();
		}

		private static string Hash(string text)
		{
			var keyBytes = Encoding.UTF8.GetBytes(text);

			var algorithm = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha512);
			var hasher = algorithm.CreateHash(keyBytes);
			return ByteToString(hasher.GetValueAndReset());
		}
	}
}