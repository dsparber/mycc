using System;
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
	}
}

