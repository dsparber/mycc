using System;
using MyCC.Core.Account.Models.Base;
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

		private readonly FunctionalAccount _account;

		private readonly bool _isEditModal;

		public AccountEditView(FunctionalAccount account, bool isEditModal = false)
		{
			InitializeComponent();

			_account = account;
			_isEditModal = isEditModal;

			Title = account.Name;
			Header.TitleText = account.Money.ToString();
			Header.InfoText = I18N.ManuallyAdded;

			AccountName.Text = account.Name;
			AmountEntry.Text = account.Money.Amount.ToString();
			AmountEntry.Entry.Keyboard = Keyboard.Numeric;
			_currencyEntryCell = new CurrencyEntryCell(Navigation) { IsAmountEnabled = false, IsFormRepresentation = true, IsEditable = false, SelectedCurrency = account.Money.Currency };
			AccountSection.Add(_currencyEntryCell);

			AmountEntry.Entry.TextChanged += (sender, e) =>
			{
				var entry = (Entry)sender;
				var val = entry.Text;

#pragma warning disable 168
				decimal n;
				if (val.Length == 0 || decimal.TryParse(val, out n)) return;
#pragma warning restore 168

				val = val.Remove(val.Length - 1);
				entry.Text = val;
			};

			DeleteButtonCell.Tapped += Delete;

			ToolbarItems.Remove(SaveItem);
			EditView.Root.Remove(DeleteSection);

			_currencyEntryCell.OnSelected = c => Header.TitleText = _currencyEntryCell.SelectedMoney.ToString();
			_currencyEntryCell.OnTyped = m => Header.TitleText = m.ToString();

			EnableAccountCell.On = account.IsEnabled;
			EnableAccountCell.Switch.IsEnabled = false;

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
			AmountEntry.IsEditable = true;
			EnableAccountCell.Switch.IsEnabled = true;

			EditView.Root.Add(DeleteSection);

			ToolbarItems.Clear();
			ToolbarItems.Add(SaveItem);

			Title = I18N.Editing;
		}

		private async void DoneEditing(object sender, EventArgs e)
		{
			AccountName.Entry.Unfocus();
			AmountEntry.Entry.Unfocus();
			_currencyEntryCell.Unfocus();

			_currencyEntryCell.IsEditable = false;
			AccountName.IsEditable = false;
			AmountEntry.IsEditable = false;
			EnableAccountCell.Switch.IsEnabled = false;
			EditView.Root.Remove(DeleteSection);

			decimal amount;
			amount = decimal.TryParse(AmountEntry.Text, out amount) ? amount : 0;
			_account.Money = new Money(amount, _currencyEntryCell.SelectedCurrency);
			_account.LastUpdate = DateTime.Now;
			_account.Name = string.IsNullOrWhiteSpace(AccountName.Text) ? I18N.Unnamed : AccountName.Text;
			_account.IsEnabled = EnableAccountCell.On;

			await AccountStorage.Update(_account);

			Messaging.UpdatingAccounts.SendFinished();

			if (_isEditModal) await Navigation.PopOrPopModal();


			Title = _account.Name;
			Header.TitleText = _account.Money.ToString();

			ToolbarItems.Clear();
			ToolbarItems.Add(EditItem);
		}

		private async void Delete(object sender, EventArgs e)
		{
			AccountName.Entry.Unfocus();
			AmountEntry.Entry.Unfocus();
			_currencyEntryCell.Unfocus();

			await AccountStorage.Instance.LocalRepository.Remove(_account);
			Messaging.UpdatingAccounts.SendFinished();

			if (_isEditModal) await Navigation.PopOrPopModal();
			else await Navigation.PopAsync();
		}
	}
}

