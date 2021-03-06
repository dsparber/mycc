﻿using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currencies;
using MyCC.Core.Settings;
using MyCC.Forms.Helpers;
using MyCC.Forms.View.Components.Cells;
using MyCC.Forms.View.Components.CellViews;
using MyCC.Forms.View.Overlays;
using MyCC.Ui;
using MyCC.Ui.Messages;

namespace MyCC.Forms.View.Pages.Settings.Data
{
    public partial class WatchedCurrenciesSettingsView
    {
        public WatchedCurrenciesSettingsView()
        {
            InitializeComponent();
            SetReferenceCurrencyCells();

            Messaging.Modified.WatchedCurrencies.Subscribe(this, SetReferenceCurrencyCells);
        }

        private void SetReferenceCurrencyCells()
        {
            var currencyCells = new List<CustomViewCell>();

            var watchedCurrencies = ApplicationSettings.WatchedCurrencies.Select(CurrencyHelper.Find).ToList();
            Header.Info = PluralHelper.GetTextCurrencies(watchedCurrencies.Count);

            foreach (var c in watchedCurrencies)
            {
                var delete = new CustomCellViewActionItem { Icon = "delete.png", Data = c };
                var items = new List<CustomCellViewActionItem> { delete };

                delete.Action = (sender, e) => UiUtils.Edit.RemoveWatchedCurrency(c.Id);

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
            CurrencyOverlay.ShowAddRateOverlay(Navigation);
        }
    }
}
