using System;
using System.Linq;
using MyCryptos.Core.Constants;
using MyCryptos.Core.Enums;
using MyCryptos.Core.Repositories.Account;
using MyCryptos.Core.Storage;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Resources;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace view
{
    public partial class RepositoryView : ContentPage
    {
        AccountRepository repository;

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

            SetAccountsView();

            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, str => SetAccountsView());
            MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.StartedFetching, speed => Header.IsLoading = true);
            MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.DoneFetching, speed => Header.IsLoading = false);
        }

        private void SetAccountsView()
        {
            var cells = repository.Elements.Select(e => new AccountViewCell(Navigation) { Account = e, Repository = repository } as SortableViewCell).OrderBy(e => e.Name).ToList();
            if (repository is LocalAccountRepository)
            {
                var actionCell = new CustomViewCell { Text = I18N.AddLocalAccount, IsActionCell = true };
                actionCell.Tapped += (sender, e) => Navigation.PushOrPushModal(new AddSourceView(true));
                cells.Add(actionCell);
            }
            else if (cells.Count == 0)
            {
                cells.Add(new CustomViewCell { Text = I18N.NoAccounts });
            }
            SortHelper.ApplySortOrder(cells, AccountsSection, SortOrder.NONE);
        }

        private async void Delete(object sender, EventArgs e)
        {
            UnfocusAll();

            Header.LoadingText = I18N.Deleting;
            Header.IsLoading = true;
            RepositoryNameEntryCell.IsEditable = false;

            await AccountStorage.Instance.Remove(repository);
            MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);
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
            await AccountStorage.Instance.Fetch();
            MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

            TableView.Root.Add(AccountsSection);
            TableView.Root.Remove(NameSection);
            TableView.Root.Remove(DeleteSection);

            ToolbarItems.Remove(SaveItem);
            ToolbarItems.Add(EditItem);

            Title = I18N.Accounts;
            Header.IsLoading = false;
            SaveItem.Clicked += SaveClicked;
        }

        private void UnfocusAll()
        {
            RepositoryNameEntryCell.Entry.Unfocus();
        }
    }
}
