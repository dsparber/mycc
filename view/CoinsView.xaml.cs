using System;
using System.Linq;
using System.Threading.Tasks;
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

		public async Task UpdateView()
		{
			CoinsTable.Root.Clear();

			var money = (await AccountStorage.Instance.AllElements()).Select(a => a.Money);
			var groups = money.GroupBy(a => a.Currency);
			var coins = groups.Select(g => new Money(g.ToList().Sum(m => m.Amount), g.Key));

			var section = new TableSection();

			foreach (var c in coins)
			{
				section.Add(new TextCell { Text = c.ToString()});
			}

			CoinsTable.Root.Add(section);
		}
	}
}

