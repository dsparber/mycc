using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.storage;
using enums;
using helpers;
using message;
using MyCryptos.resources;
using view.components;
using Xamarin.Forms;

namespace view
{
	public partial class AccountsView : ContentPage
	{
		List<Tuple<TableSection, List<AccountViewCell>>> Elements;

		public AccountsView()
		{
			InitializeComponent();

			Elements = new List<Tuple<TableSection, List<AccountViewCell>>>();

			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.UpdateAccountsView, async (speed) => await UpdateView());
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdateAccounts, async (str) => await UpdateView());
			MessagingCenter.Subscribe<string>(this, MessageConstants.SortOrderChanged, str => SortHelper.ApplySortOrder(Elements, AccountsTable));
		}

		public async void Add(object sender, EventArgs e)
		{
			var action = await DisplayActionSheet(InternationalisationResources.AddActionChooseTitle, InternationalisationResources.Cancel, null, InternationalisationResources.AddLocalAccount);
			if (InternationalisationResources.AddLocalAccount.Equals(action))
			{
				await Navigation.PushModalAsync(new NavigationPage(new AccountDetailView(null, null) { IsNew = true }));
			}
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
			Elements.Clear();

			foreach (var r in await AccountStorage.Instance.Repositories())
			{
				var cells = new List<AccountViewCell>();
				var section = new TableSection { Title = r.Name };
				foreach (var a in r.Elements)
				{
					cells.Add(new AccountViewCell(Navigation) { Account = a, Repository = r });
				}
				Elements.Add(Tuple.Create(section, cells));
			}
			SortHelper.ApplySortOrder(Elements, AccountsTable);

			setHeader();
		}

		void setHeader()
		{
			var accounts = AccountsTable.Root.Sum(s => s.Count);
			var sources = AccountsTable.Root.Count();

			if (accounts == 0)
			{
				Header.TitleText = InternationalisationResources.NoAccounts;
			}
			else if (accounts == 1)
			{
				Header.TitleText = string.Format("{0} {1}", accounts, InternationalisationResources.Account);
			}
			else {
				Header.TitleText = string.Format("{0} {1}", accounts, InternationalisationResources.Accounts);
			}

			if (sources == 0)
			{
				Header.InfoText = InternationalisationResources.NoSources;
			}
			else if (sources == 1)
			{
				Header.InfoText = string.Format("{0} {1}", sources, InternationalisationResources.Source);
			}
			else {
				Header.InfoText = string.Format("{0} {1}", sources, InternationalisationResources.Sources);
			}
		}
	}
}

