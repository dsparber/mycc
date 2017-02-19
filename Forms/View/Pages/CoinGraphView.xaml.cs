using System;
using System.Linq;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Messages;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components;
using Refractored.XamForms.PullToRefresh;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages
{
    public partial class CoinGraphView
    {
        private readonly CoinGraphComponent _graphView;
        private readonly PullToRefreshLayout _pullToRefresh;


        public CoinGraphView()
        {
            InitializeComponent();

            _graphView = new CoinGraphComponent(Navigation)
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            _pullToRefresh = new PullToRefreshLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Content = new ScrollView { VerticalOptions = LayoutOptions.FillAndExpand, Content = _graphView },
                BackgroundColor = AppConstants.TableBackgroundColor,
                RefreshCommand = new Command(Refresh),
            };

            Stack.Children.Add(_pullToRefresh);

            AddSubscriber();

            SetHeaderCarousel();
            SetNoSourcesView();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _graphView.OnAppearing();
        }

        private void PositionSelected(object sender, EventArgs e)
        {
            var currencies = ApplicationSettings.MainCurrencies;

            ApplicationSettings.BaseCurrency = currencies[HeaderCarousel.Position];
            MessagingCenter.Send(MessageInfo.ValueChanged, Messaging.ReferenceCurrency);
        }

        private void SetHeaderCarousel()
        {
            HeaderCarousel.ItemsSource = ApplicationSettings.MainCurrencies.ToList();
            HeaderCarousel.Position = ApplicationSettings.MainCurrencies.IndexOf(ApplicationSettings.BaseCurrency);
            HeaderCarousel.ShowIndicators = HeaderCarousel.ItemsSource.Count > 1;

            if (HeaderCarousel.ItemTemplate != null) return;

            HeaderCarousel.ItemTemplate = new HeaderTemplateSelector();
            HeaderCarousel.PositionSelected += PositionSelected;
            HeaderCarousel.HeightRequest = 100;
        }

        private void SetNoSourcesView()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                NoSourcesView.IsVisible = AccountStorage.Instance.AllElements.Count == 0;
                Stack.IsVisible = AccountStorage.Instance.AllElements.Count != 0;
            });
        }

        private void AddSubscriber()
        {
            Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => HeaderCarousel.Position = ApplicationSettings.MainCurrencies.IndexOf(ApplicationSettings.BaseCurrency));
            Messaging.ReferenceCurrencies.SubscribeValueChanged(this, SetHeaderCarousel);

            Messaging.Loading.SubscribeFinished(this, SetNoSourcesView);
            Messaging.FetchMissingRates.SubscribeFinished(this, SetNoSourcesView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, SetNoSourcesView);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, SetNoSourcesView);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            _graphView.HeightRequest = _pullToRefresh.Height;
        }

        private async void Refresh()
        {
            await AppTaskHelper.FetchBalancesAndRates();
            _pullToRefresh.IsRefreshing = false;
        }

        private class HeaderTemplateSelector : DataTemplateSelector
        {
            protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new DataTemplate(() => new CoinHeaderComponent((Currency)item));
        }
    }
}