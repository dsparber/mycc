using System;
using System.Collections.Generic;
using System.Net;

namespace MyCryptos
{
	public class ExchangeRateCollection
	{

		private static ExchangeRateCollection instance{ get; set; }

		public static ExchangeRateCollection Instance {
			get {
				if (instance == null) {
					instance = new ExchangeRateCollection ();
				}
				return instance;
			}
		}

		public List<ExchangeRate> ExchangeRates{ get; }

		private ExchangeRateCollection ()
		{
			ExchangeRates = new List<ExchangeRate> ();
		}

		public ExchangeRate GetRate (Currency referenceCurrency, Currency secondaryCurrency)
		{
			if (referenceCurrency.Equals (secondaryCurrency))
				return new ExchangeRate (referenceCurrency, secondaryCurrency, 1);
			
			foreach (ExchangeRate exchangeRate in ExchangeRates) {
				if (exchangeRate.ReferenceCurrency.Equals (referenceCurrency) && exchangeRate.SecondaryCurrency.Equals (secondaryCurrency)) {
					return exchangeRate;
				}
				if (exchangeRate.ReferenceCurrency.Equals (secondaryCurrency) && exchangeRate.SecondaryCurrency.Equals (referenceCurrency)) {
					return exchangeRate.GetInverse ();
				}
			}
			return null;
		}
	}
}

