using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ServiceModel.Channels;

namespace MyCryptos
{
	public partial class MainView : ContentPage
	{
		private MainViewViewModel viewModel;

		public MainView ()
		{
			InitializeComponent ();

			viewModel = new MainViewViewModel ();
			BindingContext = viewModel;
		}

		protected async override void OnAppearing ()
		{            
			base.OnAppearing ();

			await ExchangeRateCollection.Instance.LoadRates ();

			await viewModel.AccountsCollection.LoadAccounts ();
			viewModel.RaisePropertyChanged ("AccountsCollection");

			listView.ItemsSource = viewModel.AccountsCollection.Accounts;

			viewModel.IsLoading = false;
		}
	}
}