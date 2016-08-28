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

			var money = (await AccountStorage.Instance.AllElements()).Select(a => a.Money);
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
				cells.Add(cell);
			}

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

			foreach (var c in cells)
			{
				c.IsLoading = true;
				var rate = await ExchangeRateStorage.Instance.GetRate(c.SumMoney.Currency, ApplicationSettings.BaseCurrency, speed);

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
				}
				c.IsLoading = false;
			}

			LoadingPanel.IsVisible = false;
			LoadingIndicator.IsRunning = false;
		}
	}
}

