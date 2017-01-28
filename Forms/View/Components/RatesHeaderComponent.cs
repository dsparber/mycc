﻿using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.view.components
{
    public class RatesHeaderComponent : HeaderView
    {
        private readonly Currency currency;
        private readonly List<string> infoTexts;

        private static List<RatesHeaderComponent> instances = new List<RatesHeaderComponent>();
        private static int currentInfoText = 1;

        public RatesHeaderComponent(Currency currency, bool isUpdating) : base(true)
        {
            this.currency = currency;

            infoTexts = new List<string> { string.Empty, string.Empty };
            IsLoading = isUpdating;

            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += (sender, e) => SetInfoText(+1);
            GestureRecognizers.Add(recognizer);

            AddSubscriber();
            UpdateView();

            instances.Add(this);
        }

        private void UpdateView(bool? isLoading = null)
        {
            var amountDifferentCurrencies = AccountStorage.Instance.AllElements.Select(a => a.Money.Currency).Distinct().ToList().Count;

            infoTexts[0] = currency.Name;
            infoTexts[1] = string.Join(" / ", ApplicationSettings.MainCurrencies
                            .Where(c => !c.Equals(currency))
                            .Select(c => new Money(ExchangeRateHelper.GetRate(Currency.Btc, c)?.Rate ?? 0, c)
                            .ToStringTwoDigits(ApplicationSettings.RoundMoney)));

            Device.BeginInvokeOnMainThread(() =>
            {

                TitleText = Sum.ToStringTwoDigits(ApplicationSettings.RoundMoney);
                SetInfoText();

                if (isLoading.HasValue)
                {
                    IsLoading = isLoading.Value;
                }
            });
        }

        private Money Sum => new Money(ExchangeRateHelper.GetRate(Currency.Btc, currency)?.Rate ?? 0, currency);

        private void AddSubscriber()
        {
            Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => UpdateView());
            Messaging.RoundNumbers.SubscribeValueChanged(this, () => UpdateView());

            Messaging.FetchMissingRates.SubscribeFinished(this, () => UpdateView());
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, () => UpdateView());
            Messaging.UpdatingAccounts.SubscribeFinished(this, () => UpdateView());
            Messaging.Loading.SubscribeFinished(this, () => UpdateView());
        }

        private void SetInfoText(int increment = 0, bool updateOthers = true)
        {
            currentInfoText = (currentInfoText + increment) % infoTexts.Count;

            if (infoTexts == null || infoTexts.Count < currentInfoText) return;

            var text = infoTexts[currentInfoText];
            if (string.IsNullOrEmpty(text?.Trim()))
            {
                text = infoTexts[(currentInfoText + 1) % infoTexts.Count];
            }
            InfoText = text;

            if (!updateOthers) return;

            foreach (var i in instances)
            {
                i.SetInfoText(0, false);
            }
        }
    }
}