using System;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;

namespace MyCC.Forms.View.Pages.Settings
{
    public partial class AccountEditView
    {
        private readonly CurrencyEntryCell currencyEntryCell;

        private FunctionalAccount account;
        private readonly LocalAccountRepository repository;


        public AccountEditView(FunctionalAccount account, LocalAccountRepository repository)
        {
            InitializeComponent();

            this.account = account;
            this.repository = repository;

            Title = account.Name;
            Header.TitleText = account.Money.ToString();
            Header.InfoText = I18N.ManuallyAdded;

            AccountName.Text = account.Name;
            currencyEntryCell = new CurrencyEntryCell(Navigation) { IsAmountEnabled = true, IsEditable = false, SelectedMoney = account.Money };
            AccountSection.Add(currencyEntryCell);

            DeleteButtonCell.Tapped += Delete;

            ToolbarItems.Remove(SaveItem);
            EditView.Root.Remove(DeleteSection);

            currencyEntryCell.OnSelected = (c) => Header.TitleText = currencyEntryCell.SelectedMoney.ToString();
            currencyEntryCell.OnTyped = (m) => Header.TitleText = m.ToString();
        }

        private void StartEditing(object sender, EventArgs e)
        {
            currencyEntryCell.IsEditable = true;
            AccountName.IsEditable = true;
            EditView.Root.Add(DeleteSection);

            ToolbarItems.Clear();
            ToolbarItems.Add(SaveItem);

            Title = I18N.Editing;

        }

        private async void DoneEditing(object sender, EventArgs e)
        {
            AccountName.Entry.Unfocus();
            currencyEntryCell.Unfocus();

            currencyEntryCell.IsEditable = false;
            AccountName.IsEditable = false;
            EditView.Root.Remove(DeleteSection);

            account.Name = AccountName.Text ?? string.Empty;

            account = new LocalAccount(account.Id, account.Name, currencyEntryCell.SelectedMoney, account.IsEnabled, account.ParentId);
            await repository.Update(account);

            Messaging.UpdatingAccounts.SendFinished();


            Title = account.Name;
            Header.TitleText = account.Money.ToString();

            ToolbarItems.Clear();
            ToolbarItems.Add(EditItem);
        }

        private async void Delete(object sender, EventArgs e)
        {
            AccountName.Entry.Unfocus();
            currencyEntryCell.Unfocus();

            await AccountStorage.Instance.LocalRepository.Remove(account);
            Messaging.UpdatingAccounts.SendFinished();

            await Navigation.PopAsync();
        }
    }
}

