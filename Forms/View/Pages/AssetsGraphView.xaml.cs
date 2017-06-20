using System;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components;
using MyCC.Forms.View.Overlays;
using Plugin.Connectivity;
using Refractored.XamForms.PullToRefresh;
using Xamarin.Forms;
using static MyCC.Forms.App;

namespace MyCC.Forms.View.Pages
{
    public partial class AssetsGraphView
    {
        private CoinGraphComponent _graphView;
        private PullToRefreshLayout _pullToRefresh;


        public AssetsGraphView()
        {
            InitializeComponent();

            InitPullToRefresh();

            var button = new Button { Text = I18N.AddSource, BorderColor = AppConstants.BorderColor, BackgroundColor = Color.White, BorderRadius = 0, TextColor = AppConstants.ThemeColor, FontAttributes = FontAttributes.None };
            button.Clicked += AddSource;
            NoDataStack.Children.Add(button);

            AddSubscriber();

            SetHeaderCarousel();

            if (ApplicationSettings.DataLoaded)
            {
                SetNoSourcesView();
            }
        }

        private void InitPullToRefresh()
        {
            _graphView = new CoinGraphComponent(Navigation);

            _pullToRefresh = new PullToRefreshLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Content = new ScrollView { VerticalOptions = LayoutOptions.FillAndExpand, Content = _graphView },
                BackgroundColor = AppConstants.TableBackgroundColor,
                RefreshCommand = new Command(Refresh),
            };

            ContentView.Content = _pullToRefresh;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!Device.RuntimePlatform.Equals(Device.Android)) return;
            InitPullToRefresh();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            _graphView.HeightRequest = _pullToRefresh.Height;

            if (!Device.RuntimePlatform.Equals(Device.Android)) return;
            InitPullToRefresh();
        }

        private void PositionSelected(object sender, EventArgs e)
        {
            var currencies = ApplicationSettings.MainCurrencies.ToList();

            ApplicationSettings.StartupCurrencyAssets = currencies[HeaderCarousel.Position];
            MessagingCenter.Send(MessageInfo.ValueChanged, Messaging.ReferenceCurrency);
        }

        private void SetHeaderCarousel()
        {
            HeaderCarousel.ItemsSource = ApplicationSettings.MainCurrencies.Select(CurrencyHelper.Find).ToList();
            HeaderCarousel.Position = ApplicationSettings.MainCurrencies.ToList().IndexOf(ApplicationSettings.StartupCurrencyAssets);
            HeaderCarousel.ShowIndicators = HeaderCarousel.ItemsSource.Count > 1;

            if (HeaderCarousel.ItemTemplate != null) return;

            HeaderCarousel.ItemTemplate = new HeaderTemplateSelector();
            HeaderCarousel.PositionSelected += PositionSelected;
            HeaderCarousel.HeightRequest = 100;
            HeaderCarousel.WidthRequest = ScreenHeight / 3.0;
        }

        private void SetNoSourcesView()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                NoDataView.IsVisible = AccountStorage.CurrenciesForGraph == 0;
                DataView.IsVisible = AccountStorage.CurrenciesForGraph != 0;
            });

            SetFooter();
        }

        private void SetFooter()
        {
            var online = AccountStorage.Instance.AllElements.Where(a => a is OnlineFunctionalAccount).ToList();
            var accountsTime = online.Any() ? online.Min(a => a.LastUpdate) : AccountStorage.Instance.AllElements.Any() ? AccountStorage.Instance.AllElements.Max(a => a.LastUpdate) : DateTime.Now;
            var ratesTime = AccountStorage.NeededRatesFor(ApplicationSettings.StartupCurrencyAssets.ToCurrency()).Distinct().Select(e => RateUtil.GetRate(e)?.LastUpdate ?? DateTime.Now).DefaultIfEmpty(DateTime.Now).Min();

            var time = online.Count > 0 ? ratesTime < accountsTime ? ratesTime : accountsTime : ratesTime;

            Device.BeginInvokeOnMainThread(() => Footer.Text = time.LastUpdateString());
        }

        private void AddSubscriber()
        {
            Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => HeaderCarousel.Position = ApplicationSettings.MainCurrencies.ToList().IndexOf(ApplicationSettings.StartupCurrencyAssets));
            Messaging.ReferenceCurrencies.SubscribeValueChanged(this, SetHeaderCarousel);

            Messaging.Loading.SubscribeFinished(this, SetNoSourcesView);
            Messaging.FetchMissingRates.SubscribeFinished(this, SetNoSourcesView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, SetNoSourcesView);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, SetNoSourcesView);
        }

        private async void Refresh()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                await AppTaskHelper.FetchBalancesAndRates();
                _pullToRefresh.IsRefreshing = false;
            }
            else
            {
                _pullToRefresh.IsRefreshing = false;
                await DisplayAlert(I18N.NoInternetAccess, I18N.ErrorRefreshingNotPossibleWithoutInternet, I18N.Cancel);
            }
        }

        private void AddSource(object sender, EventArgs e)
        {
            Navigation.PushOrPushModal(new AddSourceOverlay());
        }

        private class HeaderTemplateSelector : DataTemplateSelector
        {
            protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new DataTemplate(() => new CoinHeaderComponent((Currency)item));
        }
    }
}
