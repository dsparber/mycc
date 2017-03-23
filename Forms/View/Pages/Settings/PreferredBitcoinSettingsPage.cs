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
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components;
using MyCC.Forms.View.Components.Cells;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages.Settings
{
    public class PreferredBitcoinSettingsPage : ContentPage
    {
        private readonly List<Tuple<int, CustomViewCell>> _items;

        public PreferredBitcoinSettingsPage()
        {
            Title = I18N.PreferredBitcoinRate;
            BackgroundColor = AppConstants.TableBackgroundColor;

            var changingStack = new ChangingStackLayout();

            var header = new HeaderView(true)
            {
                TitleText = I18N.AppName,
                InfoText = PluralHelper.GetTextSourcs(ExchangeRatesStorage.Instance.Repositories.Count(r => r.RatesType == RateRepositoryType.CryptoToFiat))
            };

            changingStack.Children.Add(header);

            var query = ExchangeRatesStorage.Instance.Repositories
                    .Where(r => r.RatesType == RateRepositoryType.CryptoToFiat)
                    .OrderBy(r => r.Name)
                    .Select(r => Tuple.Create(r, GetDetailText(r.TypeId))).ToList();

            _items = query
                .Select(r => Tuple.Create(r.Item1.TypeId, new CustomViewCell()
                {
                    Text = r.Item1.Name,
                    Detail = r.Item2.Item1,
                    Image = "checkmark.png"
                }))
                .ToList();

            SetCheckmark();

            var section = new TableSection(I18N.Sources);
            var tableView = new TableView();
            tableView.Root.Add(section);

            foreach (var i in _items)
            {
                i.Item2.Tapped += (sender, args) =>
                {
                    ApplicationSettings.PreferredBitcoinRepository = _items.Find(x => x.Item2.Equals(sender)).Item1;
                    SetCheckmark();
                    Messaging.UpdatingRates.SendFinished();
                };
            }
            section.Add(_items.Select(e => e.Item2));

            var infoView = new InfoFooterComponent { Text = query.Min(q => q.Item2.Item2).LastUpdateString() };

            changingStack.Children.Add(new StackLayout
            {
                Spacing = 0,
                Children = {
                    tableView,
                    new InfoFooterComponent(false) { Text = $"* {I18N.InfoNoDirectRate}" },
                    infoView

                }
            });

            Content = changingStack;

            Task.Run(async () => await AppTaskHelper.FetchBtcUsdRates());
            Messaging.Progress.SubscribeToComplete(this, () => Device.BeginInvokeOnMainThread(() =>
            {
                var time = DateTime.Now;
                foreach (var i in _items)
                {
                    var detail = GetDetailText(i.Item1);
                    i.Item2.Detail = detail.Item1;
                    if (detail.Item2 < time)
                    {
                        time = detail.Item2;
                    }
                }
                infoView.Text = time.LastUpdateString();
            }));
        }

        private static Tuple<string, DateTime> GetDetailText(int i)
        {
            var usd = ExchangeRateHelper.GetStoredRate(Currency.Btc, Currency.Usd, i);
            var eur = ExchangeRateHelper.GetStoredRate(Currency.Btc, Currency.Eur, i);

            var usdString = (usd?.AsMoney ?? new Money(0, Currency.Usd)).ToStringTwoDigits(ApplicationSettings.RoundMoney);
            var eurString = (eur?.AsMoney ?? ExchangeRateHelper.GetRate(Currency.Btc, Currency.Eur, i)?.AsMoney ?? new Money(0, Currency.Eur)).ToStringTwoDigits(ApplicationSettings.RoundMoney);
            var note = eur == null && usd != null ? "*" : string.Empty;

            return Tuple.Create($"{eurString}{note} / {usdString}", usd?.LastUpdate ?? eur?.LastUpdate ?? DateTime.MinValue);
        }

        private void SetCheckmark()
        {
            foreach (var i in _items.Select(x => x.Item2)) i.ShowIcon = false;
            _items.Find(i => i.Item1 == ApplicationSettings.PreferredBitcoinRepository).Item2.ShowIcon = true;
        }
    }
}
