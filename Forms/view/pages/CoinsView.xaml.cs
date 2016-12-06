using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.settings;
using MyCryptos.Core.tasks;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.view.components;
using MyCryptos.view;
using view;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
	public partial class CoinsView
	{
		private readonly ContentView tableView;
		private readonly CoinsGraphView graphView;

		private bool loadedView;

		public CoinsView()
		{
			InitializeComponent();

			tableView = new CoinsTableView();
			graphView = new CoinsGraphView(Navigation)
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			Stack.Children.Add(tableView);
			Stack.Children.Add(graphView);
			graphView.IsVisible = ApplicationSettings.ShowGraphOnStartUp;
			tableView.IsVisible = !ApplicationSettings.ShowGraphOnStartUp;

			Tabs.Tabs = new List<string> { I18N.Table, I18N.Graph };
			Tabs.SelectedIndex = ApplicationSettings.ShowGraphOnStartUp ? 1 : 0;

			AddSubscriber();

			if (Device.OS == TargetPlatform.Android)
			{
				Title = string.Empty;
			}

			Tabs.SelectionChanged = selected =>
			{
				tableView.IsVisible = (selected == 0);
				graphView.IsVisible = (selected == 1);
			};

			SetHeaderCarousel();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (loadedView) return;

			loadedView = true;
			graphView.OnAppearing();
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

		private void Add(object sender, EventArgs e)
		{
			Navigation.PushOrPushModal(new AddSourceView());
		}

		private void AddSubscriber()
		{
			Messaging.FetchMissingRates.SubscribeFinished(this, () => IsBusy = false);
			Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => HeaderCarousel.Position = ApplicationSettings.ReferenceCurrencies.IndexOf(ApplicationSettings.BaseCurrency));
			Messaging.ReferenceCurrencies.SubscribeValueChanged(this, SetHeaderCarousel);
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