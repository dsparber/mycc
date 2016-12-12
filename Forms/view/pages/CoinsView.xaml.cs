using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.settings;
using MyCryptos.Core.tasks;
using MyCryptos.Core.Types;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.view.components;
using MyCryptos.Forms.view.overlays;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
    public partial class CoinsView
    {
        private readonly ContentView listView;
        private readonly CoinsTableView tableView;
        private readonly CoinsGraphView graphView;

        private bool loadedView;

        public CoinsView()
        {
            InitializeComponent();

            listView = new CoinsListView();
            tableView = new CoinsTableView(Navigation);
            graphView = new CoinsGraphView(Navigation)
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            Stack.Children.Add(listView);
            Stack.Children.Add(tableView);
            Stack.Children.Add(graphView);
            listView.IsVisible = ApplicationSettings.DefaultPage.Equals(StartupPage.ListView);
            graphView.IsVisible = ApplicationSettings.DefaultPage.Equals(StartupPage.GraphView);
            tableView.IsVisible = ApplicationSettings.DefaultPage.Equals(StartupPage.TableView);

            Tabs.Tabs = new List<string> { I18N.List, I18N.Table, I18N.Graph };
            Tabs.SelectedIndex = ApplicationSettings.DefaultPage.Equals(StartupPage.ListView) ? 0 : ApplicationSettings.DefaultPage.Equals(StartupPage.TableView) ? 1 : 2;

            AddSubscriber();

            if (Device.OS == TargetPlatform.Android)
            {
                Title = string.Empty;
            }

            Tabs.SelectionChanged = selected =>
            {
                listView.IsVisible = (selected == 0);
                tableView.IsVisible = (selected == 1);
                graphView.IsVisible = (selected == 2);
            };

            SetHeaderCarousel();
            SetNoSourcesView();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (loadedView) return;

            loadedView = true;
            graphView.OnAppearing();
            tableView.OnAppearing();
        }


        private void PositionSelected(object sender, EventArgs e)
        {
            var currencies = ApplicationSettings.ReferenceCurrencies;

            ApplicationSettings.BaseCurrency = currencies[HeaderCarousel.Position];
            MessagingCenter.Send(MessageInfo.ValueChanged, Messaging.ReferenceCurrency);
        }

        private void SetHeaderCarousel()
        {
            HeaderCarousel.ItemsSource = ApplicationSettings.ReferenceCurrencies.ToList();
            HeaderCarousel.Position = ApplicationSettings.ReferenceCurrencies.IndexOf(ApplicationSettings.BaseCurrency);
            if (HeaderCarousel.ItemTemplate != null) return;

            HeaderCarousel.ItemTemplate = new HeaderTemplateSelector();
            HeaderCarousel.PositionSelected += PositionSelected;
            HeaderCarousel.HeightRequest = 120;// new CoinsHeaderView().HeightRequest;
        }

        private void SetNoSourcesView()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                NoSourcesView.IsVisible = (AccountStorage.Instance.AllElements.Count == 0);
                Stack.IsVisible = AccountStorage.Instance.AllElements.Count != 0;
            });
        }

        private void AddSubscriber()
        {
            Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => HeaderCarousel.Position = ApplicationSettings.ReferenceCurrencies.IndexOf(ApplicationSettings.BaseCurrency));
            Messaging.ReferenceCurrencies.SubscribeValueChanged(this, SetHeaderCarousel);

            Messaging.Loading.SubscribeFinished(this, SetNoSourcesView);
            Messaging.FetchMissingRates.SubscribeFinished(this, SetNoSourcesView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, SetNoSourcesView);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, SetNoSourcesView);
        }

        private async void Refresh(object sender, EventArgs e)
        {
            await ApplicationTasks.FetchBalancesAndRates(Messaging.UpdatingAccountsAndRates.SendStarted, Messaging.UpdatingAccountsAndRates.SendFinished, ErrorOverlay.Display);
        }

        private class HeaderTemplateSelector : DataTemplateSelector
        {
            private bool isUpdatingExchangeRates;

            public HeaderTemplateSelector()
            {
                Messaging.FetchMissingRates.SubscribeStartedAndFinished(this, () => isUpdatingExchangeRates = true, () => isUpdatingExchangeRates = false);
            }

            protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new DataTemplate(() => new CoinsHeaderView((Currency)item) { IsLoading = isUpdatingExchangeRates });
        }
    }
}