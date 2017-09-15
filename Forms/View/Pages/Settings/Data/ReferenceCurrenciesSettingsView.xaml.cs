using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Settings;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;
using MyCC.Forms.View.Components.CellViews;
using MyCC.Forms.View.Overlays;
using MyCC.Ui;
using MyCC.Ui.Messages;

namespace MyCC.Forms.View.Pages.Settings.Data
{
    public partial class ReferenceCurrenciesSettingsView
    {
        public ReferenceCurrenciesSettingsView()
        {
            InitializeComponent();

            Header.Info = PluralHelper.GetTextCurrencies(ApplicationSettings.AllReferenceCurrencies.Count());
            SetCells();
            Messaging.Modified.ReferenceCurrencies.Subscribe(this, SetCells);
        }

        private void SetCells()
        {
            CurrenciesSection.Clear();
            foreach (var currency in ApplicationSettings.AllReferenceCurrencies.OrderBy(id => id))
            {
                var cell = GetCell(currency);
                CurrenciesSection.Add(cell);
            }
        }

        private void Add(object sender, EventArgs args)
        {
            var currenciesTask = new Func<IEnumerable<Currency>>(() => CurrencyStorage.Instance.Currencies.Where(c => !ApplicationSettings.AllReferenceCurrencies.Contains(c.Id)));

            var overlay = new CurrencyOverlay(currenciesTask, I18N.AddReferenceCurrency)
            {
                CurrencySelected = c => UiUtils.Edit.AddReferenceCurrency(c.Id)
            };
            Navigation.PushAsync(overlay);
        }

        private static CustomViewCell GetCell(string currencyId)
        {
            var currency = currencyId.Find();

            var cell = new CustomViewCell { Text = currency.Code, Detail = currency.Name };

            var delete = new CustomCellViewActionItem
            {
                Icon = "delete.png",
                Data = cell,
                Action = (sender, e) => UiUtils.Edit.RemoveReferenceCurrency(currencyId)
            };
            var star = new CustomCellViewActionItem
            {
                Icon = ApplicationSettings.MainCurrencies.Contains(currencyId) ? "starFilled.png" : "star.png",
                Data = cell,
                Action = (sender, e) => UiUtils.Edit.ToggleReferenceCurrencyStar(currencyId)
            };
            var items = new List<CustomCellViewActionItem> { star, delete };

            cell.ActionItems = items;
            return cell;
        }
    }
}
