using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Forms.Resources;
using MyCryptos.view.components;

namespace MyCryptos.Forms.view.components.cells
{
	public class ReferenceValueViewCell : CustomViewCell
	{
		private Money money;
		private ExchangeRate exchangeRate;

		public Money Money
		{
			private get { return money; }
			set { money = value; SetView(); }
		}

		public ExchangeRate ExchangeRate
		{
			get { return exchangeRate; }
			set { exchangeRate = value; SetView(); }
		}

		public ReferenceValueViewCell()
		{
			SetView();
		}

		private void SetView()
		{
			if (exchangeRate?.Rate != null && money != null)
			{
				Text = new Money(money.Amount * exchangeRate.RateNotNull, exchangeRate.SecondaryCurrency).ToStringTwoDigits();
			}
			else if (exchangeRate?.SecondaryCurrency != null)
			{
				Text = new Money(0, exchangeRate.SecondaryCurrency).ToStringTwoDigits();
			}

			Detail = exchangeRate?.Rate != null ? string.Format(I18N.ExchangeRate, exchangeRate.Rate) : I18N.NoExchangeRateFound;
		}

		public override decimal Units => Money.Amount * (ExchangeRate?.RateNotNull ?? 0);
		public override string Name => (ExchangeRate != null) ? ExchangeRate.SecondaryCurrency.Code : string.Empty;
		public override decimal Value => Money.Amount * (ExchangeRate?.RateNotNull ?? 0);
	}
}