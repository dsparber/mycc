using MyCryptos.Core.Models;
using MyCryptos.Forms.Resources;

namespace MyCryptos.view.components
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
            else if (exchangeRate != null && exchangeRate.SecondaryCurrency != null)
            {
                Text = string.Format("0 {0}", exchangeRate.SecondaryCurrency.Code);
            }

            if (exchangeRate != null && exchangeRate.Rate.HasValue)
            {
                Detail = string.Format(I18N.ExchangeRate, exchangeRate.Rate);
            }
            else
            {
                Detail = I18N.NoExchangeRateFound;
            }
        }

        public override decimal Units { get { return Money.Amount * (ExchangeRate != null ? ExchangeRate.RateNotNull : 0); } }
        public override string Name { get { return (ExchangeRate != null) ? ExchangeRate.SecondaryCurrency.Code : string.Empty; } }
        public override decimal Value { get { return Money.Amount * (ExchangeRate != null ? ExchangeRate.RateNotNull : 0); } }
    }
}