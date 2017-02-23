using System;
using System.Collections.Generic;
using MyCC.Core.Currency.Model;
using MyCC.Core.Settings;
using MyCC.Forms.Messages;
using MyCC.Forms.view.components.CellViews;
using MyCC.Forms.View.Components.Cells;
using MyCC.Forms.View.Overlays;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages.Settings
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

            var watchedCurrencies = new List<Currency>(ApplicationSettings.WatchedCurrencies);

            foreach (var c in watchedCurrencies)
            {
                var delete = new CustomCellViewActionItem { Icon = "delete.png", Data = c };
                var items = new List<CustomCellViewActionItem> { delete };

                delete.Action = (sender, e) =>
                {
                    var cu = (e as TappedEventArgs)?.Parameter as Currency;
                    watchedCurrencies.Remove(cu);
                    ApplicationSettings.WatchedCurrencies = watchedCurrencies;
                    Messaging.UpdatingRates.SendFinished();
                    SetReferenceCurrencyCells();
                };

                var cell = new CustomViewCell
                {
                    Text = c.Code,
                    ActionItems = items,
                    Detail = c.Name
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
