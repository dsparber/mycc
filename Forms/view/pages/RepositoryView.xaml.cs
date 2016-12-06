using System;
using System.Linq;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Account.Repositories.Implementations;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.tasks;
using MyCryptos.Core.Types;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.view.overlays;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
    public partial class RepositoryView
    {
        private readonly AccountRepository repository;

        public RepositoryView(AccountRepository repository)
        {
            InitializeComponent();
            this.repository = repository;

            Header.TitleText = repository.Name;
            Header.InfoText = $"{I18N.Type}: {repository.Description}";

            RepositoryNameEntryCell.Text = repository.Name;
            DeleteButtonCell.Tapped += Delete;

            TableView.Root.Remove(DeleteSection);
            TableView.Root.Remove(NameSection);
            ToolbarItems.Remove(SaveItem);

            RepositoryNameEntryCell.Entry.TextChanged += (sender, e) => Header.TitleText = e.NewTextValue;

            if (Device.OS == TargetPlatform.Android)
            {
                Title = string.Empty;
            }
            //if (!(repository is OnlineAccountRepository))
            //{
            ToolbarItems.Remove(RefreshItem);
            //}

            SetAccountsView();

            Messaging.UpdatingAccounts.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => Header.IsLoading = true), SetAccountsView);
            Messaging.UpdatingAccountsAndRates.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => Header.IsLoading = true), SetAccountsView);
        }

        private void SetAccountsView()
        {
            var cells = repository.Elements.Select(e => new AccountViewCell(Navigation) { Account = e, Repository = repository } as SortableViewCell).OrderBy(e => e.Name).ToList();
            if (cells.Count == 0)
            {
                cells.Add(new CustomViewCell { Text = I18N.NoAccounts });
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                Header.IsLoading = false;
                SortHelper.ApplySortOrder(cells, AccountsSection, SortOrder.None);
            });
        }

        private async void Delete(object sender, EventArgs e)
        {
            UnfocusAll();

            Header.LoadingText = I18N.Deleting;
            Header.IsLoading = true;
            RepositoryNameEntryCell.IsEditable = false;

            await AccountStorage.Instance.Remove(repository);
            Messaging.UpdatingAccounts.SendFinished();
            await Navigation.PopAsync();
        }

        private void EditClicked(object sender, EventArgs e)
        {
            TableView.Root.Add(NameSection);
            if (!(repository is LocalAccountRepository))
            {
                TableView.Root.Add(DeleteSection);
            }
            TableView.Root.Remove(AccountsSection);

            Title = I18N.Editing;
            RepositoryNameEntryCell.IsEditable = true;

            ToolbarItems.Remove(EditItem);
            //ToolbarItems.Remove(RefreshItem);
            ToolbarItems.Add(SaveItem);
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            UnfocusAll();

            SaveItem.Clicked -= SaveClicked;
            Header.IsLoading = true;
            RepositoryNameEntryCell.IsEditable = false;

            repository.Name = RepositoryNameEntryCell.Text ?? string.Empty;
            await AccountStorage.Instance.Update(repository);
            if (repository is OnlineAccountRepository)
            {
                await repository.FetchOnline();
                Messaging.UpdatingAccounts.SendFinished();
            }

            TableView.Root.Add(AccountsSection);
            TableView.Root.Remove(NameSection);
            TableView.Root.Remove(DeleteSection);

            ToolbarItems.Remove(SaveItem);
            ToolbarItems.Add(EditItem);
            //if (repository is OnlineAccountRepository)
            //{
            //	ToolbarItems.Add(RefreshItem);
            //}

            Title = I18N.Accounts;
            Header.IsLoading = false;
            SaveItem.Clicked += SaveClicked;
        }

        private void UnfocusAll()
        {
            RepositoryNameEntryCell.Entry.Unfocus();
        }

        private async void Refresh(object sender, EventArgs e)
        {
            await ApplicationTasks.FetchBalances(repository as OnlineAccountRepository, Messaging.UpdatingAccountsAndRates.SendStarted, Messaging.UpdatingAccountsAndRates.SendFinished, ErrorOverlay.Display);
        }
    }
}
