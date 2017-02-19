using System;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Messages;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components;
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

            if (ApplicationSettings.FirstLaunch)
            {
                Task.Run(async () => await AppTaskHelper.FetchMissingRates(ApplicationSettings.WatchedCurrencies
                                       .Concat(ApplicationSettings.AllReferenceCurrencies)
                                       .Select(c => new ExchangeRate(Currency.Btc, c))
                                       .Select(r => ExchangeRateHelper.GetRate(r) ?? r)
                                       .Where(r => r.Rate == null)
                                       .Concat(AccountStorage.NeededRates).ToList()));
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
        }

        private async void Refresh()
        {
            await AppTaskHelper.UpdateRates();
            _pullToRefresh.IsRefreshing = false;
        }

        private class HeaderTemplateSelector : DataTemplateSelector
        {
            protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new DataTemplate(() =>
             {
                 var c = (Currency)item;

                 return new RatesHeaderComponent(c);
             });
        }
    }
}