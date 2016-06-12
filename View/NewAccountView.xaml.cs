using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MyCryptos
{
	public partial class NewAccountView : ContentPage
	{
		public NewAccountView ()
		{
			InitializeComponent ();
		}

		private void CancelClicked(object sender, EventArgs args)
		{
			Navigation.PopModalAsync();
		}

		private void SaveAccount(object sender, EventArgs args)
		{
			AccountsCollection.Instance.Accounts.Add(new Account(name.Text, new Money(decimal.Parse(amount.Text), new Currency(null, currency.Text))));
			Navigation.PopModalAsync();
		}
	}
}

