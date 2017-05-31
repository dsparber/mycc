using System;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components.Header;
using MyCC.Forms.View.Components.Table;
using MyCC.Forms.View.Overlays;
using Plugin.Connectivity;
using Refractored.XamForms.PullToRefresh;
using Xamarin.Forms;
using static MyCC.Forms.App;

namespace MyCC.Forms.View.Pages
{
    public partial class RateView
    {
        private PullToRefreshLayout _pullToRefresh;
        private RatesTableComponent _tableView;

        private bool _firstCall = true;

        public RateView()
        {
            InitializeComponent();

            InitPullToRefresh();

            AddSubscriber();

            SetHeaderCarousel();

            SetFooterText();

            var button = new Button { Text = I18N.AddSource, BorderColor = AppConstants.BorderColor, BackgroundColor = Color.White, BorderRadius = 0, TextColor = AppConstants.ThemeColor, FontAttributes = FontAttributes.None };
            button.Clicked += AddRate;
            NoDataStack.Children.Add(button);

            if (ApplicationSettings.DataLoaded)
            {
                SetNoData();
            }
        }

        private void InitPullToRefresh()
        {
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

            ContentView.Content = _pullToRefresh;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!_firstCall) return;

            _firstCall = false;
            InitPullToRefresh();
            await Task.Delay(1000);
            Device.BeginInvokeOnMainThread(InitPullToRefresh);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (!Device.RuntimePlatform.Equals(Device.Android)) return;
            InitPullToRefresh();
        }

        private void PositionSelected(object sender, EventArgs e)
        {
            var currencies = ApplicationSettings.MainCurrencies.ToList();

            ApplicationSettings.StartupCurrencyRates = currencies[HeaderCarousel.Position];
            Messaging.RatesPageCurrency.SendValueChanged();
        }

        private void SetHeaderCarousel()
        {
            HeaderCarousel.ItemsSource = ApplicationSettings.MainCurrencies.Select(CurrencyHelper.Find).ToList();
            HeaderCarousel.Position = ApplicationSettings.MainCurrencies.ToList().IndexOf(ApplicationSettings.StartupCurrencyRates);
            HeaderCarousel.ShowIndicators = HeaderCarousel.ItemsSource.Count > 1;
            HeaderCarousel.PageIndicatorTintColor = Color.FromHex("#5FFF");

            if (HeaderCarousel.ItemTemplate != null) return;

            HeaderCarousel.ItemTemplate = new HeaderTemplateSelector();
            HeaderCarousel.PositionSelected += PositionSelected;
            HeaderCarousel.HeightRequest = 100;
            HeaderCarousel.WidthRequest = ScreenHeight / 3.0;
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
            if (CrossConnectivity.Current.IsConnected)
            {
                await AppTaskHelper.UpdateRates();
                _pullToRefresh.IsRefreshing = false;
            }
            else
            {
                _pullToRefresh.IsRefreshing = false;
                await DisplayAlert(I18N.NoInternetAccess, I18N.ErrorRefreshingNotPossibleWithoutInternet, I18N.Cancel);
            }
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
                            .Distinct()
                            .Select(e => new ExchangeRate(e, ApplicationSettings.StartupCurrencyRates))
                            .Select(e => ExchangeRateHelper.GetRate(e)?.LastUpdate ?? DateTime.Now).DefaultIfEmpty(DateTime.Now).Min().LastUpdateString();

            Device.BeginInvokeOnMainThread(() => Footer.Text = text);
        }

        private void SetNoData()
        {
            var anyItems = ApplicationSettings.WatchedCurrencies
                .Concat(ApplicationSettings.AllReferenceCurrencies)
                .Concat(AccountStorage.UsedCurrencies)
                .Distinct()
                .Any(c => !c.Equals(ApplicationSettings.StartupCurrencyRates));

            Device.BeginInvokeOnMainThread(() =>
            {
                NoDataView.IsVisible = !anyItems;
                DataView.IsVisible = anyItems;
            });
        }
    }
}