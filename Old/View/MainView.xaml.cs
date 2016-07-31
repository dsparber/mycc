using System;
using Xamarin.Forms;


namespace MyCryptos
{
	public partial class MainView : ContentPage
	{

		public MainView()
		{

		}

		protected async override void OnAppearing()
		{
			
		}

		private void AddAccount(object sender, EventArgs args)
		{
			Navigation.PushModalAsync(new NavigationPage(new NewAccountView()));
		}
	}
}