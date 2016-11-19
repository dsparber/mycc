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
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => setReferenceCurrencyCells());
        }

        private void setReferenceCurrencyCells()
        {
            CurrencySection.Clear();

            var referenceCurrencies = new List<Currency>(ApplicationSettings.ReferenceCurrencies);

            foreach (var c in referenceCurrencies)
            {
                var delete = new CustomViewCellActionItem { Icon = "delete.png", Data = c };
                var items = new List<CustomViewCellActionItem> { delete };

                delete.Action = (sender, e) =>
                {
                    var cu = (e as TappedEventArgs).Parameter as Currency;
                    referenceCurrencies.Remove(cu);
                    if (cu.Equals(ApplicationSettings.BaseCurrency))
                    {
                        ApplicationSettings.BaseCurrency = referenceCurrencies[0];
                    }
                    ApplicationSettings.ReferenceCurrencies = referenceCurrencies;
                    setReferenceCurrencyCells();
                };

                if (referenceCurrencies.Count <= 1)
                {
                    items.Remove(delete);
                }

                var cell = new CustomViewCell { Text = c.Code, ActionItems = items };
                if (c.Equals(ApplicationSettings.BaseCurrency))
                {
                    cell.Detail = InternationalisationResources.PrimaryReferenceCurrency;
                }else
                {
                    cell.Detail = "-";
                }

                cell.Tapped += (sender, e) =>
                {
                    ApplicationSettings.BaseCurrency = referenceCurrencies.Find(x => x.Code.Equals((sender as CustomViewCell).Text));
                    setReferenceCurrencyCells();
                };

                CurrencySection.Add(cell);
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

