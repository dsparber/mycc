using System;
using System.Collections.Generic;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components
{
    public class CoinInfoHeaderComponent : HeaderView
    {
        private readonly Currency _currency;
        private readonly List<string> _infoTexts;

        private static readonly List<CoinInfoHeaderComponent> Instances = new List<CoinInfoHeaderComponent>();
        private static int _currentInfoText = 1;

        public CoinInfoHeaderComponent(Currency currency) : base(true)
        {
            _currency = currency;

            _infoTexts = new List<string> { string.Empty, string.Empty };

            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += (sender, e) => SetInfoText(+1);
            GestureRecognizers.Add(recognizer);

            AddSubscriber();
            UpdateView();

            Instances.Add(this);
        }

        private void UpdateView(bool? isLoading = null)
        {
            _infoTexts[0] = new Money(ExchangeRateHelper.GetRate(_currency, Currency.Btc)?.Rate ?? 0, Currency.Btc).ToString8Digits();
            _infoTexts[1] = (_currency.IsCryptoCurrency ? CoinInfoStorage.Instance.Get(_currency)?.LastUpdate ?? DateTime.MinValue : DateTime.Now).LastUpdateString();

            Device.BeginInvokeOnMainThread(() =>
            {

                TitleText = _currency.Name;
                SetInfoText();

                if (isLoading.HasValue)
                {
                    IsLoading = isLoading.Value;
                }
            });
        }

        private void AddSubscriber()
        {
            Messaging.FetchingCoinInfo.SubscribeFinished(this, () => UpdateView());
            Messaging.Loading.SubscribeFinished(this, () => UpdateView());
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
