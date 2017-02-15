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
        private readonly CurrencyEntryCell _currencyEntryCell;

        private FunctionalAccount _account;
        private readonly LocalAccountRepository _repository;


        public AccountEditView(FunctionalAccount account, LocalAccountRepository repository)
        {
            InitializeComponent();

            _account = account;
            _repository = repository;

            Title = account.Name;
            Header.TitleText = account.Money.ToString();
            Header.InfoText = I18N.ManuallyAdded;

            AccountName.Text = account.Name;
            _currencyEntryCell = new CurrencyEntryCell(Navigation) { IsAmountEnabled = true, IsEditable = false, SelectedMoney = account.Money };
            AccountSection.Add(_currencyEntryCell);

            DeleteButtonCell.Tapped += Delete;

            ToolbarItems.Remove(SaveItem);
            EditView.Root.Remove(DeleteSection);

            _currencyEntryCell.OnSelected = c => Header.TitleText = _currencyEntryCell.SelectedMoney.ToString();
            _currencyEntryCell.OnTyped = m => Header.TitleText = m.ToString();
        }

        private void StartEditing(object sender, EventArgs e)
        {
            _currencyEntryCell.IsEditable = true;
            AccountName.IsEditable = true;
            EditView.Root.Add(DeleteSection);

            ToolbarItems.Clear();
            ToolbarItems.Add(SaveItem);

            Title = I18N.Editing;

        }

        private async void DoneEditing(object sender, EventArgs e)
        {
            AccountName.Entry.Unfocus();
            _currencyEntryCell.Unfocus();

            _currencyEntryCell.IsEditable = false;
            AccountName.IsEditable = false;
            EditView.Root.Remove(DeleteSection);

            _account.Name = AccountName.Text ?? string.Empty;

            _account = new LocalAccount(_account.Id, _account.Name, _currencyEntryCell.SelectedMoney, _account.IsEnabled, DateTime.Now, _account.ParentId);
            await _repository.Update(_account);

            Messaging.UpdatingAccounts.SendFinished();


            Title = _account.Name;
            Header.TitleText = _account.Money.ToString();

            ToolbarItems.Clear();
            ToolbarItems.Add(EditItem);
        }

        private async void Delete(object sender, EventArgs e)
        {
            AccountName.Entry.Unfocus();
            _currencyEntryCell.Unfocus();

            await AccountStorage.Instance.LocalRepository.Remove(_account);
            Messaging.UpdatingAccounts.SendFinished();

            await Navigation.PopAsync();
        }
    }
}

