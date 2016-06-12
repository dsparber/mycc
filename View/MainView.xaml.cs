using System;
using Xamarin.Forms;


namespace MyCryptos
{
	public partial class MainView : ContentPage
	{
		private MainViewModel viewModel;

		public MainView()
		{
			InitializeComponent();

			viewModel = new MainViewModel();
			BindingContext = viewModel;
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			viewModel.IsLoading = true;

			await ExchangeRateCollection.Instance.LoadRates();

			await viewModel.AccountsCollection.LoadAccounts();
			viewModel.RaisePropertyChanged("AccountsCollection");

			listView.ItemsSource = viewModel.AccountsCollection.Accounts;

			viewModel.IsLoading = false;
		}

		private void AddAccount(object sender, EventArgs args)
		{
			Navigation.PushModalAsync(new NavigationPage(new NewAccountView()));
		}
	}
}