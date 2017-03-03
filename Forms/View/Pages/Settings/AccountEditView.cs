using System;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages.Settings
{
    public partial class AccountEditView
    {
        private readonly CurrencyEntryCell _currencyEntryCell;

        private FunctionalAccount _account;
        private readonly LocalAccountRepository _repository;

        private readonly bool _isEditModal;

        public AccountEditView(FunctionalAccount account, LocalAccountRepository repository, bool isEditModal = false)
        {
            InitializeComponent();

            _account = account;
            _repository = repository;
            _isEditModal = isEditModal;

            Title = account.Name;
            Header.TitleText = account.Money.ToString();
            Header.InfoText = I18N.ManuallyAdded;

            AccountName.Text = account.Name;
            _currencyEntryCell = new CurrencyEntryCell(Navigation) { IsAmountEnabled = true, IsEditable = false, SelectedMoney = account.Money };
            AccountSection.Add(_currencyEntryCell);

            DeleteButtonCell.Tapped += Delete;

            ToolbarItems.Remove(SaveItem);
            EditView.Root.Remove(DeleteSection);
            EditView.Root.Remove(EnableSection);

            _currencyEntryCell.OnSelected = c => Header.TitleText = _currencyEntryCell.SelectedMoney.ToString();
            _currencyEntryCell.OnTyped = m => Header.TitleText = m.ToString();

            EnableAccountCell.On = account.IsEnabled;

            if (!isEditModal) return;

            StartEditing(null, null);
            var cancel = new ToolbarItem { Text = I18N.Cancel };
            cancel.Clicked += (s, e) => Navigation.PopOrPopModal();
            ToolbarItems.Add(cancel);
        }

        private void StartEditing(object sender, EventArgs e)
        {
            _currencyEntryCell.IsEditable = true;
            AccountName.IsEditable = true;
            EditView.Root.Add(EnableSection);
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

            _account.Name = AccountName.Text ?? string.Empty;
            _account.IsEnabled = EnableAccountCell.On;

            _account = new LocalAccount(_account.Id, _account.Name, _currencyEntryCell.SelectedMoney, _account.IsEnabled, DateTime.Now, _account.ParentId);
            await _repository.Update(_account);

            Messaging.UpdatingAccounts.SendFinished();

            if (_isEditModal) await Navigation.PopOrPopModal();

            EditView.Root.Remove(DeleteSection);
            EditView.Root.Remove(EnableSection);

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

            if (_isEditModal) await Navigation.PopOrPopModal();
            else await Navigation.PopAsync();
        }
    }
}

