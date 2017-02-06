using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components
{
    public class RatesHeaderComponent : HeaderView
    {
        private readonly Currency _currency;
        private readonly List<string> _infoTexts;

        private static readonly List<RatesHeaderComponent> Instances = new List<RatesHeaderComponent>();
        private static int _currentInfoText = 1;

        public RatesHeaderComponent(Currency currency) : base(true)
        {
            _currency = currency;

            _infoTexts = new List<string> { string.Empty, string.Empty, string.Empty };

            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += (sender, e) => SetInfoText(+1);
            GestureRecognizers.Add(recognizer);

            AddSubscriber();
            UpdateView();

            Instances.Add(this);
        }

        private void UpdateView()
        {
            _infoTexts[0] = _currency.Name;
            _infoTexts[1] = string.Join(" / ", ApplicationSettings.MainCurrencies
                            .Where(c => !c.Equals(_currency))
                            .Select(c => new Money(ExchangeRateHelper.GetRate(Currency.Btc, c)?.Rate ?? 0, c)
                            .ToStringTwoDigits(ApplicationSettings.RoundMoney)));
            _infoTexts[2] = ApplicationSettings.WatchedCurrencies
                            .Concat(ApplicationSettings.AllReferenceCurrencies)
                            .Concat(AccountStorage.UsedCurrencies)
                            .Select(e => new ExchangeRate(_currency, e))
                            .SelectMany(ExchangeRateHelper.GetNeededRates)
                            .Distinct()
                            .Select(e => ExchangeRateHelper.GetRate(e)?.LastUpdate ?? DateTime.Now).Min().LastUpdateString();

            Device.BeginInvokeOnMainThread(() =>
            {

                TitleText = Sum.ToStringTwoDigits(ApplicationSettings.RoundMoney);
                SetInfoText();
            });
        }

        private Money Sum => new Money(ExchangeRateHelper.GetRate(Currency.Btc, _currency)?.Rate ?? 0, _currency);

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

        private void SetInfoText(int increment = 0, bool updateOthers = true)
        {
            _currentInfoText = (_currentInfoText + increment) % _infoTexts.Count;

            if (_infoTexts == null || _infoTexts.Count < _currentInfoText) return;

            var text = _infoTexts[_currentInfoText];
            if (string.IsNullOrEmpty(text?.Trim()))
            {
                text = _infoTexts[(_currentInfoText + 1) % _infoTexts.Count];
            }
            InfoText = text;

            if (!updateOthers) return;

            foreach (var i in Instances)
            {
                i.SetInfoText(0, false);
            }
        }
    }
}
