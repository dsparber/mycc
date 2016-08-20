using System;
using System.Linq;
using System.Collections.Generic;
using data.repositories.account;
using data.storage;
using Xamarin.Forms;
using System.Threading.Tasks;

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

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			var accountStorage = AccountStorage.Instance;
			await accountStorage.Fetch();
			var localStorages = await accountStorage.RepositoriesOfType<LocalAccountRepository>();
			foreach (var s in localStorages)
			{
				StorageSection.Insert(0, new TextCell { Text = s.Name });
			}
		}
	}
}

