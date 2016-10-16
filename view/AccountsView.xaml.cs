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

namespace view
{
    public partial class AccountsView : ContentPage
    {
        List<Tuple<TableSection, List<AccountViewCell>>> Elements;

        public AccountsView()
        {
            InitializeComponent();

            Elements = new List<Tuple<TableSection, List<AccountViewCell>>>();

            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, async (str) => await updateView());
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedSortOrder, str => SortHelper.ApplySortOrder(Elements, AccountsTable));
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => SortHelper.ApplySortOrder(Elements, AccountsTable));

			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.StartedFetching, speed => Header.IsLoading = true);
			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.DoneFetching, speed => Header.IsLoading = false);
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
            var newPage = new SourcesView(await AccountStorage.Instance.Repositories());
            if (Device.OS == TargetPlatform.Android)
            {
                await Navigation.PushAsync(newPage);
            }
            else
            {
                await Navigation.PushModalAsync(new NavigationPage(newPage));
            }
        }

        public static async Task AddDialog(ContentPage page)
        {
            var action = await page.DisplayActionSheet(InternationalisationResources.AddActionChooseTitle, InternationalisationResources.Cancel, null, InternationalisationResources.AddLocalAccount, InternationalisationResources.AddSource);

            var newPage = (InternationalisationResources.AddLocalAccount.Equals(action)) ? (ContentPage)new AccountDetailView(null, null) { IsNew = true } : new AddRepositoryView();

            if (InternationalisationResources.AddLocalAccount.Equals(action) || InternationalisationResources.AddSource.Equals(action))
            {
                if (Device.OS == TargetPlatform.Android)
                {
                    await page.Navigation.PushAsync(newPage);
                }
                else
                {
                    await page.Navigation.PushModalAsync(new NavigationPage(newPage));
                }
            }
        }

        async Task updateView()
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

