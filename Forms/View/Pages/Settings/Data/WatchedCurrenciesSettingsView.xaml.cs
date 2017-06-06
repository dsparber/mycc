using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Settings;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.View.Components.Cells;
using MyCC.Forms.View.Components.CellViews;
using MyCC.Forms.View.Overlays;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages.Settings.Data
{
    public partial class WatchedCurrenciesSettingsView
    {
        public WatchedCurrenciesSettingsView()
        {
            InitializeComponent();
            SetReferenceCurrencyCells();

            Messaging.UpdatingRates.SubscribeFinished(this, SetReferenceCurrencyCells);
        }

        private void SetReferenceCurrencyCells()
        {
            var currencyCells = new List<CustomViewCell>();

            var watchedCurrencies = new List<string>(ApplicationSettings.WatchedCurrencies).ToList();
            Header.InfoText = PluralHelper.GetTextCurrencies(watchedCurrencies.Count);

            foreach (var c in watchedCurrencies)
            {
                var delete = new CustomCellViewActionItem { Icon = "delete.png", Data = c };
                var items = new List<CustomCellViewActionItem> { delete };

                delete.Action = (sender, e) =>
                {
                    var currencyId = (e as TappedEventArgs)?.Parameter as string;
                    watchedCurrencies.RemoveAll(x => x.Equals(currencyId));
                    ApplicationSettings.WatchedCurrencies = watchedCurrencies;
                    Messaging.UpdatingRates.SendFinished();
                    SetReferenceCurrencyCells();
                };

                var currency = c.Find();

                var cell = new CustomViewCell
                {
                    Text = currency.Code,
                    ActionItems = items,
                    Detail = currency.Name
                };
                currencyCells.Add(cell);
            }

            AllCurrenciesSection.Clear();
            AllCurrenciesSection.Add(currencyCells);
        }

        private void Add(object sender, EventArgs args)
        {
            CurrencyOverlay.ShowAddRateOverlay(Navigation, SetReferenceCurrencyCells);
        }
    }
}
