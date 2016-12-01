using System;
using System.Linq;
using Xamarin.Forms;
using System.Collections.Generic;
using MyCryptos.Core.Constants;
using MyCryptos.Core.Enums;
using MyCryptos.Core.Models;
using MyCryptos.Core.Settings;
using MyCryptos.Core.Tasks;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Resources;
using MyCryptos.view;
using MyCryptos.view.components;

namespace view
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

            addSubscriber();

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


        public void PositionSelected(object sender, EventArgs e)
        {
            var currencies = ApplicationSettings.ReferenceCurrencies;
            if (HeaderCarousel.Position >= 0 && HeaderCarousel.Position < ApplicationSettings.ReferenceCurrencies.Count)
            {
                ApplicationSettings.BaseCurrency = currencies[HeaderCarousel.Position];
            }
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

        public async void Add(object sender, EventArgs e)
        {
            await Navigation.PushOrPushModal(new AddSourceView());
        }

        void addSubscriber()
        {
            MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.StartedFetching, speed => setLoadingAnimation(speed, true));
            MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.DoneFetching, speed => setLoadingAnimation(speed, false));

            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, s => HeaderCarousel.Position = ApplicationSettings.ReferenceCurrencies.IndexOf(ApplicationSettings.BaseCurrency));
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrencies, s => SetHeaderCarousel());
        }

        void setLoadingAnimation(FetchSpeed speed, bool loading)
        {
            if (loading)
            {
                IsBusy = loading;
            }
            else
            {
                if (speed.Speed != FetchSpeedEnum.FAST)
                {
                    IsBusy = false;
                }
            }
        }

        void Refresh(object sender, EventArgs e)
        {
            AppTasks.Instance.StartFetchTask(false);
        }

        private class HeaderTemplateSelector : DataTemplateSelector
        {
            private List<Tuple<Currency, CoinsHeaderView>> existingViews;

            public HeaderTemplateSelector()
            {
                existingViews = new List<Tuple<Currency, CoinsHeaderView>>();
            }

            protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new DataTemplate(() =>
            {
                var currency = (Currency)item;
                var cell = existingViews.Find(t => t.Item1.Equals(currency))?.Item2;
                if (cell == null)
                {
                    cell = new CoinsHeaderView(currency);
                    existingViews.Add(Tuple.Create(currency, cell));
                }
                else
                {
                    cell = new CoinsHeaderView(currency) { IsLoading = cell.IsLoading };
                }
                return cell;
            });
        }
    }
}