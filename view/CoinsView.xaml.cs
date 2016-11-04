using System;
using System.Linq;
using data.settings;
using data.storage;
using enums;
using MyCryptos.models;
using message;
using MyCryptos.resources;
using Xamarin.Forms;
using MyCryptos.helpers;
using System.Collections.Generic;
using tasks;
using MyCryptos.view;
using System.Collections.ObjectModel;

namespace view
{
	public partial class CoinsView : ContentPage
	{

		public CoinsView()
		{
			InitializeComponent();

            var views = new ObservableCollection<ContentView> {
                   new CoinsTableView (),
                   new CoinsGraphView()
            };

            Carousel.ItemsSource = views;

            addSubscriber();

			if (Device.OS == TargetPlatform.Android)
			{
				ToolbarItems.Remove(SourcesToolbarItem);
				Title = string.Empty;
			}
		}

		void updateView()
		{
            var sum = moneySum;
            var amountDifferentCurrencies = AccountStorage.Instance.AllElements.Select(a => a.Money.Currency).Distinct().ToList().Count;

            Header.TitleText = (sum.Amount > 0) ? sum.ToString() : string.Format("? {0}", sum.Currency.Code);
			Header.InfoText = string.Format(InternationalisationResources.DifferentCoinsCount, amountDifferentCurrencies);
		}

		Money moneySum
		{
			get
			{
                var neededRates = new List<ExchangeRate>();

                var amount = AccountStorage.Instance.AllElements.Select(a =>
                {
                    var neededRate = new ExchangeRate(a.Money.Currency, ApplicationSettings.BaseCurrency);
                    var rate = ExchangeRateHelper.GetRate(neededRate);

                    if (rate == null || !rate.Rate.HasValue)
                    {
                        neededRates.Add(neededRate);
                    }

                    return a.Money.Amount * (rate ?? neededRate).RateNotNull;
                }).Sum();

                AppTasks.Instance.StartMissingRatesTask(neededRates.Distinct());

                return new Money(amount, ApplicationSettings.BaseCurrency);
			}
		}

		void addSubscriber()
		{
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedExchangeRates, str => updateView());
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => updateView());
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, str => updateView());

			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.StartedFetching, speed => setLoadingAnimation(speed, true));
			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.DoneFetching, speed => setLoadingAnimation(speed, false));
		}

		public async void Add(object sender, EventArgs e)
		{
			await AccountsView.AddDialog(this);
		}

		public async void SourcesClicked(object sender, EventArgs e)
		{
			await AccountsView.OpenSourcesView(Navigation);
		}

		void setLoadingAnimation(FetchSpeed speed, bool loading)
		{
			if (speed.Speed == FetchSpeedEnum.SLOW)
			{
				IsBusy = loading;
			}
			else {
				Header.IsLoading = loading;
			}
		}
	}
}