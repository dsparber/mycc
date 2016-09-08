using System;
using data.settings;
using models;
using tasks;
using view.components;
using Xamarin.Forms;

namespace view
{
	public partial class AddAccountView : ContentPage
	{

		CurrencyEntryCell currencyEntryCell;

		public AddAccountView()
		{
			InitializeComponent();

			currencyEntryCell = new CurrencyEntryCell(Navigation);
			MoneySection.Add(currencyEntryCell);
			currencyEntryCell.SelectedCurrency = ApplicationSettings.BaseCurrency;
		}

		public void Cancel(object sender, EventArgs e)
		{
			Navigation.PopModalAsync();
		}

		public async void Save(object sender, EventArgs e)
		{
			var name = LabelInput.Text;
			var value = ValueInput.Text ?? "0";
			var currency = currencyEntryCell.SelectedCurrency;

			// TODO Floatpoint numbers
			// TODO Default label

			if (AppTasks.Instance.IsFetchTaskStarted && !AppTasks.Instance.IsFetchTaskStarted)
			{
				await AppTasks.Instance.FetchTask;
			}

			var account = new Account(name, new Money(decimal.Parse(value), currency));

			AppTasks.Instance.StartAddAccountTask(account);
			await Navigation.PopModalAsync();
		}
	}
}

