using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Enums;
using MyCryptos.Core.Helpers;
using MyCryptos.Core.Models;
using MyCryptos.Core.Settings;
using MyCryptos.Core.Storage;
using MyCryptos.Core.Tasks;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
    public class CoinsHeaderView : HeaderView
    {
        Currency currency;
        private List<string> InfoTexts;
        private static int currentInfoText = 1;

        public CoinsHeaderView(Currency currency)
        {
            this.currency = currency ?? ApplicationSettings.BaseCurrency;

            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += (sender, e) =>
            {
                currentInfoText = (currentInfoText + 1) % InfoTexts.Count;
                InfoText = InfoTexts[currentInfoText];
            };

            var amountDifferentCurrencies = AccountStorage.Instance.AllElements.Select(a => a.Money.Currency).Distinct().ToList().Count;

            InfoTexts = new List<string> { string.Empty, string.Empty };

            Padding = new Thickness(0, 0, 0, 20);

            GestureRecognizers.Add(recognizer);
            addSubscriber();
            updateView();
        }

        void updateView()
        {
            var sum = moneySum;
            var amountDifferentCurrencies = AccountStorage.Instance.AllElements.Select(a => a.Money.Currency).Distinct().ToList().Count;

            TitleText = (sum.Amount > 0) ? sum.ToString() : $"0 {sum.Currency.Code}";
            InfoTexts[0] = PluralHelper.GetText(I18N.NoCoins, I18N.OneCoin, I18N.Coins, amountDifferentCurrencies);
            InfoTexts[1] = string.Join(" | ", ApplicationSettings.ReferenceCurrencies.Where(c => !c.Equals(currency)).Select(c => MoneySum(c)?.ToString() ?? $"0 {c.Code}"));
            InfoText = InfoTexts[currentInfoText];
        }

        Money moneySum => MoneySum(currency);

        private static Money MoneySum(Currency currency)
        {
            var neededRates = new List<ExchangeRate>();

            var amount = AccountStorage.Instance.AllElements.Select(a =>
            {
                var neededRate = new ExchangeRate(a.Money.Currency, currency);
                var rate = ExchangeRateHelper.GetRate(neededRate);

                if (rate == null || !rate.Rate.HasValue)
                {
                    neededRates.Add(neededRate);
                }

                return a.Money.Amount * (rate ?? neededRate).RateNotNull;
            }).Sum();

            if (neededRates.Count > 0)
            {
                Messaging.UpdatingExchangeRates.SendStarted();
                var task = ApplicationTasks.FetchMissingRates(neededRates);
                task.ContinueWith(t => Messaging.UpdatingExchangeRates.SendFinished());
            }

            return new Money(amount, currency);
        }

        void addSubscriber()
        {
            Messaging.ReferenceCurrency.SubscribeValueChanged(this, updateView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, updateView);

            Messaging.UpdatingExchangeRates.Subscribe(this, new List<Tuple<MessageInfo, Action>>
            {
                Tuple.Create<MessageInfo, Action>(MessageInfo.Started, () => IsLoading = true),
                Tuple.Create<MessageInfo, Action>(MessageInfo.Finished, () => IsLoading = false)
            });
        }
    }
}
