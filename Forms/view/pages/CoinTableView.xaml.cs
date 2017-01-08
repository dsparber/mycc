using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.settings;
using MyCryptos.Core.Types;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.Tasks;
using MyCryptos.Forms.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
	public partial class CoinTableView
	{
		private readonly CoinTableComponent tableView;

		public CoinTableView()
		{
			InitializeComponent();

			tableView = new CoinTableComponent(Navigation);

			Stack.Children.Add(tableView);

			AddSubscriber();

			if (Device.OS == TargetPlatform.Android)
			{
				Title = string.Empty;
			}

			SetHeaderCarousel();
			SetNoSourcesView();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			tableView.OnAppearing();
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
			HeaderCarousel.ShowIndicators = (HeaderCarousel.ItemsSource.Count > 1);


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
			Messaging.ReferenceCurrency.SubscribeValueChanged(this, () => HeaderCarousel.Position = ApplicationSettings.MainCurrencies.IndexOf(ApplicationSettings.BaseCurrency));
			Messaging.ReferenceCurrencies.SubscribeValueChanged(this, SetHeaderCarousel);

			Messaging.Loading.SubscribeFinished(this, SetNoSourcesView);
			Messaging.FetchMissingRates.SubscribeFinished(this, SetNoSourcesView);
			Messaging.UpdatingAccounts.SubscribeFinished(this, SetNoSourcesView);
			Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, SetNoSourcesView);
		}

		private async void Refresh(object sender, EventArgs e)
		{
			await AppTaskHelper.FetchBalancesAndRates();
			await AppTaskHelper.FetchMissingRates();
		}

		private class HeaderTemplateSelector : DataTemplateSelector
		{
			private bool isUpdatingExchangeRates;

			public HeaderTemplateSelector()
			{
				Messaging.FetchMissingRates.SubscribeStartedAndFinished(this, () => isUpdatingExchangeRates = true, () => isUpdatingExchangeRates = false);
			}

			protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new DataTemplate(() => new CoinHeaderComponent((Currency)item) { IsLoading = isUpdatingExchangeRates });
		}
	}
}