namespace MyCryptos.models.helper
{
	public static class ExchangeRateHelper
	{
		public static bool OneMatch(ExchangeRate r1, ExchangeRate r2)
		{
			return r1.Contains(r2.ReferenceCurrency) || r1.Contains(r2.SecondaryCurrency);
		}

		public static ExchangeRate GetCombinedRate(ExchangeRate rate1, ExchangeRate rate2)
		{
			ExchangeRate r = new ExchangeRate(DifferentCurrency(rate1, rate2), DifferentCurrency(rate2, rate1));

			ExchangeRate r1 = GetFor(rate1, CommonCurrency(rate1, rate2));
			ExchangeRate r2 = GetFor(rate2, CommonCurrency(rate2, rate1));

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
				return rate.GetInverse();
			return null;
		}

	}
}

