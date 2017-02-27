using System;
using System.Linq;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.view.components.CellViews;
using MyCC.Forms.View.Components;
using MyCC.Forms.View.Overlays;
using Refractored.XamForms.PullToRefresh;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages
{
    public partial class CoinTableView
    {
        private readonly CoinTableComponent _tableView;
        private readonly PullToRefreshLayout _pullToRefresh;

        public CoinTableView()
        {
            InitializeComponent();

            _tableView = new CoinTableComponent(Navigation);

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

            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += AddSource;
            var addCell = new CustomCellView(true) { Text = I18N.AddSource, IsActionCell = true, IsCentered = true };
            addCell.GestureRecognizers.Add(recognizer);
            NoDataStack.Children.Add(addCell);

            AddSubscriber();

            SetHeaderCarousel();
            SetNoSourcesView();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _tableView.OnAppearing();
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
                NoDataView.IsVisible = AccountStorage.Instance.AllElements.Count == 0;
                DataView.IsVisible = AccountStorage.Instance.AllElements.Count != 0;
            });

            SetFooter();
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

        private async void Refresh()
        {
            await AppTaskHelper.FetchBalancesAndRates();
            _pullToRefresh.IsRefreshing = false;
        }

        private class HeaderTemplateSelector : DataTemplateSelector
        {
            protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new DataTemplate(() => new CoinHeaderComponent((Currency)item));
        }

        private void AddSource(object sender, EventArgs e)
        {
            Navigation.PushOrPushModal(new AddSourceView());
        }

        private void SetFooter()
        {
            var online = AccountStorage.Instance.AllElements.Where(a => a is OnlineFunctionalAccount).ToList();
            var accountsTime = online.Any() ? online.Min(a => a.LastUpdate) : AccountStorage.Instance.AllElements.Any() ? AccountStorage.Instance.AllElements.Max(a => a.LastUpdate) : DateTime.Now;
            var ratesTime = AccountStorage.NeededRatesFor(ApplicationSettings.BaseCurrency).Distinct().Select(e => ExchangeRateHelper.GetRate(e)?.LastUpdate ?? DateTime.Now).DefaultIfEmpty(DateTime.Now).Min();

            var time = online.Count > 0 ? ratesTime < accountsTime ? ratesTime : accountsTime : ratesTime;

            Device.BeginInvokeOnMainThread(() => Footer.Text = time.LastUpdateString());
        }
    }
}