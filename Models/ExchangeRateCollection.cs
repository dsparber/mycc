using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

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

		public async Task LoadRates ()
		{
			ExchangeRates.AddRange (await new BtceAPI ().GetExchangeRatesAsync ());
		}

		public ExchangeRate GetRate (Currency referenceCurrency, Currency secondaryCurrency)
		{
			ExchangeRate rate = getDirectRate (referenceCurrency, secondaryCurrency);
			if (rate != null)
				return rate;

			// Indirect match (one intermediate currency)
			List<ExchangeRate> referenceCurrencyRates = new List<ExchangeRate> ();
			List<ExchangeRate> secondaryCurrencyRates = new List<ExchangeRate> ();

			foreach (ExchangeRate exchangeRate in ExchangeRates) {
				if (exchangeRate.Contains (referenceCurrency)) {
					referenceCurrencyRates.Add (exchangeRate);
				}
				if (exchangeRate.Contains (secondaryCurrency)) {
					secondaryCurrencyRates.Add (exchangeRate);
				}
			}

			foreach (ExchangeRate r1 in referenceCurrencyRates) {
				foreach (ExchangeRate r2 in secondaryCurrencyRates) {
					if (r1.OneMatch (r2)) {
						return r1.GetCombinedRate (r2);
					}
				}
			}
			return null;
		}

		private ExchangeRate getDirectRate (Currency referenceCurrency, Currency secondaryCurrency)
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

