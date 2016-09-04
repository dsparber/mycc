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
				if (!speed.Speed.Equals(FetchSpeedEnum.FAST))
				{
					LoadingPanel.IsVisible = true;
					LoadingIndicator.IsRunning = true;
				}
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
			if (!speed.Equals(FetchSpeedEnum.FAST))
			{
				LoadingPanel.IsVisible = true;
				LoadingIndicator.IsRunning = true;
			}

			if (CoinsTable.Root.Count == 0)
			{
				CoinsTable.Root.Add(new TableSection());
			}

			var allAccounts = await AccountStorage.Instance.AllElements();

			var money = allAccounts.Select(a => a.Money);
			var groups = money.GroupBy(a => a.Currency);
			var coins = groups.Select(g => new Money(g.ToList().Sum(m => m.Amount), g.Key));

			var section = CoinsTable.Root[0];
			var cells = new List<CoinViewCell>();

			foreach (var c in coins)
			{
				var cell = section.Select(e => (CoinViewCell)e).ToList().Find(e => e.SumMoney.Currency.Equals(c.Currency));
				if (cell == null)
				{
					cell = new CoinViewCell { SumMoney = c };
				}
				else {
					cell = new CoinViewCell { SumMoney = cell.SumMoney, ReferenceValue = cell.ReferenceValue };
				}
				cells.Add(cell);
			}

			cells = sortCells(cells);

			section.Clear();
			foreach (var c in cells)
			{
				section.Add(c);
			}

			var moneySum = new Money(0, ApplicationSettings.BaseCurrency);
			foreach (var c in cells)
			{
				if (c.ReferenceValue != null && c.ReferenceValue.Currency.Equals(moneySum.Currency))
				{
					moneySum += c.ReferenceValue;
				}
			}
			TotalMoneyLabel.Text = moneySum.ToString();

			var rates = new List<ExchangeRate>();

			foreach (var c in cells)
			{
				c.IsLoading = true;
				var rate = await ExchangeRateStorage.Instance.GetRate(c.SumMoney.Currency, ApplicationSettings.BaseCurrency, speed);
				rates.Add(rate);

				if (rate != null && rate.Rate.HasValue)
				{
					var mRef = new Money(c.SumMoney.Amount * rate.Rate.Value, ApplicationSettings.BaseCurrency);
					if (c.ReferenceValue != null && c.ReferenceValue.Currency.Equals(moneySum.Currency))
					{
						moneySum -= c.ReferenceValue;
					}
					moneySum += mRef;
					TotalMoneyLabel.Text = moneySum.ToString();
					c.ReferenceValue = mRef;
					cells = sortCells(cells);
				}
				c.IsLoading = false;
			}

			cells = sortCells(cells);

			section.Clear();
			foreach (var c in cells)
			{
				c.Tapped += (sender, e) =>
				{
					Navigation.PushAsync(new CoinDetailView(allAccounts.Where(a => a.Money.Currency.Equals(c.SumMoney.Currency)), rates.Find(r => r.ReferenceCurrency.Equals(c.SumMoney.Currency))));
				};
				section.Add(c);
			}

			LoadingPanel.IsVisible = false;
			LoadingIndicator.IsRunning = false;
		}

		List<CoinViewCell> sortCells(List<CoinViewCell> cells)
		{
			if (ApplicationSettings.SortOrder.Equals(SortOrder.BY_VALUE))
			{
				if (ApplicationSettings.SortDirection.Equals(SortDirection.ASCENDING))
				{
					cells = cells.OrderBy(c => c.ReferenceValue != null ? c.ReferenceValue.Amount : 0).ToList();
				}
				else {
					cells = cells.OrderByDescending(c => c.ReferenceValue != null ? c.ReferenceValue.Amount : 0).ToList();
				}
			}

			if (ApplicationSettings.SortOrder.Equals(SortOrder.BY_UNITS))
			{
				if (ApplicationSettings.SortDirection.Equals(SortDirection.ASCENDING))
				{
					cells = cells.OrderBy(c => c.SumMoney.Amount).ToList();
				}
				else {
					cells = cells.OrderByDescending(c => c.SumMoney.Amount).ToList();
				}
			}

			if (ApplicationSettings.SortOrder.Equals(SortOrder.ALPHABETICAL))
			{
				if (ApplicationSettings.SortDirection.Equals(SortDirection.ASCENDING))
				{
					cells = cells.OrderBy(c => c.SumMoney.Currency.Code).ToList();
				}
				else {
					cells = cells.OrderByDescending(c => c.SumMoney.Currency.Code).ToList();
				}
			}

			return cells;
		}
	}
}

