using System;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.ExchangeRate.Helpers;
using MyCC.Core.ExchangeRate.Model;
using MyCC.Core.Settings;
using MyCC.Forms.Messages;
using MyCC.Forms.Tasks;
using MyCC.Forms.view.components;
using Xamarin.Forms;

namespace MyCC.Forms.view.pages
{
    public partial class RateView
    {
        private readonly RatesTableComponent _tableView;

        public RateView()
        {
            InitializeComponent();

            _tableView = new RatesTableComponent(Navigation);

            Stack.Children.Add(_tableView);

            AddSubscriber();

            if (Device.OS == TargetPlatform.Android)
            {
                Title = string.Empty;
            }

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
            HeaderCarousel.ShowIndicators = (HeaderCarousel.ItemsSource.Count > 1);

            if (HeaderCarousel.ItemTemplate != null) return;

            HeaderCarousel.ItemTemplate = new HeaderTemplateSelector();
            HeaderCarousel.PositionSelected += PositionSelected;
            HeaderCarousel.HeightRequest = 120;
        }

        private void AddSubscriber()
        {
            Messaging.ReferenceCurrencies.SubscribeValueChanged(this, SetHeaderCarousel);
        }

        private async void Refresh(object sender, EventArgs e)
        {
            await AppTaskHelper.FetchBalancesAndRates();
            await AppTaskHelper.FetchMissingRates(ApplicationSettings.WatchedCurrencies
                                    .Concat(ApplicationSettings.AllReferenceCurrencies)
                                    .Select(c => new ExchangeRate(Currency.Btc, c))
                                    .Select(r => ExchangeRateHelper.GetRate(r) ?? r)
                                    .Where(r => r.Rate == null)
                                    .Concat(AccountStorage.NeededRates).ToList());
        }

        private class HeaderTemplateSelector : DataTemplateSelector
        {
            private bool _isUpdatingExchangeRates;

            public HeaderTemplateSelector()
            {
                Messaging.FetchMissingRates.SubscribeStartedAndFinished(this, () => _isUpdatingExchangeRates = true, () => _isUpdatingExchangeRates = false);
            }

            protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new DataTemplate(() =>
            {
                var c = (Currency)item;

                return new RatesHeaderComponent(c, _isUpdatingExchangeRates);
            });
        }
    }
}