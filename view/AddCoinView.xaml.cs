using System;
using System.Linq;
using data.repositories.account;
using data.storage;
using models;
using Xamarin.Forms;

namespace view
{
	public partial class AddCoinView : ContentPage
	{
		public AddCoinView()
		{
			InitializeComponent();
		}

		public void Cancel(object sender, EventArgs e)
		{
			Navigation.PopModalAsync();
		}

		public async void Save(object sender, EventArgs e)
		{
			var name = LabelInput.Text;
			var value = ValueInput.Text;
			var currency = CurrencyInput.Text;

			var currencyObject = (await CurrencyStorage.Instance.AllElements()).Find(c => c.Code.ToLower().Equals(currency.ToLower()));
			if (currencyObject == null)
			{
				currencyObject = new Currency(currency);
			}

			var money = new Money(Decimal.Parse(value), currencyObject);
			var account = new Account(name, money);
			await (await AccountStorage.Instance.Repositories()).Find(r => r is LocalAccountRepository).Add(account);
			await Navigation.PopModalAsync();
		}
	}
}

