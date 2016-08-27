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

			AppTasks.Instance.StartAddAccountTask(name, decimal.Parse(value), currency);
			await Navigation.PopModalAsync();
		}
	}
}

