using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currency.Model;
using MyCC.Core.Currency.Storage;
using MyCC.Forms.Constants;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;
using Xamarin.Forms;

namespace MyCC.Forms.View.Overlays
{
    internal class CurrencyOverlay : ContentPage
    {
        public Action<Currency> CurrencySelected;

        private readonly SearchBar _searchBar;
        private readonly CurrencyEntryCell _parent;

        public CurrencyOverlay(List<Currency> currenciesToSelect = null) : this(null, currenciesToSelect)
        { }

        public CurrencyOverlay(CurrencyEntryCell parent, List<Currency> currenciesToSelect = null)
        {
            _parent = parent;

            var currencies = currenciesToSelect ?? _parent?.CurrenciesToSelect?.OrderBy(c => c?.Code).ToList() ?? CurrencyStorage.Instance.AllElements;

            Title = I18N.Currency;

            if (_parent != null)
            {
                var done = new ToolbarItem { Text = I18N.Save };
                done.Clicked += (sender, e) =>
                {
                    _parent.OnSelected(_parent.SelectedCurrency);
                    Navigation.PopAsync();
                };
                ToolbarItems.Add(done);
            }
            else
            {
                var cancel = new ToolbarItem { Text = I18N.Cancel };
                cancel.Clicked += (sender, e) => Navigation.PopAsync();
                ToolbarItems.Add(cancel);
            }

            _searchBar = new SearchBar { Placeholder = I18N.SearchCurrencies };
            if (Device.OS == TargetPlatform.Android)
            {
                _searchBar.TextColor = AppConstants.FontColor;
                _searchBar.PlaceholderColor = AppConstants.FontColorLight;
                _searchBar.HeightRequest = _searchBar.HeightRequest + 50;
                _searchBar.Margin = new Thickness(0, 0, 0, -51);
            }

            var currenciesTableView = new TableView();

            var stack = new StackLayout();
            stack.Children.Add(_searchBar);
            stack.Children.Add(currenciesTableView);

            Content = stack;

            var section = new TableSection();

            var currenciesSorted = currencies.Where(c => c != null).Distinct().OrderBy(c => c.Code);
            SetTableContent(section, currenciesSorted);

            _searchBar.TextChanged += (sender, e) =>
            {
                var txt = e.NewTextValue ?? string.Empty;
                var filtered = currenciesSorted.Where(c => c.Code.ToLower().Contains(txt.ToLower()) || c.Name.ToLower().Contains(txt.ToLower()));
                SetTableContent(section, filtered);
            };

            currenciesTableView.Root.Add(section);

            currenciesTableView.IsVisible = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _searchBar.Focus();
        }

        private void SetTableContent(TableSection section, IEnumerable<Currency> currenciesSorted)
        {
            section.Clear();
            foreach (var c in currenciesSorted)
            {
                var cell = new CustomViewCell { Text = c.Code, Detail = c.Name };
                cell.Tapped += (sender, e) =>
                {
                    if (_parent != null)
                    {
                        _parent.SelectedCurrency = c;
                        _parent.OnSelected(_parent.SelectedCurrency);
                    }
                    CurrencySelected?.Invoke(c);
                    Navigation.PopAsync();
                };
                section.Add(cell);
            }
        }
    }
}
