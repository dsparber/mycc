using System;
using models;
using MyCryptos.resources;

namespace view.components
{
	public class ReferenceValueViewCell : CustomViewCell
	{
		Money money;
		ExchangeRate exchangeRate;

		public Money Money
		{
			get { return money; }
			set { money = value; setView(); }
		}

		public ExchangeRate ExchangeRate
		{
			get { return exchangeRate; }
			set { exchangeRate = value; setView(); }
		}

		public ReferenceValueViewCell()
		{
			setView();
		}

		void setView()
		{
			IsLoading = true;
			if (exchangeRate != null && exchangeRate.Rate.HasValue && money != null)
			{
				Text = new Money(money.Amount * exchangeRate.RateNotNull, exchangeRate.SecondaryCurrency).ToString();
				IsLoading = false;
			}
			else if (exchangeRate != null)
			{
				Text = string.Format("X {0}", exchangeRate.SecondaryCurrency.Code);
			}

			if (exchangeRate != null && exchangeRate.Rate.HasValue)
			{
				Detail = string.Format(InternationalisationResources.ExchangeRate, exchangeRate.Rate);
			}
			else {
				Detail = InternationalisationResources.NoExchangeRateFound;
			}
		}

		public override decimal Units { get { return Money.Amount * (ExchangeRate != null ? ExchangeRate.RateNotNull : 0); } }
		public override string Name { get { return ExchangeRate.SecondaryCurrency.Code; } }
		public override decimal Value { get { return Money.Amount * (ExchangeRate != null ? ExchangeRate.RateNotNull : 0); } }
	}
}

