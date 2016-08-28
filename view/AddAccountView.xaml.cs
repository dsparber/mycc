using System;
using tasks;
using Xamarin.Forms;

namespace view
{
	public partial class AddAccountView : ContentPage
	{
		public AddAccountView()
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

