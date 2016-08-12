using System;
using Xamarin.Forms;

namespace view
{
	public partial class AccountsView : ContentPage
	{
		public AccountsView()
		{
			InitializeComponent();
		}

		public void AddAccount(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new NavigationPage(new AddAccountView()));
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (Device.OS == TargetPlatform.Android)
			{
				Title = MyCryptos.resources.Resources.AppName;
			}
		}
	}
}

