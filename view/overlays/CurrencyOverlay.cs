using constants;
using data.storage;
using MyCryptos.models;
using MyCryptos.resources;
using MyCryptos.view.components;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace MyCryptos.view.overlays
{
    class CurrencyOverlay : ContentPage
    {
        public Action<Currency> CurrencySelected;

        readonly SearchBar searchBar;
        readonly TableView currenciesTableView;
        readonly CurrencyEntryCell parent;

        List<Currency> currencies;

        public CurrencyOverlay() : this(null)
        { }

        public CurrencyOverlay(CurrencyEntryCell p)
        {
            parent = p;

            var type = parent?.CurrencyRepositoryType;

            var repos = (type != null) ? CurrencyStorage.Instance.RepositoriesOfType(type) : CurrencyStorage.Instance.Repositories;
            var allElements = CurrencyStorage.Instance.AllElements;

            if (type != null)
            {
                var ids = repos.Select(e => e.Id);
                var currencyMapCodes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => ids.Contains(e.RepositoryId)).Select(e => e.Code);
                currencies = allElements.Where(e => currencyMapCodes.Contains(e.Code)).ToList();
            }
            else
            {
                currencies = allElements;
            }

            Title = I18N.Currency;

            if (parent != null)
            {
                var done = new ToolbarItem { Text = I18N.Save };
                done.Clicked += (sender, e) =>
                {
                    parent.OnSelected(parent.SelectedCurrency);
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

            searchBar = new SearchBar { Placeholder = I18N.SearchCurrencies };
            if (Device.OS == TargetPlatform.Android)
            {
                searchBar.TextColor = AppConstants.FontColor;
                searchBar.PlaceholderColor = AppConstants.FontColorLight;
                searchBar.HeightRequest = searchBar.HeightRequest + 50;
                searchBar.Margin = new Thickness(0, 0, 0, -51);
            }

            currenciesTableView = new TableView();

            var stack = new StackLayout();
            stack.Children.Add(searchBar);
            stack.Children.Add(currenciesTableView);

            Content = stack;

            var section = new TableSection();

            var currenciesSorted = currencies.Distinct().OrderBy(c => c.Code);
            setTableContent(section, currenciesSorted);

            searchBar.TextChanged += (sender, e) =>
            {
                var txt = e.NewTextValue;
                var filtered = currenciesSorted.Where(c => c.Code.ToLower().Contains(txt.ToLower()) || c.Name.ToLower().Contains(txt.ToLower()));
                setTableContent(section, filtered);
            };

            currenciesTableView.Root.Add(section);

            currenciesTableView.IsVisible = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            searchBar.Focus();
        }

        void setTableContent(TableSection section, IEnumerable<Currency> currenciesSorted)
        {
            section.Clear();
            foreach (var c in currenciesSorted)
            {
                var cell = new CustomViewCell { Text = c.Code, Detail = c.Name };
                cell.Tapped += (sender, e) =>
                {
                    if (parent != null)
                    {
                        parent.SelectedCurrency = c;
                        parent.OnSelected(parent.SelectedCurrency);
                    }
                    CurrencySelected?.Invoke(c);
                    Navigation.PopAsync();
                };
                section.Add(cell);
            }
        }
    }
}
