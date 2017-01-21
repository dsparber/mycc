using MyCC.Core.Account.Models.Base;
using MyCC.Core.ExchangeRate.Model;
using MyCryptos.Forms.Messages;
using MyCC.Forms.Resources;
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

            Messaging.RoundNumbers.SubscribeValueChanged(this, SetView);
        }

        private void SetView()
        {
            if (exchangeRate?.Rate != null && money != null)
            {
                Text = new Money(money.Amount * exchangeRate.RateNotNull, exchangeRate.SecondaryCurrency).ToString();
            }
            else if (exchangeRate?.SecondaryCurrency != null)
            {
                Text = new Money(0, exchangeRate.SecondaryCurrency).ToString();
            }
        }

        public override decimal Units => Money.Amount * (ExchangeRate?.RateNotNull ?? 0);
        public override string Name => (ExchangeRate != null) ? ExchangeRate.SecondaryCurrency.Code : string.Empty;
        public override decimal Value => Money.Amount * (ExchangeRate?.RateNotNull ?? 0);
    }
}