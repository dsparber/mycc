using System;
using System.Linq;
using System.Threading.Tasks;
using data.settings;
using data.storage;
using enums;
using models;
using message;
using MyCryptos.resources;
using Xamarin.Forms;
using view.components;
using System.Collections.Generic;
using helpers;
using data.repositories.account;

namespace view
{
	public partial class CoinsView : ContentPage
	{
		Task updateViewTask;

		TableSection Section;
		List<CoinViewCell> Cells;

		public CoinsView()
		{
			InitializeComponent();

			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.UpdateCoinsView, async (speed) =>
			{
				if (updateViewTask != null)
				{
					await updateViewTask;
				}
				updateViewTask = UpdateView(speed.Speed);
				await updateViewTask;
			});

			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdateAccounts, async (str) =>
			{
				if (updateViewTask != null)
				{
					await updateViewTask;
				}
				updateViewTask = UpdateView(FetchSpeedEnum.FAST);
				await updateViewTask;
			});

			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.StartedFetching, (speed) => Header.IsLoading |= !speed.Speed.Equals(FetchSpeedEnum.FAST));
			MessagingCenter.Subscribe<string>(this, MessageConstants.SortOrderChanged, str => SortHelper.ApplySortOrder(Cells, Section));
		}

		public void AddCoin(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new NavigationPage(new AccountDetailView(null, null) { IsNew = true }));
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (Device.OS == TargetPlatform.Android)
			{
				Title = InternationalisationResources.AppName;
			}
		}

		public async Task UpdateView(FetchSpeedEnum speed)
		{
			showIsLoading(true, speed);
			initializeTable();

			Cells = await getCells();
			SortHelper.ApplySortOrder(Cells, Section);

			var moneySum = getMoneySum();
			Header.TitleText = moneySum.ToString();
			Header.InfoText = string.Format(InternationalisationResources.DifferentCoinsCount, (await groups()).ToList().Count);

			foreach (var c in Cells)
			{
				if (c.MoneyReference != null && moneySum.Currency.Equals(c.MoneyReference.Currency))
				{
					moneySum -= c.MoneyReference;
				}

				if (c.IsLoading)
				{
					var rate = await ExchangeRateStorage.Instance.GetRate(c.Currency, ApplicationSettings.BaseCurrency, speed);

					c.ExchangeRate = rate;
					c.IsLoading = false;
				}

				if (c.MoneyReference != null && moneySum.Currency.Equals(c.MoneyReference.Currency))
				{
					moneySum += c.MoneyReference;
				}
				Header.TitleText = moneySum.ToString();
			}

			SortHelper.ApplySortOrder(Cells, Section);
			showIsLoading(false, speed);
		}

		void showIsLoading(bool loading, FetchSpeedEnum speed)
		{
			if (!speed.Equals(FetchSpeedEnum.FAST))
			{
				Header.IsLoading = loading;
			}
		}

		async Task<IEnumerable<IGrouping<Currency, Tuple<Account, AccountRepository>>>> groups()
		{
			var allAccounts = await AccountStorage.Instance.AllElementsWithRepositories();
			return allAccounts.GroupBy(a => a.Item1.Money.Currency);
		}

		void initializeTable()
		{
			if (CoinsTable.Root.Count == 0)
			{
				CoinsTable.Root.Add(new TableSection());
			}

			Section = CoinsTable.Root[0];
			Cells = new List<CoinViewCell>();
		}

		async Task<List<CoinViewCell>> getCells()
		{
			var cells = new List<CoinViewCell>();

			foreach (var g in await groups())
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
					cells.Add(cell);
				}
			}
			return cells;
		}

		Money getMoneySum()
		{
			var moneySum = new Money(0, ApplicationSettings.BaseCurrency);
			foreach (var c in Cells)
			{
				if (c.MoneyReference != null && moneySum.Currency.Equals(c.MoneyReference.Currency))
				{
					moneySum += c.MoneyReference;
				}
			}
			return moneySum;
		}
	}
}