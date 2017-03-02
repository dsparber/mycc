using System;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components.CellViews;
using MyCC.Forms.View.Components;
using MyCC.Forms.View.Overlays;
using Refractored.XamForms.PullToRefresh;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages
{
    public partial class RateView
    {
        private readonly PullToRefreshLayout _pullToRefresh;
        private readonly RatesTableComponent _tableView;

        public RateView()
        {
            InitializeComponent();

            _tableView = new RatesTableComponent(Navigation);

            var stack = new StackLayout { Spacing = 0, VerticalOptions = LayoutOptions.FillAndExpand };
            stack.Children.Add(_tableView);
            stack.Children.Add(new ContentView { VerticalOptions = LayoutOptions.FillAndExpand });

            _pullToRefresh = new PullToRefreshLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Content = new ScrollView { Content = stack, VerticalOptions = LayoutOptions.FillAndExpand },
                BackgroundColor = AppConstants.TableBackgroundColor,
                RefreshCommand = new Command(Refresh),
            };

            Content.Content = _pullToRefresh;

            AddSubscriber();

            SetHeaderCarousel();

            SetFooterText();

            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += AddRate;
            var addCell = new CustomCellView(true) { Text = I18N.AddRate, IsActionCell = true, IsCentered = true };
            addCell.GestureRecognizers.Add(recognizer);
            NoDataStack.Children.Add(addCell);

            if (ApplicationSettings.FirstLaunch)
            {
                Task.Run(async () => await AppTaskHelper.FetchMissingRates(ApplicationSettings.WatchedCurrencies
                                       .Concat(ApplicationSettings.AllReferenceCurrencies)
                                       .Select(c => new ExchangeRate(Currency.Btc, c))
                                       .Select(r => ExchangeRateHelper.GetRate(r) ?? r)
                                       .Where(r => r.Rate == null)
                                       .Concat(AccountStorage.NeededRates).ToList()));
            }

            if (ApplicationSettings.DataLoaded)
            {
                SetNoData();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _tableView.OnAppearing();
        }

        private void PositionSelected(object sender, EventArgs e)
        {
            var currencies = ApplicationSettings.MainCurrencies;

            ApplicationSettings.SelectedRatePageCurrency = currencies[HeaderCarousel.Position];
            Messaging.RatesPageCurrency.SendValueChanged();
        }

        private void SetHeaderCarousel()
        {
            HeaderCarousel.ItemsSource = ApplicationSettings.MainCurrencies.ToList();
            HeaderCarousel.Position = ApplicationSettings.MainCurrencies.IndexOf(ApplicationSettings.SelectedRatePageCurrency);
            HeaderCarousel.ShowIndicators = HeaderCarousel.ItemsSource.Count > 1;

            if (HeaderCarousel.ItemTemplate != null) return;

            HeaderCarousel.ItemTemplate = new HeaderTemplateSelector();
            HeaderCarousel.PositionSelected += PositionSelected;
            HeaderCarousel.HeightRequest = 100;
        }

        private void AddSubscriber()
        {
            Messaging.ReferenceCurrencies.SubscribeValueChanged(this, SetHeaderCarousel);
            Messaging.UpdatingRates.SubscribeFinished(this, () => { SetFooterText(); SetNoData(); });

            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, SetNoData);
            Messaging.UpdatingAccounts.SubscribeFinished(this, SetNoData);
            Messaging.Loading.SubscribeFinished(this, SetNoData);
        }

        private async void Refresh()
        {
            await AppTaskHelper.UpdateRates();
            _pullToRefresh.IsRefreshing = false;
        }

        private void AddRate(object sender, EventArgs e) => CurrencyOverlay.ShowAddRateOverlay(Navigation);

        private class HeaderTemplateSelector : DataTemplateSelector
        {
            protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new DataTemplate(() =>
             {
                 var c = (Currency)item;

                 return new RatesHeaderComponent(c);
             });
        }

        private void SetFooterText()
        {
            var text = ApplicationSettings.WatchedCurrencies
                            .Concat(ApplicationSettings.AllReferenceCurrencies)
                            .Concat(AccountStorage.UsedCurrencies)
                            .Select(e => new ExchangeRate(ApplicationSettings.SelectedRatePageCurrency, e))
                            .SelectMany(ExchangeRateHelper.GetNeededRates)
                            .Distinct()
                            .Select(e => ExchangeRateHelper.GetRate(e)?.LastUpdate ?? DateTime.Now).DefaultIfEmpty(DateTime.Now).Min().LastUpdateString();

            Device.BeginInvokeOnMainThread(() => Footer.Text = text);
        }

        private void SetNoData()
        {
            var anyItems = ApplicationSettings.WatchedCurrencies
                .Concat(ApplicationSettings.AllReferenceCurrencies)
                .Concat(AccountStorage.UsedCurrencies)
                .Distinct()
                .Any(c => !c.Equals(ApplicationSettings.SelectedRatePageCurrency));

            Device.BeginInvokeOnMainThread(() =>
            {
                NoDataView.IsVisible = !anyItems;
                DataView.IsVisible = anyItems;
            });
        }
    }
}