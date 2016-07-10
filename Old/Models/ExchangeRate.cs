﻿using System;
using System.Runtime.Serialization;

namespace MyCryptos
{
	public class ExchangeRate
	{
		public Currency ReferenceCurrency { get; }

		public Currency SecondaryCurrency { get; }

		private decimal rate { get; set; }

		public decimal? Rate
		{
			get 
			{
				if (rate == 0)
				{
					return null;
				}
				return rate; 
			}
			set
			{
				if (value != null && value > 0)
				{
					rate = (decimal)value;
					lastUpdate = Environment.TickCount;
				}
			}
		}

		private long lastUpdate { get; set; }
		public long LastUpdate { get { return lastUpdate; } }

		public ExchangeRate(Currency referenceCurrency, Currency secondaryCurrency, decimal rate) : this(referenceCurrency, secondaryCurrency)
		{
			this.Rate = rate;
		}

		public ExchangeRate(Currency referenceCurrency, Currency secondaryCurrency)
		{
			this.ReferenceCurrency = referenceCurrency;
			this.SecondaryCurrency = secondaryCurrency;
			this.Rate = null;
			this.lastUpdate = 0;
		}

		public ExchangeRate GetFor(Currency currency)
		{
			if (currency.Equals(ReferenceCurrency))
				return this;
			if (currency.Equals(SecondaryCurrency))
				return GetInverse();
			return null;
		}

		public ExchangeRate GetInverse()
		{
			ExchangeRate exchangeRate = new ExchangeRate(SecondaryCurrency, ReferenceCurrency);
			if (Rate > 0)
			{
				exchangeRate.Rate = 1 / Rate;
			}
			return exchangeRate;
		}

		public ExchangeRate GetCombinedRate(ExchangeRate rate)
		{
			ExchangeRate r = new ExchangeRate(DifferentCurrency(rate), rate.DifferentCurrency(this));

			ExchangeRate r1 = GetFor(CommonCurrency(rate));
			ExchangeRate r2 = rate.GetFor(rate.CommonCurrency(this));

			r.Rate = r2.Rate / r1.Rate;

			return r;
		}

		public bool Contains(Currency currency)
		{
			return ReferenceCurrency.Equals(currency) || SecondaryCurrency.Equals(currency);
		}

		public bool OneMatch(ExchangeRate rate)
		{
			return rate.Contains(ReferenceCurrency) || rate.Contains(SecondaryCurrency);
		}

		public Currency CommonCurrency(ExchangeRate rate)
		{
			if (rate.Contains(ReferenceCurrency))
				return ReferenceCurrency;
			if (rate.Contains(SecondaryCurrency))
				return SecondaryCurrency;
			return null;
		}

		public Currency DifferentCurrency(ExchangeRate rate)
		{
			if (CommonCurrency(rate) == null)
			{
				return null;
			}
			if (rate.Contains(ReferenceCurrency))
				return SecondaryCurrency;
			if (rate.Contains(SecondaryCurrency))
				return ReferenceCurrency;
			return null;
		}

		public override bool Equals(Object obj)
		{
			// Check for null values and compare run-time types.
			if (obj == null || GetType() != obj.GetType())
				return false;

			ExchangeRate r = (ExchangeRate)obj;

			return r.ReferenceCurrency.Equals(ReferenceCurrency) && r.SecondaryCurrency.Equals(SecondaryCurrency);
		}


		public override int GetHashCode()
		{
			return ReferenceCurrency.GetHashCode() + SecondaryCurrency.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("[ExchangeRate: ReferenceCurrency={0}, SecondaryCurrency={1}, Rate={2}, LastUpdate={3}]", ReferenceCurrency, SecondaryCurrency, Rate, LastUpdate);
		}
	}
}
