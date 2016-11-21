using System.Collections.Generic;
using data.settings;
using message;
using MyCryptos.models;
using MyCryptos.resources;
using MyCryptos.view.components;
using MyCryptos.view.components.cells;
using MyCryptos.view.overlays;
using Xamarin.Forms;

namespace MyCryptos.view.pages.settings
{
    public partial class ReferenceCurrenciesSettingsView
    {
        public ReferenceCurrenciesSettingsView()
        {
            InitializeComponent();
            SetReferenceCurrencyCells();

            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => SetReferenceCurrencyCells());
        }

        private void SetReferenceCurrencyCells()
        {
            CurrencySection.Clear();

            var referenceCurrencies = new List<Currency>(ApplicationSettings.ReferenceCurrencies);

            foreach (var c in referenceCurrencies)
            {
                var delete = new CustomViewCellActionItem { Icon = "delete.png", Data = c };
                var items = new List<CustomViewCellActionItem> { delete };

                delete.Action = (sender, e) =>
                {
                    var cu = (e as TappedEventArgs)?.Parameter as Currency;
                    referenceCurrencies.Remove(cu);
                    if (ApplicationSettings.BaseCurrency.Equals(cu))
                    {
                        ApplicationSettings.BaseCurrency = referenceCurrencies[0];
                    }
                    ApplicationSettings.ReferenceCurrencies = referenceCurrencies;
                    SetReferenceCurrencyCells();
                };

                if (referenceCurrencies.Count <= 1)
                {
                    items.Remove(delete);
                }

                var cell = new CustomViewCell
                {
                    Text = c.Code,
                    ActionItems = items,
                    Detail = c.Equals(ApplicationSettings.BaseCurrency) ? I18N.PrimaryReferenceCurrency : "-"
                };

                cell.Tapped += (sender, e) =>
                {
                    ApplicationSettings.BaseCurrency = referenceCurrencies.Find(x => x.Code.Equals((sender as CustomViewCell)?.Text));
                    SetReferenceCurrencyCells();
                };

                CurrencySection.Add(cell);
            }

            var addCurrencyCell = new CustomViewCell { Text = I18N.AddReferenceCurrency, IsActionCell = true };
            addCurrencyCell.Tapped += (sender, e) =>
            {
                var overlay = new CurrencyOverlay
                {
                    CurrencySelected = (c) =>
                    {
                        referenceCurrencies.Add(c);
                        if (referenceCurrencies.Count == 1)
                        {
                            ApplicationSettings.BaseCurrency = c;
                        }
                        ApplicationSettings.ReferenceCurrencies = referenceCurrencies;
                        SetReferenceCurrencyCells();
                    }
                };

                Navigation.PushAsync(overlay);
            };
            CurrencySection.Add(addCurrencyCell);
        }
    }
}
