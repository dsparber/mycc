using System;
using System.Linq;
using data.settings;
using data.storage;
using enums;
using models;
using message;
using MyCryptos.resources;
using Xamarin.Forms;
using MyCryptos.view.components;
using System.Collections.Generic;
using helpers;
using data.repositories.account;
using tasks;

namespace view
{
	public partial class CoinsView : ContentPage
	{
		List<CoinViewCell> Cells;

		public CoinsView()
		{
			InitializeComponent();
			addSubscriber();
        
			Cells = new List<CoinViewCell>();

            if (Device.OS == TargetPlatform.Android) {
                CoinsSection.Title = InternationalisationResources.Coins;
            }
		}

		void updateView(bool loadMissing)
		{
			setCells(loadMissing);

			Header.TitleText = moneySum.ToString();
			Header.InfoText = string.Format(InternationalisationResources.DifferentCoinsCount, groups.ToList().Count);

			if (loadMissing)
			{
				var missingRates = new List<ExchangeRate>();

				foreach (var c in Cells)
				{
					var neededRate = new ExchangeRate(c.Currency, ApplicationSettings.BaseCurrency);

					var rate = (c.ExchangeRate != null && c.ExchangeRate.Rate.HasValue) ? c.ExchangeRate : null;
					rate = rate ?? ExchangeRateStorage.Instance.CachedElements.Find(x => x.Equals(neededRate));

					if (rate == null || !rate.Rate.HasValue)
					{
						missingRates.Add(neededRate);
					}
				}
				AppTasks.Instance.StartMissingRatesTask(missingRates);
			}
		}

		IEnumerable<IGrouping<Currency, Tuple<Account, AccountRepository>>> groups
		{
			get
			{
				var allAccounts = AccountStorage.Instance.CachedElementsWithRepository;
				return allAccounts.GroupBy(a => a.Item1.Money.Currency);
			}
		}

		void setCells(bool loadMissing)
		{
			var cells = new List<CoinViewCell>();

			foreach (var g in groups)
			{
				if (g.Key != null)
				{
					var cell = Cells.ToList().Find(e => g.Key.Equals(e.Currency));
					if (cell == null)
					{
						cell = new CoinViewCell(Navigation) { Accounts = g.ToList(), IsLoading = true };
					}
					else {
						cell.Accounts = g.ToList();
					}
					var rate = cell.Currency.Equals(ApplicationSettings.BaseCurrency) ? new ExchangeRate(cell.Currency, ApplicationSettings.BaseCurrency, 1) : null;
					rate = rate ?? ExchangeRateStorage.Instance.CachedElements.Find(c => c.Equals(new ExchangeRate(cell.Currency, ApplicationSettings.BaseCurrency))) ?? cell.ExchangeRate;
					if (rate != null && rate.Rate.HasValue)
					{
						cell.ExchangeRate = rate;
					}
					if (loadMissing)
					{
						cell.IsLoading = (rate == null || !rate.Rate.HasValue);
					}
					else {
						cell.IsLoading = false;
					}
					cells.Add(cell);
				}
			}
			Cells = cells;
			SortHelper.ApplySortOrder(Cells, CoinsSection);
		}

		Money moneySum
		{
			get
			{
				var sum = new Money(0, ApplicationSettings.BaseCurrency);
				foreach (var c in Cells)
				{
					if (c.MoneyReference != null && sum.Currency.Equals(c.MoneyReference.Currency))
					{
						sum += c.MoneyReference;
					}
				}
				return sum;
			}
		}

		void addSubscriber()
		{
			MessagingCenter.Subscribe<string>(this, MessageConstants.LoadedMissing, str => updateView(false));
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedExchangeRates, str => updateView(true));
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => updateView(true));
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, str => updateView(false));
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedSortOrder, str => SortHelper.ApplySortOrder(Cells, CoinsSection));

			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.StartedFetching, speed => Header.IsLoading = true);
			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.DoneFetching, speed => Header.IsLoading = false);
		}

		public async void Add(object sender, EventArgs e)
		{
			await AccountsView.AddDialog(this);
		}

		public async void SourcesClicked(object sender, EventArgs e)
		{
			await AccountsView.OpenSourcesView(Navigation);
		}
	}
}