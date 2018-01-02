using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;
using MyCC.Ui;
using Xamarin.Forms;

namespace MyCC.Forms.View.Overlays
{
    internal class CurrencyOverlay : ContentPage
    {
        public Action<Currency> CurrencySelected;

        private readonly SearchBar _searchBar;
        private readonly ActivityIndicator _activityIndicator;
        private readonly StackLayout _activityView;
        private readonly Func<IEnumerable<Currency>> _currenciesToSelect;
        private readonly TableView _currencyTableView;

        private readonly bool _isModal;
        private readonly bool _viewOnly;

        public CurrencyOverlay(Func<IEnumerable<Currency>> currenciesToSelect, string title, bool isModal = false, bool viewOnly = false)
        {
            _isModal = isModal;
            _viewOnly = viewOnly;
            _currenciesToSelect = currenciesToSelect;

            Title = title;

            if (_isModal)
            {
                var cancel = new ToolbarItem { Text = I18N.Cancel };
                cancel.Clicked += (sender, e) => Navigation.PopOrPopModal();
                ToolbarItems.Add(cancel);
            }

            _searchBar = new SearchBar { Placeholder = I18N.SearchCurrencies };
            if (Device.RuntimePlatform.Equals(Device.Android))
            {
                _searchBar.TextColor = AppConstants.FontColor;
                _searchBar.PlaceholderColor = AppConstants.FontColorLight;
                _searchBar.HeightRequest = 50;
            }

            _activityIndicator = new ActivityIndicator
            {
                IsRunning = true,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };

            _activityView = new StackLayout
            {
                Children = {
                    _activityIndicator,
                    new Label {
                        Text = I18N.LoadingCurrencies,
                        TextColor = AppConstants.FontColorLight,
                        FontSize = AppConstants.AndroidFontSize,
                        HorizontalOptions = LayoutOptions.CenterAndExpand
                    }
                },
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            _currencyTableView = new TableView { IsVisible = false };

            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(_searchBar);
            stack.Children.Add(_activityView);
            stack.Children.Add(_currencyTableView);

            Content = stack;


        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var section = new TableSection();

            Task.Run(() =>
            {
                var currencies = _currenciesToSelect != null ? _currenciesToSelect() : CurrencyStorage.Instance.Currencies;
                var selectableCurrencies = currencies.Distinct().Where(c => c != null).OrderBy(c => c.Code).ToList();

                SetTableContent(section, selectableCurrencies);
                Device.BeginInvokeOnMainThread(() =>
                {
                    _currencyTableView.IsVisible = true;
                    _activityIndicator.IsRunning = false;
                    _activityView.IsVisible = false;
                    _searchBar.Focus();
                });

                _searchBar.TextChanged += (sender, e) =>
                {
                    var search = (e.NewTextValue ?? string.Empty).ToLower();
                    var filtered = !string.IsNullOrWhiteSpace(e.NewTextValue) ? selectableCurrencies.AsParallel().Where(c => c.Code.ToLower().Contains(search) || c.Name.ToLower().Contains(search)) as IEnumerable<Currency> : selectableCurrencies;
                    SetTableContent(section, filtered);
                };
                _currencyTableView.Root.Add(section);
            });
        }

        private void SetTableContent(TableSection section, IEnumerable<Currency> currenciesSorted)
        {
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
            }).ToList();
            Device.BeginInvokeOnMainThread(() =>
            {
                section.Clear();
                section.Add(items);
            });
        }

        public static void ShowAddRateOverlay(INavigation navigation)
        {
            var task = new Func<IEnumerable<Currency>>(() =>
            {
                var exceptions = UiUtils.Get.Rates.EnabledCurrencyIds;
                return CurrencyStorage.Instance.Currencies.Where(c => !exceptions.Contains(c.Id));
            });

            var overlay = new CurrencyOverlay(task, I18N.AddRate, true)
            {
                CurrencySelected = c => UiUtils.Edit.AddWatchedCurrency(c.Id)
            };

            navigation.PushOrPushModal(overlay);
        }
    }
}
