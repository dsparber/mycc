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
		}

		public void AddCoin(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new NavigationPage(new AddCoinView()));
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

			if (speed != FetchSpeedEnum.FAST)
			{
				LoadingPanel.IsVisible = true;
				LoadingIndicator.IsRunning = true;
			}

			var money = (await AccountStorage.Instance.AllElements()).Select(a => a.Money);
			var groups = money.GroupBy(a => a.Currency);
			var coins = groups.Select(g => new Money(g.ToList().Sum(m => m.Amount), g.Key));

			var section = new TableSection();

			var moneySum = new Money(0, ApplicationSettings.BaseCurrency);

			foreach (var c in coins)
			{
				var textCell = new TextCell { Text = c.ToString() };

				var rate = await ExchangeRateStorage.Instance.GetRate(c.Currency, ApplicationSettings.BaseCurrency, speed);

				if (rate != null && rate.Rate.HasValue)
				{
					var mRef = new Money(c.Amount * rate.Rate.Value, ApplicationSettings.BaseCurrency);
					moneySum += mRef;
					textCell.Detail = mRef.ToString();
				}
				section.Add(textCell);
			}

			// TODO Speed Improvments => Update every Cell seperately / Add Cell after adding new account
			CoinsTable.Root.Clear();
			CoinsTable.Root.Add(section);
			TotalMoneyLabel.Text = moneySum.ToString();

			LoadingPanel.IsVisible = false;
			LoadingIndicator.IsRunning = false;
		}
	}
}

