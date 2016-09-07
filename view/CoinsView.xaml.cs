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

namespace view
{
	public partial class CoinsView : ContentPage
	{
		Task updateViewTask;

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

			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.StartedFetching, (speed) =>
			{
				Header.IsLoading |= !speed.Speed.Equals(FetchSpeedEnum.FAST);
			});
		}

		public void AddCoin(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new NavigationPage(new AddAccountView()));
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

			if (CoinsTable.Root.Count == 0)
			{
				CoinsTable.Root.Add(new TableSection());
			}

			var allAccounts = await AccountStorage.Instance.AllElementsWithRepositories();
			var groups = allAccounts.GroupBy(a => a.Item1.Money.Currency);

			var section = CoinsTable.Root[0];
			var cells = new List<CoinViewCell>();

			foreach (var g in groups)
			{
				var cell = section.Select(e => (CoinViewCell)e).ToList().Find(e => e.Currency.Equals(g.Key));
				if (cell == null)
				{
					cell = new CoinViewCell(Navigation) { Accounts = g.ToList() };
				}
				else {
					cell.Accounts = g.ToList();
				}
				cells.Add(cell);
			}
			setCells(section, cells);

			var moneySum = new Money(0, ApplicationSettings.BaseCurrency);
			foreach (var c in cells)
			{
				if (c.MoneyReference != null && moneySum.Currency.Equals(c.MoneyReference.Currency))
				{
					moneySum += c.MoneyReference;
				}
			}
			Header.TitleText = moneySum.ToString();

			foreach (var c in cells)
			{
				c.IsLoading = true;

				var rate = await ExchangeRateStorage.Instance.GetRate(c.Currency, ApplicationSettings.BaseCurrency, speed);

				if (c.MoneyReference != null && moneySum.Currency.Equals(c.MoneyReference.Currency))
				{
					moneySum -= c.MoneyReference;
				}

				c.ExchangeRate = rate;
				setCells(section, cells);

				if (c.MoneyReference != null)
				{
					moneySum += c.MoneyReference;
				}
				Header.TitleText = moneySum.ToString();

				c.IsLoading = false;
			}

			showIsLoading(false, speed);
		}

		void showIsLoading(bool loading, FetchSpeedEnum speed)
		{
			if (!speed.Equals(FetchSpeedEnum.FAST))
			{
				Header.IsLoading = loading;
			}
		}

		void setCells(TableSection section, List<CoinViewCell> cells)
		{
			cells = SortHelper.SortCells(cells);

			section.Clear();
			foreach (var c in cells)
			{
				section.Add(c);
			}
		}
	}
}

