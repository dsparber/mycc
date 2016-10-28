using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.storage;
using enums;
using helpers;
using message;
using MyCryptos.resources;
using MyCryptos.view.components;
using Xamarin.Forms;
using MyCryptos.helpers;
using System.Diagnostics;

namespace view
{
	public partial class AccountsView : ContentPage
	{
		List<Tuple<TableSection, List<SortableViewCell>>> Elements;

		public AccountsView()
		{
			InitializeComponent();

			Elements = new List<Tuple<TableSection, List<SortableViewCell>>>();

			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, async str =>
			{
				try
				{
					await updateView();
				}
				catch (Exception e)
				{
					Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
				}
			});
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedSortOrder, str => SortHelper.ApplySortOrder(Elements, AccountsTable));
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => SortHelper.ApplySortOrder(Elements, AccountsTable));

			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.StartedFetching, speed => Header.IsLoading = true);
			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.DoneFetching, speed => Header.IsLoading = false);

			if (Device.OS == TargetPlatform.Android)
			{
				ToolbarItems.Remove(SourcesToolbarItem);
				Title = string.Empty;
			}
		}

		public async void Add(object sender, EventArgs e)
		{
			await AddDialog(this);
		}

		public async void SourcesClicked(object sender, EventArgs e)
		{
			await OpenSourcesView(Navigation);
		}

		public static async Task OpenSourcesView(INavigation Navigation)
		{
			var page = new SourcesView(await AccountStorage.Instance.Repositories());

			await Navigation.PushOrPushModal(page);
		}

		public static async Task AddDialog(ContentPage page)
		{
			var action = await page.DisplayActionSheet(InternationalisationResources.AddActionChooseTitle, InternationalisationResources.Cancel, null, InternationalisationResources.AddLocalAccount, InternationalisationResources.AddSource);

			var newPage = (InternationalisationResources.AddLocalAccount.Equals(action)) ? (ContentPage)new AccountDetailView(null, null) { IsNew = true } : new AddRepositoryView();

			if (InternationalisationResources.AddLocalAccount.Equals(action) || InternationalisationResources.AddSource.Equals(action))
			{
				await page.Navigation.PushOrPushModal(newPage);
			}
		}

		async Task updateView()
		{
			Elements.Clear();

			foreach (var r in await AccountStorage.Instance.Repositories())
			{
				var cells = new List<SortableViewCell>();
				var section = new TableSection { Title = r.Name };
				foreach (var a in r.Elements)
				{
					cells.Add(new AccountViewCell(Navigation) { Account = a, Repository = r });
				}
				if (cells.Count == 0)
				{
					cells.Add(new CustomViewCell { Text = InternationalisationResources.NoAccounts });
				}
				Elements.Add(Tuple.Create(section, cells));
			}
			SortHelper.ApplySortOrder(Elements, AccountsTable);

			setHeader();
		}

		void setHeader()
		{
			var accounts = AccountsTable.Root.Where(s => s.Count > 0 && s.ElementAt(0) is AccountViewCell).Sum(s => s.Count);
			var sources = AccountsTable.Root.Count();

			if (accounts == 0)
			{
				Header.TitleText = InternationalisationResources.NoAccounts;
			}
			else if (accounts == 1)
			{
				Header.TitleText = string.Format("{0} {1}", accounts, InternationalisationResources.Account);
			}
			else
			{
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
			else
			{
				Header.InfoText = string.Format("{0} {1}", sources, InternationalisationResources.Sources);
			}
		}
	}
}

