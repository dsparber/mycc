using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Forms.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Header
{
    public class CoinInfoHeaderComponent : HeaderView
    {
        private readonly Currency _currency;

        public CoinInfoHeaderComponent(Currency currency) : base(true)
        {
            _currency = currency;
            AddSubscriber();
            UpdateView();
        }

        private void UpdateView()
        {
            var text = new Money(ExchangeRateHelper.GetRate(_currency, Currency.Btc)?.Rate ?? 0, Currency.Btc).ToString8Digits();

            Device.BeginInvokeOnMainThread(() =>
            {
                InfoText = text;
                TitleText = _currency.Name;
            });
        }

        private void AddSubscriber()
        {
            Messaging.FetchingCoinInfo.SubscribeFinished(this, UpdateView);
            Messaging.Loading.SubscribeFinished(this, UpdateView);
        }
    }
}
