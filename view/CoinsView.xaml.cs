using System;
using System.Linq;
using data.settings;
using data.storage;
using enums;
using MyCryptos.models;
using message;
using MyCryptos.resources;
using Xamarin.Forms;
using MyCryptos.view.components;
using System.Collections.Generic;
using helpers;
using data.repositories.account;

namespace view
{
	public partial class CoinsView : ContentPage
	{
		List<SortableViewCell> Cells;

		public CoinsView()
		{
			InitializeComponent();
			addSubscriber();

			Cells = new List<SortableViewCell>();
			CoinsSection.Title = InternationalisationResources.Coins;

			if (Device.OS == TargetPlatform.Android)
			{
				ToolbarItems.Remove(SourcesToolbarItem);
				Title = string.Empty;
			}
		}

		void updateView(string action)
		{
			setCells(action);

			Header.TitleText = moneySum.ToString();
			Header.InfoText = string.Format(InternationalisationResources.DifferentCoinsCount, groups.ToList().Count);
		}

		IEnumerable<IGrouping<Currency, Tuple<Account, AccountRepository>>> groups
		{
			get
			{
				var allAccounts = AccountStorage.Instance.AllElementsWithRepositories;
				return allAccounts.GroupBy(a => a.Item1.Money.Currency);
			}
		}

		void setCells(string action)
		{
			var cells = new List<SortableViewCell>();

			foreach (var g in groups)
			{
				if (g.Key != null)
				{
					var cell = Cells.OfType<CoinViewCell>().ToList().Find(e => g.Key.Equals(e.Currency));
					if (cell == null)
					{
						cell = new CoinViewCell(Navigation) { Accounts = g.ToList(), IsLoading = true };
					}
					else
					{
						cell.Accounts = g.ToList();
					}
					var rate = cell.Currency.Equals(ApplicationSettings.BaseCurrency) ? new ExchangeRate(cell.Currency, ApplicationSettings.BaseCurrency, 1) : null;
					rate = rate ?? ExchangeRateStorage.Instance.AllElements.Find(c => c.Equals(new ExchangeRate(cell.Currency, ApplicationSettings.BaseCurrency))) ?? cell.ExchangeRate;
					if (rate != null && rate.Rate.HasValue)
					{
						cell.ExchangeRate = rate;
					}
					if (!action.Equals(MessageConstants.UpdatedExchangeRates))
					{
						cell.IsLoading = (rate == null || !rate.Rate.HasValue);
					}
					else
					{
						cell.IsLoading = false;
					}
					cells.Add(cell);
				}
			}
			if (cells.Count == 0)
			{
				cells.Add(new CustomViewCell { Text = InternationalisationResources.NoCoins });
			}

			Cells = cells;
			SortHelper.ApplySortOrder(Cells, CoinsSection);
		}

		Money moneySum
		{
			get
			{
				var sum = new Money(0, ApplicationSettings.BaseCurrency);
				foreach (var c in Cells.OfType<CoinViewCell>())
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
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedExchangeRates, str =>updateView(MessageConstants.UpdatedExchangeRates));
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str =>  updateView(MessageConstants.UpdatedReferenceCurrency));
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, str => updateView(MessageConstants.UpdatedAccounts));
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