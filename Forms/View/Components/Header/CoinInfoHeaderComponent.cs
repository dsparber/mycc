using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
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
            var text = new Money(ExchangeRateHelper.GetRate(_currency, Core.Currencies.CurrencyConstants.Btc)?.Rate ?? 0, Core.Currencies.CurrencyConstants.Btc).ToString8Digits();

            Device.BeginInvokeOnMainThread(() =>
            {
                InfoText = text;
                TitleText = _currency.Name ?? _currency.FindName();
            });
        }

        private void AddSubscriber()
        {
            Messaging.FetchingCoinInfo.SubscribeFinished(this, UpdateView);
            Messaging.Loading.SubscribeFinished(this, UpdateView);
        }
    }
}
