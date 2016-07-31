using System.Collections.Generic;
using System;
using System.Diagnostics;
using models;

namespace MyCryptos
{
	public class ExchangeRateCollection
	{

		//private List<CurrencyAPI> currencyAPIs { get; }
		//private long lastUpdate;
		private bool firstCall;

		public List<ExchangeRate> ExchangeRates { get; }

		private ExchangeRateCollection()
		{
			ExchangeRates = new List<ExchangeRate>();
			// lastUpdate = 0;
			firstCall = true;

			// Add available APIs
			//currencyAPIs = new List<CurrencyAPI>();
			//currencyAPIs.Add(new BtceAPI());
			//currencyAPIs.Add(new BittrexAPI());
		}

		public void LoadRates(bool reload = false)
		{
			if (firstCall || reload)
			{
				firstCall = false;
				// lastUpdate = Environment.TickCount;

				//foreach (CurrencyAPI api in currencyAPIs)
				//{
				//	ExchangeRates.AddRange(await api.GetAvailableRatesAsync());
				//}
			}
		}

		public ExchangeRate GetRate(Currency referenceCurrency, Currency secondaryCurrency)
		{
			LoadRates();
			ExchangeRate rate = getDirectRate(referenceCurrency, secondaryCurrency);
			if (rate != null)
				return rate;

			// Indirect match (one intermediate currency)
			var referenceCurrencyRates = new List<ExchangeRate>();
			var secondaryCurrencyRates = new List<ExchangeRate>();

			foreach (ExchangeRate exchangeRate in ExchangeRates)
			{
				if (exchangeRate.Contains(referenceCurrency))
				{
					referenceCurrencyRates.Add(exchangeRate);
				}
				if (exchangeRate.Contains(secondaryCurrency))
				{
					secondaryCurrencyRates.Add(exchangeRate);
				}
			}

			foreach (ExchangeRate r1 in referenceCurrencyRates)
			{
				//foreach (ExchangeRate r2 in secondaryCurrencyRates)
				//{
				//	if (r1.OneMatch(r2))
				//	{
				//		await LoadRateFor(r1);
				//		await LoadRateFor(r2);
				//		return r1.GetCombinedRate(r2);
				//	}
				//}
			}
			return null;
		}

		private ExchangeRate getDirectRate(Currency referenceCurrency, Currency secondaryCurrency)
		{
			if (referenceCurrency.Equals(secondaryCurrency))
				return new ExchangeRate(referenceCurrency, secondaryCurrency, 1);

			foreach (ExchangeRate exchangeRate in ExchangeRates)
			{
				if (exchangeRate.ReferenceCurrency.Equals(referenceCurrency) && exchangeRate.SecondaryCurrency.Equals(secondaryCurrency))
				{
					LoadRateFor(exchangeRate);
					return exchangeRate;
				}
				if (exchangeRate.ReferenceCurrency.Equals(secondaryCurrency) && exchangeRate.SecondaryCurrency.Equals(referenceCurrency))
				{
					LoadRateFor(exchangeRate);
					//return exchangeRate.GetInverse();
				}
			}
			return null;
		}

		private void LoadRateFor(ExchangeRate exchangeRate, bool reload = false)
		{
			if (!reload)
			{
				var findRate = ExchangeRates.Find(item => item.Equals(exchangeRate));
				Debug.WriteLine(findRate);
				if (findRate != null && findRate.Rate != null)
				{
					return;
				}
			}

			//foreach (CurrencyAPI api in currencyAPIs)
			//{
			//	if ((await api.GetAvailableRatesAsync()).Contains(exchangeRate))
			//	{
			//		await api.GetExchangeRateAsync(exchangeRate);
			//		return;
			//	}
			//}
		}

		private static ExchangeRateCollection instance { get; set; }
		public static ExchangeRateCollection Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ExchangeRateCollection();
				}
				return instance;
			}
		}
	}
}