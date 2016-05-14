using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace MyCryptos
{
	public partial class MainView : ContentPage
	{
		ObservableCollection<Account> accounts = new ObservableCollection<Account> ();

		public MainView ()
		{
			InitializeComponent ();

			listView.ItemsSource = accounts;

			accounts.Add (new Account{ Money = new Money { Amount = 30, Currency = Currency.BTC } });
		}
	}
}