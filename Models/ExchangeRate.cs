using System;

namespace MyCryptos
{
	public class ExchangeRate
	{
		public Currency ReferenceCurrency{ get; }

		public Currency SecondaryCurrency{ get; }

		public decimal Rate {
			get { 
				return Rate;
			}
			set {
				if (value > 0)
					Rate = value;
				else
					throw new Exception ("The exchange rate has to be greater than 0");
			}
		}


		public ExchangeRate (Currency referenceCurrency, Currency secondaryCurrency, decimal rate) : this (referenceCurrency, secondaryCurrency)
		{
			this.Rate = rate;
		}

		public ExchangeRate (Currency referenceCurrency, Currency secondaryCurrency)
		{
			this.ReferenceCurrency = referenceCurrency;
			this.SecondaryCurrency = secondaryCurrency;
		}

		public ExchangeRate GetInverse ()
		{
			ExchangeRate exchangeRate = new ExchangeRate (SecondaryCurrency, ReferenceCurrency);
			if (Rate > 0) {
				exchangeRate.Rate = 1 / Rate;
			}
			return exchangeRate;
		}

	}
}

