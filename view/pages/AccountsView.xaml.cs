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
using data.repositories.account;

namespace view
{
	public partial class AccountsView : ContentPage
	{
		List<Tuple<TableSection, List<SortableViewCell>>> Elements;

		public AccountsView()
		{
			InitializeComponent();

			Elements = new List<Tuple<TableSection, List<SortableViewCell>>>();

			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, str => updateView());
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedSortOrder, str => SortHelper.ApplySortOrder(Elements, AccountsTable));
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => SortHelper.ApplySortOrder(Elements, AccountsTable));

			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.StartedFetching, speed => setLoadingAnimation(speed, true));
			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.DoneFetching, speed => setLoadingAnimation(speed, false));

			if (Device.OS == TargetPlatform.Android)
			{
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
			var page = new SourcesView();

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

		void updateView()
		{
			Elements.Clear();

			foreach (var r in AccountStorage.Instance.Repositories.GroupBy(r => r.Description))
			{
				var cells = new List<SortableViewCell>();
				var section = new TableSection { Title = r.FirstOrDefault().Description };
				foreach (var a in r.SelectMany(x => x.Elements))
				{
					cells.Add(new AccountViewCell(Navigation) { Account = a, Repository = r.FirstOrDefault() });
				}
				if (cells.Count == 0)
				{
                    if (r.FirstOrDefault() is LocalAccountRepository)
                    {
                        var localAccountCell = new CustomViewCell { Text = InternationalisationResources.AddLocalAccount, IsActionCell = true };
                        localAccountCell.Tapped += (sender, e) => Navigation.PushOrPushModal(new AccountDetailView(null, null) { IsNew = true });
                        cells.Add(localAccountCell);
                    }
                    else {
                        cells.Add(new CustomViewCell { Text = InternationalisationResources.NoAccounts });
                    }
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
				Header.TitleText = InternationalisationResources.OneAccount;
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
				Header.InfoText = InternationalisationResources.OneSource;
            }
			else
			{
				Header.InfoText = string.Format("{0} {1}", sources, InternationalisationResources.Sources);
			}
		}

		void setLoadingAnimation(FetchSpeed speed, bool loading)
		{
            // TODO Online accounts not loaded from db
			if (speed.Speed == FetchSpeedEnum.SLOW)
			{
				IsBusy = loading;
			}
			else {
				Header.IsLoading = loading;
			}
		}
	}
}

