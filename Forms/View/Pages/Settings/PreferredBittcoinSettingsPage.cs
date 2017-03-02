using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Rates.Repositories.Interfaces;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components.CellViews;
using MyCC.Forms.View.Components;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages.Settings
{
    public class PreferredBittcoinSettingsPage : ContentPage
    {
        private readonly List<Tuple<int, CustomCellView>> _items;
        private readonly HeaderView _header;

        public PreferredBittcoinSettingsPage()
        {
            Title = I18N.PreferredBitcoinRate;
            BackgroundColor = AppConstants.TableBackgroundColor;

            var changingStack = new ChangingStackLayout();

            _header = new HeaderView(true)
            {
                TitleText = I18N.AppName,
                InfoText = ExchangeRatesStorage.PreferredBtcRepository.Name
            };

            changingStack.Children.Add(_header);

            _items = ExchangeRatesStorage.Instance.Repositories
                    .Where(r => r.RatesType == RateRepositoryType.CryptoToFiat)
                    .OrderBy(r => r.Name)
                    .Select(r => Tuple.Create(r.TypeId, new CustomCellView
                    {
                        Text = r.Name,
                        Detail = GetDetailText(r.TypeId),
                        Image = "checkmark.png"
                    }))
                    .ToList();

            SetCheckmark();

            var contentStack = new StackLayout { Spacing = 0 };
            contentStack.Children.Add(new SectionHeaderView { Title = I18N.AvailableRates });

            foreach (var i in _items)
            {
                var recognizer = new TapGestureRecognizer();
                recognizer.Tapped += (sender, args) =>
                {
                    ApplicationSettings.PreferredBitcoinRepository = _items.Find(x => x.Item2.Equals(sender)).Item1;
                    SetCheckmark();
                    Messaging.UpdatingRates.SendFinished();
                };
                i.Item2.GestureRecognizers.Add(recognizer);
                contentStack.Children.Add(i.Item2);
            }

            contentStack.Children.Add(new SectionFooterView { Text = $"*{I18N.InfoNoDirectRate}" });
            contentStack.Children.Last().Margin = new Thickness(0, 0, 0, 40);

            changingStack.Children.Add(new ScrollView { Content = contentStack });

            Content = changingStack;

            Task.Run(async () => await AppTaskHelper.FetchBtcUsdRates());
            Messaging.Progress.SubscribeToComplete(this, () => Device.BeginInvokeOnMainThread(() =>
            {
                foreach (var i in _items)
                {
                    i.Item2.Detail = GetDetailText(i.Item1);
                }
            }));
        }

        private static string GetDetailText(int i)
        {
            var usd = ExchangeRateHelper.GetStoredRate(Currency.Btc, Currency.Usd, i)?.AsMoney;
            var eur = ExchangeRateHelper.GetStoredRate(Currency.Btc, Currency.Eur, i)?.AsMoney;

            var usdString = (usd ?? new Money(0, Currency.Usd)).ToStringTwoDigits(ApplicationSettings.RoundMoney);
            var eurString = (eur ?? ExchangeRateHelper.GetRate(Currency.Btc, Currency.Eur, i)?.AsMoney ?? new Money(0, Currency.Eur)).ToStringTwoDigits(ApplicationSettings.RoundMoney);
            var note = eur == null && usd != null ? "*" : string.Empty;

            return $"{usdString} / {eurString}{note}";
        }

        private void SetCheckmark()
        {
            foreach (var i in _items.Select(x => x.Item2)) i.ShowIcon = false;
            _items.Find(i => i.Item1 == ApplicationSettings.PreferredBitcoinRepository).Item2.ShowIcon = true;
            _header.InfoText = ExchangeRatesStorage.PreferredBtcRepository.Name;
        }
    }
}
