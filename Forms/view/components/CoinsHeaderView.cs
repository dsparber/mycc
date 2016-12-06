using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.settings;
using MyCryptos.Core.tasks;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.view.overlays;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.components
{
    public class CoinsHeaderView : HeaderView
    {

        private readonly Currency currency;
        private readonly bool useOnlyThisCurrency;
        private readonly List<string> infoTexts;

        private static int currentInfoText = 1;
        private static bool dataLoaded;
        private static bool sentMissing;
        private bool shouldBeLoading = true;

        public CoinsHeaderView(Currency currency = null, bool useOnlyThisCurrency = false) : this()
        {
            this.currency = currency ?? ApplicationSettings.BaseCurrency;
            this.useOnlyThisCurrency = useOnlyThisCurrency;

            UpdateView();
        }

        private CoinsHeaderView()
        {
            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += (sender, e) =>
            {
                currentInfoText = (currentInfoText + 1) % infoTexts.Count;
                InfoText = infoTexts[currentInfoText];
            };

            infoTexts = new List<string> { string.Empty, string.Empty };

            GestureRecognizers.Add(recognizer);
            AddSubscriber();
        }

        private void UpdateView(bool? isLoading = null)
        {
            var sum = (useOnlyThisCurrency ? CoinSum : MoneySum) ?? new Money(0, currency);
            var amountDifferentCurrencies = AccountStorage.Instance.AllElements.Select(a => a.Money.Currency).Distinct().ToList().Count;

            if (useOnlyThisCurrency)
            {
                infoTexts[0] = PluralHelper.GetText(I18N.NoAccounts, I18N.OneAccount, I18N.Accounts, AccountStorage.Instance.AllElements.Where(a => currency.Equals(a.Money.Currency)).ToList().Count);
            }
            else
            {
                infoTexts[0] = PluralHelper.GetText(I18N.NoCoins, I18N.OneCoin, I18N.Coins, amountDifferentCurrencies);
            }
            infoTexts[1] = string.Join(" / ", ApplicationSettings.ReferenceCurrencies.Where(c => !c.Equals(currency)).Select(c => ((useOnlyThisCurrency ? CoinSumAs(c) : MoneySumOf(c)) ?? new Money(0, c)).ToStringTwoDigits()));


            Device.BeginInvokeOnMainThread(() =>
            {
                if (dataLoaded)
                {
                    TitleText = useOnlyThisCurrency ? sum.ToString() : sum.ToStringTwoDigits();
                    InfoText = infoTexts[currentInfoText];
                    if (!shouldBeLoading)
                    {
                        IsLoading = false;
                    }
                }
                else
                {
                    shouldBeLoading = IsLoading;
                    IsLoading = true;
                }
                if (isLoading.HasValue)
                {
                    IsLoading = isLoading.Value;
                }
            });


        }

        private Money CoinSum => new Money(AccountStorage.Instance.AllElements.Where(a => currency.Equals(a.Money.Currency)).Sum(a => a.Money.Amount), currency);
        private Money CoinSumAs(Currency c) => new Money(CoinSum.Amount * (ExchangeRateHelper.GetRate(CoinSum.Currency, c)?.RateNotNull ?? 0), c);

        private Money MoneySum => MoneySumOf(currency);

        private static Money MoneySumOf(Currency currency)
        {
            var neededRates = new List<ExchangeRate>();

            var amount = AccountStorage.Instance.AllElements.Select(a =>
            {
                var neededRate = new ExchangeRate(a.Money.Currency, currency);
                var rate = ExchangeRateHelper.GetRate(neededRate);

                if (rate != null && rate.Rate == null)
                {
                    neededRates.Add(neededRate);
                }

                return a.Money.Amount * (rate ?? neededRate).RateNotNull;
            }).Sum();

            if (neededRates.Count == 0) return new Money(amount, currency);

            if (!sentMissing)
            {
                sentMissing = true;
                ApplicationTasks.FetchMissingRates(neededRates, Messaging.FetchMissingRates.SendStarted,
                    Messaging.FetchMissingRates.SendFinished, ErrorOverlay.Display);
            }
            else
            {
                sentMissing = false;
            }

            return new Money(amount, currency);
        }

        private void AddSubscriber()
        {
            Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => UpdateView());
            Messaging.UpdatingAccounts.SubscribeFinished(this, () => UpdateView());
            Messaging.Loading.SubscribeFinished(this, () => { dataLoaded = true; UpdateView(); });

            Messaging.FetchMissingRates.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => IsLoading = true), () => UpdateView(false));
            Messaging.UpdatingAccounts.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => IsLoading = true), () => UpdateView(false));
            Messaging.UpdatingAccountsAndRates.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => IsLoading = true), () => UpdateView(false));
        }
    }
}
