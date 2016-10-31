﻿using System.Collections.Generic;
using System.Threading.Tasks;
using data.storage;
using enums;
using MyCryptos.models;

namespace MyCryptos.helpers
{
	public static class ExchangeRateHelper
	{
		public static ExchangeRate GetRate(Currency referenceCurrency, Currency secondaryCurrency)
		{
			if (referenceCurrency == null || secondaryCurrency == null)
			{
				return null;
			}

			ExchangeRate rate = GetDirectRate(referenceCurrency, secondaryCurrency);

			if (rate != null)
			{
				return rate;
			}

			// Indirect match (one intermediate currency)
			var referenceCurrencyRates = AvailableRatesStorage.Instance.ExchangeRatesWithCurrency(referenceCurrency);
			var secondaryCurrencyRates = AvailableRatesStorage.Instance.ExchangeRatesWithCurrency(secondaryCurrency);

			foreach (var r1 in referenceCurrencyRates)
			{
				foreach (var r2 in secondaryCurrencyRates)
				{
					if (OneMatch(r1, r2))
					{
						var e1 = ExchangeRateStorage.Instance.Find(r1) ?? r1;
						var e2 = ExchangeRateStorage.Instance.Find(r2) ?? r2;
						return GetCombinedRate(e1, e2);
					}
				}
			}
			return null;
		}

		public static ExchangeRate GetDirectRate(Currency referenceCurrency, Currency secondaryCurrency)
		{
			if (referenceCurrency.Equals(secondaryCurrency))
			{
				return new ExchangeRate(referenceCurrency, secondaryCurrency, 1);
			}

			var exchangeRate = new ExchangeRate(referenceCurrency, secondaryCurrency);

			if (AvailableRatesStorage.Instance.IsAvailable(exchangeRate))
			{
				return ExchangeRateStorage.Instance.Find(exchangeRate);
			}
			if (AvailableRatesStorage.Instance.IsAvailable(exchangeRate.Inverse))
			{
				var e = ExchangeRateStorage.Instance.Find(exchangeRate.Inverse);
				return (e != null) ? e.Inverse : null;
			}

			return null;
		}

		public static async Task<ExchangeRate> GetRate(Currency referenceCurrency, Currency secondaryCurrency, FetchSpeedEnum speed)
		{
			if (referenceCurrency == null || secondaryCurrency == null)
			{
				return null;
			}

			ExchangeRate rate = await GetDirectRate(referenceCurrency, secondaryCurrency, speed);

			if (rate != null)
			{
				return rate;
			}

			// Indirect match (one intermediate currency)
			var referenceCurrencyRates = AvailableRatesStorage.Instance.ExchangeRatesWithCurrency(referenceCurrency);
			var secondaryCurrencyRates = AvailableRatesStorage.Instance.ExchangeRatesWithCurrency(secondaryCurrency);


			foreach (ExchangeRate r1 in referenceCurrencyRates)
			{
				foreach (ExchangeRate r2 in secondaryCurrencyRates)
				{
					if (OneMatch(r1, r2))
					{
						await AddRate(r1);
						await AddRate(r2);
						await fetch(speed);

						var e1 = ExchangeRateStorage.Instance.Find(r1) ?? r1;
						var e2 = ExchangeRateStorage.Instance.Find(r2) ?? r2;

						return GetCombinedRate(e1, e2);
					}
				}
			}
			return null;
		}

		public static async Task<ExchangeRate> GetDirectRate(Currency referenceCurrency, Currency secondaryCurrency, FetchSpeedEnum speed)
		{
			if (referenceCurrency.Equals(secondaryCurrency))
			{
				return new ExchangeRate(referenceCurrency, secondaryCurrency, 1);
			}

			var exchangeRate = new ExchangeRate(referenceCurrency, secondaryCurrency);

			var exists = AvailableRatesStorage.Instance.IsAvailable(exchangeRate);
			var existsInverse = AvailableRatesStorage.Instance.IsAvailable(exchangeRate.Inverse);

			if (exists || existsInverse)
			{
				await addAndFetch(!exists, speed, exchangeRate);

				if (exists)
				{
					return ExchangeRateStorage.Instance.Find(exchangeRate);
				}
				return ExchangeRateStorage.Instance.Find(exchangeRate.Inverse).Inverse;
			}
			return null;
		}

		static Task AddRate(ExchangeRate exchangeRate)
		{
			if (!ExchangeRateStorage.Instance.AllElements.Contains(exchangeRate))
			{
				foreach (var r in AvailableRatesStorage.Instance.Repositories)
				{
					if (r.IsAvailable(exchangeRate))
					{
						exchangeRate.RepositoryId = r.ExchangeRateRepository.Id;
						return r.ExchangeRateRepository.AddOrUpdate(exchangeRate);
					}
				}
			}
			return Task.Factory.StartNew(() => { });
		}

		static async Task addAndFetch(bool inverse, FetchSpeedEnum speed, ExchangeRate exchangeRate)
		{
			if (inverse)
			{
				await AddRate(exchangeRate.Inverse);
			}
			else {
				await AddRate(exchangeRate);
			}

			await fetch(speed);
		}

		static async Task fetch(FetchSpeedEnum speed)
		{
			if (speed == FetchSpeedEnum.SLOW)
			{
				await ExchangeRateStorage.Instance.Fetch();
			}
			else if (speed == FetchSpeedEnum.MEDIUM)
			{
				await ExchangeRateStorage.Instance.FetchNew();
			}
		}

		public static bool OneMatch(ExchangeRate r1, ExchangeRate r2)
		{
			return r1.Contains(r2.ReferenceCurrency) || r1.Contains(r2.SecondaryCurrency);
		}

		public static ExchangeRate GetCombinedRate(ExchangeRate rate1, ExchangeRate rate2)
		{
			var r = new ExchangeRate(DifferentCurrency(rate1, rate2), DifferentCurrency(rate2, rate1));

			var r1 = GetFor(rate1, CommonCurrency(rate1, rate2));
			var r2 = GetFor(rate2, CommonCurrency(rate2, rate1));

			r.Rate = r2.Rate / r1.Rate;

			return r;
		}

		public static Currency DifferentCurrency(ExchangeRate r1, ExchangeRate r2)
		{
			if (CommonCurrency(r1, r2) == null)
			{
				return null;
			}
			if (r2.Contains(r1.ReferenceCurrency))
				return r1.SecondaryCurrency;
			if (r2.Contains(r1.SecondaryCurrency))
				return r1.ReferenceCurrency;
			return null;
		}

		public static Currency CommonCurrency(ExchangeRate r1, ExchangeRate r2)
		{
			if (r2.Contains(r1.ReferenceCurrency))
				return r1.ReferenceCurrency;
			if (r2.Contains(r1.SecondaryCurrency))
				return r1.SecondaryCurrency;
			return null;
		}

		public static ExchangeRate GetFor(ExchangeRate rate, Currency currency)
		{
			if (currency.Equals(rate.ReferenceCurrency))
				return rate;
			if (currency.Equals(rate.SecondaryCurrency))
				return rate.Inverse;
			return null;
		}
	}
}
