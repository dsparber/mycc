using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currency.Model;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;
using Xamarin.Forms;

namespace MyCC.Forms.View.Overlays
{
    internal class CurrencyOverlay : ContentPage
    {
        public Action<Currency> CurrencySelected;

        private readonly SearchBar _searchBar;

        private readonly bool _isModal;
        private readonly bool _viewOnly;

        public CurrencyOverlay(IEnumerable<Currency> currenciesToSelect, string title, bool isModal = false, bool viewOnly = false)
        {
            _isModal = isModal;
            _viewOnly = viewOnly;
            var selectableCurrencies = currenciesToSelect.Distinct().Where(c => c != null).OrderBy(c => c.Code).ToList();

            Title = title;

            if (_isModal)
            {
                var cancel = new ToolbarItem { Text = I18N.Cancel };
                cancel.Clicked += (sender, e) => Navigation.PopOrPopModal();
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

            SetTableContent(section, selectableCurrencies);

            _searchBar.TextChanged += (sender, e) =>
            {
                var filtered = !string.IsNullOrWhiteSpace(e.NewTextValue) ? selectableCurrencies.Where(c => c.Code.ToLower().Contains(e.NewTextValue.ToLower()) || c.Name.ToLower().Contains(e.NewTextValue.ToLower())) : selectableCurrencies;
                SetTableContent(section, filtered);
            };

            currenciesTableView.Root.Add(section);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _searchBar.Focus();
        }

        private void SetTableContent(TableSection section, IEnumerable<Currency> currenciesSorted)
        {
            section.Clear();
            var items = currenciesSorted.Select(c =>
            {
                var cell = new CustomViewCell { Text = c.Code, Detail = c.Name };

                if (_viewOnly) return cell;

                cell.Tapped += (sender, e) =>
                {
                    CurrencySelected?.Invoke(c);
                    if (_isModal) Navigation.PopOrPopModal();
                    else Navigation.PopAsync();
                };
                return cell;
            });
            section.Add(items);
        }

        public static void ShowAddRateOverlay(INavigation navigation, Action onComplete = null)
        {
            var allReferenceCurrencies = ApplicationSettings.WatchedCurrencies.ToArray();
            var currencies = CurrencyStorage.Instance.AllElements.Where(c => !allReferenceCurrencies.Contains(c)).ToList();

            var overlay = new CurrencyOverlay(currencies, I18N.AddRate, true)
            {
                CurrencySelected = c =>
                {
                    onComplete?.Invoke();
                    ApplicationSettings.WatchedCurrencies = new List<Currency>(ApplicationSettings.WatchedCurrencies) { c };
                    Messaging.ReferenceCurrencies.SendValueChanged();
                    Messaging.UpdatingRates.SendFinished();
                }
            };

            navigation.PushOrPushModal(overlay);
        }
    }
}
