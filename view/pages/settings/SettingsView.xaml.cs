using data.settings;
using message;
using MyCryptos.models;
using System;
using Xamarin.Forms;
using MyCryptos.view.components;
using MyCryptos.view.pages.settings;
using MyCryptos.resources;
using enums;
using System.Collections.Generic;
using MyCryptos.view.components.cells;
using MyCryptos.view.overlays;

namespace view
{
    public partial class SettingsView : ContentPage
    {
        public SettingsView()
        {
            InitializeComponent();

            setSortCellText();
            setReferenceCurrencyCells();

            AutoRefresh.On = ApplicationSettings.AutoRefreshOnStartup;

            AutoRefresh.Switch.Toggled += AutoRefreshChanged;
            SortingCell.Tapped += (sender, e) => Navigation.PushAsync(new SortSettingsView());

            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedSortOrder, (str) => setSortCellText());
        }

        private void setReferenceCurrencyCells()
        {
            CurrencySection.Clear();

            var referenceCurrencies = new List<Currency>(ApplicationSettings.ReferenceCurrencies);

            var i = 0;
            foreach (var c in referenceCurrencies)
            {
                var up = new CustomViewCellActionItem { Icon = "up.png", Data = c };
                var down = new CustomViewCellActionItem { Icon = "down.png", Data = c };
                var delete = new CustomViewCellActionItem { Icon = "delete.png", Data = c };
                var items = new List<CustomViewCellActionItem> { up, down, delete };

                Action applySettings = () =>
                {
                    ApplicationSettings.BaseCurrency = referenceCurrencies[0];
                    ApplicationSettings.ReferenceCurrencies = referenceCurrencies;
                    setReferenceCurrencyCells();
                };

                Action<Currency, int> move = (currency, relativePosition) =>
                {
                    var index = referenceCurrencies.IndexOf(currency);
                    referenceCurrencies.RemoveAt(index);
                    referenceCurrencies.Insert(index + relativePosition, currency);
                    applySettings();
                };

                delete.Action = (sender, e) =>
                {
                    referenceCurrencies.Remove((e as TappedEventArgs).Parameter as Currency);
                    applySettings();
                };

                up.Action = (sender, e) =>
                {
                    var currency = (e as TappedEventArgs).Parameter as Currency;
                    move(currency, -1);
                };

                down.Action = (sender, e) =>
                {
                    var currency = (e as TappedEventArgs).Parameter as Currency;
                    move(currency, 1);
                };

                if (i == 0)
                {
                    items.Remove(up);
                }
                if (i == referenceCurrencies.Count - 1)
                {
                    items.Remove(down);
                }
                if (referenceCurrencies.Count <= 1)
                {
                    items.Remove(delete);
                }

                var cell = new CustomViewCell { Text = c.Code, ActionItems = items };
                cell.Detail = (c.Equals(ApplicationSettings.BaseCurrency)) ? InternationalisationResources.PrimaryReferenceCurrency : InternationalisationResources.AdditionalReferenceCurrency;

                CurrencySection.Add(cell);

                i += 1;
            }

            var addCurrencyCell = new CustomViewCell { Text = InternationalisationResources.AddReferenceCurrency, IsActionCell = true };
            addCurrencyCell.Tapped += (sender, e) =>
            {
                var overlay = new CurrencyOverlay();
                overlay.CurrencySelected = (c) =>
                {
                    referenceCurrencies.Add(c);
                    if (referenceCurrencies.Count == 1)
                    {
                        ApplicationSettings.BaseCurrency = c;
                    }
                    ApplicationSettings.ReferenceCurrencies = referenceCurrencies;
                    setReferenceCurrencyCells();
                };

                Navigation.PushAsync(overlay);
            };
            CurrencySection.Add(addCurrencyCell);
        }

        private void setSortCellText()
        {
            var order = string.Empty;
            switch (ApplicationSettings.SortOrder)
            {
                case SortOrder.ALPHABETICAL: order = InternationalisationResources.Alphabetical; break;
                case SortOrder.BY_UNITS: order = InternationalisationResources.ByUnits; break;
                case SortOrder.BY_VALUE: order = InternationalisationResources.ByValue; break;
            }
            var direction = string.Empty;
            switch (ApplicationSettings.SortDirection)
            {
                case SortDirection.ASCENDING: direction = InternationalisationResources.Ascending; break;
                case SortDirection.DESCENDING: direction = InternationalisationResources.Descending; break;
            }
            SortingCell.Detail = string.Format("{0} ({1})", order, direction);
        }

        void ReferenceCurrencyEntered(Currency currency)
        {
            ApplicationSettings.BaseCurrency = currency;
        }

        void AutoRefreshChanged(object sender, EventArgs e)
        {
            if (AutoRefresh != null)
            {
                ApplicationSettings.AutoRefreshOnStartup = AutoRefresh.On;
            }
        }
    }
}

