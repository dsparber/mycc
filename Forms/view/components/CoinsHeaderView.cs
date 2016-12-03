using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Helpers;
using MyCryptos.Core.Models;
using MyCryptos.Core.Settings;
using MyCryptos.Core.Storage;
using MyCryptos.Core.Tasks;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.components
{
    public class CoinsHeaderView : HeaderView
    {
        private readonly Currency currency;
        private readonly List<string> infoTexts;
        private static int currentInfoText = 1;

        public CoinsHeaderView(Currency currency)
        {
            this.currency = currency ?? ApplicationSettings.BaseCurrency;

            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += (sender, e) =>
            {
                currentInfoText = (currentInfoText + 1) % infoTexts.Count;
                InfoText = infoTexts[currentInfoText];
            };

            infoTexts = new List<string> { string.Empty, string.Empty };

            Padding = new Thickness(0, 0, 0, 20);

            GestureRecognizers.Add(recognizer);
            AddSubscriber();
            UpdateView();
        }

        private void UpdateView()
        {
            var sum = MoneySum;
            var amountDifferentCurrencies = AccountStorage.Instance.AllElements.Select(a => a.Money.Currency).Distinct().ToList().Count;

            TitleText = (sum.Amount > 0) ? sum.ToString() : $"0 {sum.Currency.Code}";
            infoTexts[0] = PluralHelper.GetText(I18N.NoCoins, I18N.OneCoin, I18N.Coins, amountDifferentCurrencies);
            infoTexts[1] = string.Join(" | ", ApplicationSettings.ReferenceCurrencies.Where(c => !c.Equals(currency)).Select(c => MoneySumOf(c)?.ToString() ?? $"0 {c.Code}"));
            InfoText = infoTexts[currentInfoText];
        }

        private Money MoneySum => MoneySumOf(currency);

        private static Money MoneySumOf(Currency currency)
        {
            var neededRates = new List<ExchangeRate>();

            var amount = AccountStorage.Instance.AllElements.Select(a =>
            {
                var neededRate = new ExchangeRate(a.Money.Currency, currency);
                var rate = ExchangeRateHelper.GetRate(neededRate);

                if (rate?.Rate == null)
                {
                    neededRates.Add(neededRate);
                }

                return a.Money.Amount * (rate ?? neededRate).RateNotNull;
            }).Sum();

            if (neededRates.Count == 0) return new Money(amount, currency);

            Messaging.UpdatingExchangeRates.SendStarted();
            ApplicationTasks.FetchMissingRates(neededRates, Messaging.UpdatingExchangeRates.SendFinished);

            return new Money(amount, currency);
        }

        private void AddSubscriber()
        {
            Messaging.ReferenceCurrency.SubscribeValueChanged(this, UpdateView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, UpdateView);

            Messaging.UpdatingExchangeRates.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => IsLoading = true), () => Device.BeginInvokeOnMainThread(() => IsLoading = false));
        }
    }
}
