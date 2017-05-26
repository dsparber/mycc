using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Header
{
    public class RatesHeaderComponent : HeaderView
    {
        private readonly Currency _currency;

        public RatesHeaderComponent(Currency currency) : base(true)
        {
            _currency = currency;

            AddSubscriber();
            UpdateView();
        }

        private void UpdateView()
        {
            var text = string.Join(" / ", ApplicationSettings.MainCurrencies
                            .Where(c => !c.Equals(_currency.Id))
                            .Select(c => new Money(ExchangeRateHelper.GetRate(CurrencyConstants.Btc.Id, c)?.Rate ?? 0, c.ToCurrency())
                            .ToStringTwoDigits(ApplicationSettings.RoundMoney)));

            text = string.IsNullOrWhiteSpace(text) ? _currency.Name : text;

            Device.BeginInvokeOnMainThread(() =>
            {
                InfoText = text;
                TitleText = Sum.ToStringTwoDigits(ApplicationSettings.RoundMoney);
            });
        }

        private Money Sum => new Money(ExchangeRateHelper.GetRate(CurrencyConstants.Btc, _currency)?.Rate ?? 0, _currency);

        private void AddSubscriber()
        {
            Messaging.ReferenceCurrency.SubscribeValueChanged(this, UpdateView);
            Messaging.RoundNumbers.SubscribeValueChanged(this, UpdateView);

            Messaging.FetchMissingRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingRates.SubscribeFinished(this, UpdateView);
            Messaging.Loading.SubscribeFinished(this, UpdateView);
        }

    }
}
