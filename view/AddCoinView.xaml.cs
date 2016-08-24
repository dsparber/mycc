using System;
using data.repositories.account;
using data.repositories.currency;
using data.storage;
using models;
using tasks;
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

			if (!AppTasks.Instance.IsFetchTaskFinished)
			{
				await AppTasks.Instance.FetchTask;
			}

			var currencyObject = await CurrencyStorage.Instance.GetByString(currency);
			if (currencyObject == null)
			{
				currencyObject = new Currency(currency.ToUpper());
				await (await CurrencyStorage.Instance.Repositories()).Find(r => r is LocalCurrencyRepository).Add(currencyObject);
				currencyObject = await CurrencyStorage.Instance.GetByString(currency);
			}

			var money = new Money(decimal.Parse(value), currencyObject);
			var account = new Account(name, money);
			await (await AccountStorage.Instance.Repositories()).Find(r => r is LocalAccountRepository).Add(account);
			await Navigation.PopModalAsync();
		}
	}
}

