using System;
using System.Collections.Generic;

namespace MyCryptos
{
	public class ExchangeRateCollection
	{

		public static ExchangeRateCollection Instance {
			get {
				if (Instance == null) {
					Instance = new ExchangeRateCollection ();
				}
				return Instance;
			}
			private set {
				Instance = value;
			}
		}

		public List<ExchangeRate> ExchangeRates{ get; }

		private ExchangeRateCollection ()
		{
			ExchangeRates = new List<ExchangeRate> ();
		}

		public ExchangeRate GetRate (Currency referenceCurrency, Currency secondaryCurrency)
		{
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

