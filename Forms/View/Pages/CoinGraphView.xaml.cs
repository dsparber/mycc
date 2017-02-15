using System;
using System.Linq;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Settings;
using MyCC.Forms.Messages;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages
{
    public partial class CoinGraphView
    {
        private readonly CoinGraphComponent _graphView;

        public CoinGraphView()
        {
            InitializeComponent();

            _graphView = new CoinGraphComponent(Navigation)
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            Stack.Children.Add(_graphView);

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

        private async void Refresh(object sender, EventArgs e)
        {
            RefreshItem.Clicked -= Refresh;
            await AppTaskHelper.FetchBalancesAndRates();
            RefreshItem.Clicked += Refresh;
        }

        private class HeaderTemplateSelector : DataTemplateSelector
        {
            protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new DataTemplate(() => new CoinHeaderComponent((Currency)item));
        }
    }
}