using System;
using System.Linq;
using System.Threading.Tasks;
using data.storage;
using MyCryptos.resources;
using Xamarin.Forms;

namespace view
{
	public partial class AccountsView : ContentPage
	{
		public AccountsView()
		{
			InitializeComponent();
		}

		public void AddAccount(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new NavigationPage(new AddAccountView()));
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (Device.OS == TargetPlatform.Android)
			{
				Title = InternationalisationResources.AppName;
			}
		}

		public async Task UpdateView()
		{
			AccountsTable.Root.Clear();

			foreach (var r in await AccountStorage.Instance.Repositories())
			{
				var section = new TableSection { Title = r.Name };

				foreach (var e in r.Elements)
				{
					section.Add(new TextCell { Text = e.Name, Detail = e.Money.ToString() });
				}

				if (section.Count > 0)
				{
					AccountsTable.Root.Add(section);
				}
			}

			var accounts = AccountsTable.Root.Sum(s => s.Count);
			var sources = AccountsTable.Root.Count();

			if (accounts == 0)
			{
				AccountsLabel.Text = InternationalisationResources.NoAccounts;
			}
			else if (accounts == 1)
			{
				AccountsLabel.Text = String.Format("{0} {1}", accounts, InternationalisationResources.Account);
			}
			else {
				AccountsLabel.Text = String.Format("{0} {1}", accounts, InternationalisationResources.Accounts);
			}

			if (sources == 0)
			{
				SourcesLabel.Text = InternationalisationResources.NoSources;
			}
			else if (sources == 1)
			{
				SourcesLabel.Text = String.Format("{0} {1}", sources, InternationalisationResources.Source);
			}
			else {
				SourcesLabel.Text = String.Format("{0} {1}", sources, InternationalisationResources.Sources);
			}
		}
	}
}

