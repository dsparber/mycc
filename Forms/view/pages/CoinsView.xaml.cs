using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Models;
using MyCryptos.Core.Settings;
using MyCryptos.Core.Tasks;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.view;
using MyCryptos.view.components;
using view;
using Xamarin.Forms;
using CoinsTableView = MyCryptos.Forms.view.components.CoinsTableView;

namespace MyCryptos.Forms.view.pages
{
    public partial class CoinsView : ContentPage
    {
        ContentView TableView;
        CoinsGraphView GraphView;

        bool loadedView;

        public CoinsView()
        {
            InitializeComponent();

            TableView = new CoinsTableView();
            GraphView = new CoinsGraphView(Navigation)
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            Stack.Children.Add(TableView);
            Stack.Children.Add(GraphView);
            GraphView.IsVisible = ApplicationSettings.ShowGraphOnStartUp;
            TableView.IsVisible = !ApplicationSettings.ShowGraphOnStartUp;

            Tabs.Tabs = new List<string> { I18N.Table, I18N.Graph };
            Tabs.SelectedIndex = ApplicationSettings.ShowGraphOnStartUp ? 1 : 0;

            AddSubscriber();

            if (Device.OS == TargetPlatform.Android)
            {
                Title = string.Empty;
            }

            Tabs.SelectionChanged = selected =>
            {
                TableView.IsVisible = (selected == 0);
                GraphView.IsVisible = (selected == 1);
            };

            SetHeaderCarousel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!loadedView)
            {
                loadedView = true;
                GraphView.OnAppearing();
            }
        }


        private void PositionSelected(object sender, EventArgs e)
        {
            var currencies = ApplicationSettings.ReferenceCurrencies;

            ApplicationSettings.BaseCurrency = currencies[HeaderCarousel.Position];
            MessagingCenter.Send(MessageInfo.ValueChanged, Messaging.ReferenceCurrency);
        }

        void SetHeaderCarousel()
        {
            HeaderCarousel.ItemsSource = ApplicationSettings.ReferenceCurrencies.ToList();
            HeaderCarousel.Position = ApplicationSettings.ReferenceCurrencies.IndexOf(ApplicationSettings.BaseCurrency);
            if (HeaderCarousel.ItemTemplate == null)
            {
                HeaderCarousel.ItemTemplate = new HeaderTemplateSelector();
                HeaderCarousel.PositionSelected += PositionSelected;
            }

        }

        private void Add(object sender, EventArgs e)
        {
            Navigation.PushOrPushModal(new AddSourceView());
        }

        private void AddSubscriber()
        {
            Messaging.UpdatingExchangeRates.SubscribeFinished(this, () => IsBusy = false);
            Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => HeaderCarousel.Position = ApplicationSettings.ReferenceCurrencies.IndexOf(ApplicationSettings.BaseCurrency));
            Messaging.ReferenceCurrencies.SubscribeValueChanged(this, SetHeaderCarousel);
        }

        private async void Refresh(object sender, EventArgs e)
        {
            Messaging.UpdatingExchangeRates.SendStarted();
            await ApplicationTasks.FetchAllExchangeRates();
            Messaging.UpdatingExchangeRates.SendFinished();
        }

        private class HeaderTemplateSelector : DataTemplateSelector
        {
            protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new DataTemplate(() => new CoinsHeaderView((Currency)item));
        }
    }
}