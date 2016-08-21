using System;
using System.Linq;
using System.Threading.Tasks;
using data.settings;
using data.storage;
using models;
using MyCryptos.resources;
using Xamarin.Forms;

namespace view
{
	public partial class CoinsView : ContentPage
	{
		public CoinsView()
		{
			InitializeComponent();
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

		public async Task UpdateView(bool fast, bool done)
		{
			if (!done)
			{
				TotalMoneyLabel.IsVisible = false;
				LoadingIndicator.IsVisible = true;
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

				var rate = await ExchangeRateStorage.Instance.GetRate(c.Currency, ApplicationSettings.BaseCurrency, fast);

				if (rate != null && rate.Rate.HasValue)
				{
					var mRef = new Money(c.Amount * rate.Rate.Value, ApplicationSettings.BaseCurrency);
					moneySum += mRef;
					textCell.Detail = mRef.ToString();
				}
				section.Add(textCell);
			}

			CoinsTable.Root.Clear();
			CoinsTable.Root.Add(section);
			TotalMoneyLabel.Text = moneySum.ToString();

			if (done)
			{
				LoadingIndicator.IsVisible = false;
				LoadingIndicator.IsRunning = false;
				TotalMoneyLabel.IsVisible = true;
			}
		}
	}
}

