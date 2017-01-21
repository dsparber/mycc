using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.ExchangeRate.Helpers;
using MyCC.Core.ExchangeRate.Model;
using MyCC.Core.Settings;
using MyCC.Forms.helpers;
using MyCC.Forms.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.view.components
{
    public class CoinHeaderComponent : HeaderView
    {

        private readonly Currency currency;
        private readonly bool useOneBitcoinAsReference;
        private readonly FunctionalAccount account;
        private readonly bool useOnlyThisCurrency;
        private readonly List<string> infoTexts;

        private static int currentInfoText = 1;

        public CoinHeaderComponent(FunctionalAccount account) : this()
        {
            this.account = account;
            currency = account.Money.Currency;
            useOnlyThisCurrency = true;


            UpdateView();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            SetInfoText();
        }

        public CoinHeaderComponent(Currency currency = null, bool useOnlyThisCurrency = false, bool useOneBitcoinAsReference = false) : this()
        {
            this.currency = currency ?? ApplicationSettings.BaseCurrency;
            this.useOneBitcoinAsReference = useOneBitcoinAsReference;
            this.useOnlyThisCurrency = useOnlyThisCurrency;

            UpdateView();
        }

        private CoinHeaderComponent() : base(true)
        {
            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += (sender, e) =>
            {
                currentInfoText = (currentInfoText + 1) % infoTexts.Count;
                SetInfoText();
            };

            infoTexts = new List<string> { string.Empty, string.Empty };

            GestureRecognizers.Add(recognizer);
            AddSubscriber();
        }

        private void UpdateView(bool? isLoading = null)
        {
            var amountDifferentCurrencies = AccountStorage.Instance.AllElements.Select(a => a.Money.Currency).Distinct().ToList().Count;

            if (useOneBitcoinAsReference)
            {
                infoTexts[0] = currency.Name;
            }
            else if (account != null)
            {
                infoTexts[0] = account.Name;
            }
            else if (useOnlyThisCurrency)
            {
                infoTexts[0] = PluralHelper.GetTextAccounts(AccountStorage.AccountsWithCurrency(currency).Count);
            }
            else
            {
                infoTexts[0] = PluralHelper.GetTextCurrencies(amountDifferentCurrencies);
            }
            infoTexts[1] = (useOneBitcoinAsReference) ? currency?.Name : string.Join(" / ", ApplicationSettings.MainCurrencies
                                       .Where(c => !c.Equals(currency))
                                                                                     .Select(c => (useOneBitcoinAsReference ? new Money(ExchangeRateHelper.GetRate(Currency.Btc, c)?.RateNotNull ?? 0, c)
                                                     : (useOnlyThisCurrency ? CoinSumAs(c)
                                                        : MoneySumOf(c)) ?? new Money(0, c))
                                               .ToStringTwoDigits(ApplicationSettings.RoundMoney)));


            Device.BeginInvokeOnMainThread(() =>
            {
                if (useOnlyThisCurrency && !useOneBitcoinAsReference)
                {
                    var s = Sum.ToString(false);
                    var beforeDecimal = new Money(Math.Truncate(Sum.Amount), Sum.Currency).ToString(false);
                    var decimals = s.Remove(0, beforeDecimal.Length);
                    var i1 = decimals.IndexOf(".", StringComparison.CurrentCulture);
                    var i2 = decimals.IndexOf(",", StringComparison.CurrentCulture);
                    var i = i1 > i2 ? i1 : i2;
                    i = i == -1 ? s.Length : i;
                    i += 4 + beforeDecimal.Length;
                    i = i > s.Length ? s.Length : i;
                    TitleText = s.Substring(0, i);
                    TitleTextSmall = s.Substring(i);
                }
                else
                {
                    TitleText = Sum.ToStringTwoDigits(ApplicationSettings.RoundMoney);
                }

                SetInfoText();

                if (isLoading.HasValue)
                {
                    IsLoading = isLoading.Value;
                }
            });
        }

        private Money Sum => useOneBitcoinAsReference ? new Money(ExchangeRateHelper.GetRate(Currency.Btc, currency).RateNotNull, currency) : account != null ? account.Money : (useOnlyThisCurrency ? CoinSum : MoneySum) ?? new Money(0, currency);

        private Money CoinSum => account != null ? account.Money : new Money(AccountStorage.Instance.AllElements.Where(a => currency.Equals(a.Money.Currency)).Sum(a => a.Money.Amount), currency);
        private Money CoinSumAs(Currency c) => new Money(CoinSum.Amount * (ExchangeRateHelper.GetRate(CoinSum.Currency, c)?.RateNotNull ?? 0), c);

        private Money MoneySum => MoneySumOf(currency);

        private static Money MoneySumOf(Currency currency)
        {
            var amount = AccountStorage.Instance.AllElements.Sum(a =>
            {
                var neededRate = new ExchangeRate(a.Money.Currency, currency);
                var rate = ExchangeRateHelper.GetRate(neededRate);

                if (rate != null && rate.Rate == null)
                {
                }

                return a.Money.Amount * (rate ?? neededRate).RateNotNull;
            });

            return new Money(amount, currency);
        }

        private void AddSubscriber()
        {
            Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => UpdateView());
            Messaging.RoundNumbers.SubscribeValueChanged(this, () => UpdateView());

            Messaging.FetchMissingRates.SubscribeFinished(this, () => UpdateView());
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, () => UpdateView());
            Messaging.UpdatingAccounts.SubscribeFinished(this, () => UpdateView());
            Messaging.Loading.SubscribeFinished(this, () => UpdateView());
        }

        void SetInfoText()
        {
            if (infoTexts != null && infoTexts.Count >= currentInfoText)
            {
                var text = infoTexts[currentInfoText];
                if (string.IsNullOrEmpty(text?.Trim()))
                {
                    text = infoTexts[(currentInfoText + 1) % infoTexts.Count];
                }
                InfoText = text;
            }
        }
    }
}
