using System;
using data.settings;
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

			if (AppTasks.Instance.IsFetchTaskStarted && !AppTasks.Instance.IsFetchTaskFinished)
			{
				await AppTasks.Instance.FetchTask;
			}

			AppTasks.Instance.StartAddAccountTask(name, decimal.Parse(value), currency.Code);
			await Navigation.PopModalAsync();
		}
	}
}

