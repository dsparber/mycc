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
    public partial class AssetsTableView
    {
        private CoinTableComponent _tableView;
        private PullToRefreshLayout _pullToRefresh;

        private bool _firstCall = true;

        public AssetsTableView()
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
                UpdateView();
            }
        }

        private void InitPullToRefresh()
        {
            _tableView = new CoinTableComponent();

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


        private string _lastCurrencyId = ApplicationSettings.StartupCurrencyAssets;
        private void PositionSelected(object sender, EventArgs e)
        {
            var currencies = ApplicationSettings.MainCurrencies.ToList();

            ApplicationSettings.StartupCurrencyAssets = currencies[HeaderCarousel.Position];
            if (_lastCurrencyId.Equals(ApplicationSettings.StartupCurrencyAssets)) return;

            _lastCurrencyId = ApplicationSettings.StartupCurrencyAssets;
            Messaging.Status.CarouselPosition.Send();
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

        private void UpdateView()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                NoDataView.IsVisible = AccountStorage.Instance.AllElements.Count == 0;
                DataView.IsVisible = AccountStorage.Instance.AllElements.Count != 0;
                Footer.Text = UiUtils.Get.Assets.LastUpdate.LastUpdateString();
            });

        }

        private void AddSubscriber()
        {
            Messaging.Status.CarouselPosition.Subscribe(this, () => HeaderCarousel.Position = ApplicationSettings.MainCurrencies.ToList().IndexOf(ApplicationSettings.StartupCurrencyAssets));
            Messaging.Status.Progress.SubscribeFinished(this, () => Device.BeginInvokeOnMainThread(() => _pullToRefresh.IsRefreshing = false));
            Messaging.Update.Rates.Subscribe(this, UpdateView);
            Messaging.Update.Balances.Subscribe(this, UpdateView);
            Messaging.Modified.ReferenceCurrencies.Subscribe(this, SetHeaderCarousel);
        }

        private async void Refresh()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                UiUtils.Update.FetchAllAssetsAndRates();
            }
            else
            {
                _pullToRefresh.IsRefreshing = false;
                await DisplayAlert(I18N.NoInternetAccess, I18N.ErrorRefreshingNotPossibleWithoutInternet, I18N.Cancel);
            }
        }

        private class HeaderTemplateSelector : DataTemplateSelector
        {
            protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new DataTemplate(() =>
            {
                var currencyId = ((Currency)item).Id;
                var header = new HeaderView(true) { Data = UiUtils.Get.Assets.HeaderFor(currencyId) };

                Messaging.Update.Rates.Subscribe(header, () => header.Data = UiUtils.Get.Assets.HeaderFor(currencyId));
                Messaging.Update.Balances.Subscribe(header, () => header.Data = UiUtils.Get.Assets.HeaderFor(currencyId));

                return header;
            });
        }

        private void AddSource(object sender, EventArgs e)
        {
            Navigation.PushOrPushModal(new AddSourceOverlay());
        }
    }
}