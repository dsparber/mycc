using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Constants;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.Account;
using MyCryptos.Core.Storage;
using MyCryptos.Core.Tasks;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Resources;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace MyCryptos.view.pages
{
    public partial class AccountDetailView
    {
        private readonly ToolbarItem edit = new ToolbarItem { Text = I18N.Edit };
        private readonly ToolbarItem done = new ToolbarItem { Text = I18N.Save };

        private readonly CurrencyEntryCell currencyEntryCell;
        private readonly List<ReferenceValueViewCell> referenceValueCells;

        private Account account;
        private readonly AccountRepository repository;


        public AccountDetailView(Account account, AccountRepository repository)
        {
            InitializeComponent();

            this.account = account;
            this.repository = repository;

            currencyEntryCell = new CurrencyEntryCell(Navigation) { IsAmountEnabled = true };
            AccountSection.Add(currencyEntryCell);

            edit.Clicked += StartEditing;
            DeleteButtonCell.Tapped += Delete;


            referenceValueCells = new List<ReferenceValueViewCell>();
            SetToExistingView();
            done.Clicked += DoneEditing;
            if (repository is LocalAccountRepository)
            {
                ToolbarItems.Add(edit);
            }

            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedExchangeRates, str => UpdateReferenceValues());
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => UpdateReferenceValues());
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrencies, str => UpdateReferenceValues());


            currencyEntryCell.OnSelected = (c) => Header.TitleText = currencyEntryCell.SelectedMoney.ToString();
            currencyEntryCell.OnTyped = (m) => Header.TitleText = m.ToString();

            if (Device.OS == TargetPlatform.Android)
            {
                Title = string.Empty;
            }
        }

        private void StartEditing(object sender, EventArgs e)
        {
            AccountName.Text = account.Name;
            currencyEntryCell.SelectedMoney = account.Money;

            var isLocal = repository is LocalAccountRepository;

            currencyEntryCell.IsEditable = isLocal;
            if (!isLocal)
            {
                EditView.Root.Remove(DeleteSection);
            }

            EditView.IsVisible = true;
            DefaultView.IsVisible = false;
            ToolbarItems.Clear();
            ToolbarItems.Add(done);
            if (Device.OS != TargetPlatform.Android)
            {
                Title = I18N.Editing;
            }
        }

        private async void DoneEditing(object sender, EventArgs e)
        {
            AccountName.Entry.Unfocus();
            currencyEntryCell.Unfocus();

            account.Name = AccountName.Text;
            account = new Account(account.Id, account.RepositoryId, account.Name, currencyEntryCell.SelectedMoney);
            await repository.Update(account);

            MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

            if (Device.OS != TargetPlatform.Android)
            {
                Title = account.Name;
            }
            Header.TitleText = account.Money.ToString();

            UpdateReferenceValues();
            DefaultView.IsVisible = true;
            EditView.IsVisible = false;
            ToolbarItems.Clear();
            ToolbarItems.Add(edit);
        }

        private void Save(object sender, EventArgs e)
        {
            AccountName.Entry.Unfocus();
            currencyEntryCell.Unfocus();

            var money = currencyEntryCell.SelectedMoney;
            var name = string.IsNullOrEmpty(AccountName.Text?.Trim()) ? I18N.LocalAccount : AccountName.Text.Trim();

            account = new Account(name, money) { RepositoryId = AccountStorage.Instance.LocalRepository.Id };

            AppTasks.Instance.StartAddAccountTask(account);

            Navigation.PopOrPopModal();
        }

        private async void Delete(object sender, EventArgs e)
        {
            AccountName.Entry.Unfocus();
            currencyEntryCell.Unfocus();

            AppTasks.Instance.StartDeleteAccountTask(account);
            await AppTasks.Instance.DeleteAccountTask;
            MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

            await Navigation.PopAsync();
        }

        private void SetToExistingView()
        {
            if (Device.OS != TargetPlatform.Android)
            {
                Title = account.Name;
            }
            Header.InfoText = string.Format(I18N.SourceText, repository.Name);
            currencyEntryCell.SelectedMoney = account.Money;

            UpdateReferenceValues();
        }

        private void UpdateReferenceValues()
        {
            var table = new ReferenceCurrenciesSection(account.Money);
            referenceValueCells.Clear();
            EqualsSection.Clear();
            foreach (var cell in table.Cells)
            {
                referenceValueCells.Add(cell);
                EqualsSection.Add(cell);
            }
            Header.TitleText = account.Money.ToString();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (account == null) return;

            var neededRates = referenceValueCells.Where(c => c.IsLoading).Select(c => c.ExchangeRate);
            AppTasks.Instance.StartMissingRatesTask(neededRates);
        }
    }
}

