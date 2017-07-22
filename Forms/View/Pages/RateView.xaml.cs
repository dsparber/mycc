using System;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Header;
using MyCC.Forms.View.Components.Table;
using MyCC.Forms.View.Overlays;
using MyCC.Ui;
using MyCC.Ui.Messages;
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

            var button = new Button { Text = I18N.AddSource, BorderColor = AppConstants.BorderColor, BackgroundColor = Color.White, BorderRadius = 0, TextColor = AppConstants.ThemeColor, FontAttributes = FontAttributes.None };
            button.Clicked += AddRate;
            NoDataStack.Children.Add(button);

            if (ApplicationSettings.DataLoaded)
            {
                SetData();
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

        private string _lastCurrencyId = ApplicationSettings.StartupCurrencyRates;
        private void PositionSelected(object sender, EventArgs e)
        {
            var currencies = ApplicationSettings.MainCurrencies.ToList();
            var position = HeaderCarousel.Position < currencies.Count || HeaderCarousel.Position > 0 ? HeaderCarousel.Position : currencies.Count - 1;

            ApplicationSettings.StartupCurrencyRates = currencies[position];
            if (_lastCurrencyId.Equals(ApplicationSettings.StartupCurrencyRates)) return;

            Messaging.Status.CarouselPosition.Send();
            _lastCurrencyId = ApplicationSettings.StartupCurrencyRates;
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
            Messaging.Status.CarouselPosition.Subscribe(this, () => HeaderCarousel.Position = ApplicationSettings.MainCurrencies.ToList().IndexOf(ApplicationSettings.StartupCurrencyRates));
            Messaging.Status.Progress.SubscribeFinished(this, () => Device.BeginInvokeOnMainThread(() => _pullToRefresh.IsRefreshing = false));
            Messaging.Update.Rates.Subscribe(this, SetData);
        }

        private void Refresh()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                UiUtils.Update.FetchAllRates();
            }
            else
            {
                _pullToRefresh.IsRefreshing = false;
                DisplayAlert(I18N.NoInternetAccess, I18N.ErrorRefreshingNotPossibleWithoutInternet, I18N.Cancel);
            }
        }

        private void AddRate(object sender, EventArgs e) => CurrencyOverlay.ShowAddRateOverlay(Navigation);

        private class HeaderTemplateSelector : DataTemplateSelector
        {
            protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new DataTemplate(() =>
            {
                var currencyId = ((Currency)item).Id;
                return new RatesOverviewHeader(currencyId);
            });
        }

        private void SetData()
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
                Footer.Text = UiUtils.Get.Rates.LastUpdate.LastUpdateString();
            });
        }
    }
}