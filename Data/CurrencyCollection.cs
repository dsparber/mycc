using System;
using System.Collections.Generic;

namespace MyCryptos
{
	public class CurrencyCollection
	{
		public List<Currency> AvailableCurrencies{ get; }

		private CurrencyCollection ()
		{
			AvailableCurrencies = new List<Currency> ();
			AvailableCurrencies.Add (Currency.BTC);
			AvailableCurrencies.Add (Currency.EUR);
			AvailableCurrencies.Add (Currency.USD);
		}

		private static CurrencyCollection instance{ get; set; }

		public static CurrencyCollection Instance {
			get {
				if (instance == null)
					instance = new CurrencyCollection ();
				return instance;
			}
		}
	}
}

