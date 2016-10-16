﻿using System;
using System.Collections.Generic;
using data.repositories.account;
using data.settings;
using data.storage;
using helpers;
using message;
using models;
using MyCryptos.resources;
using tasks;
using Xamarin.Forms;
using MyCryptos.view.components;
using System.Threading.Tasks;
using System.Linq;

namespace view
{
    public partial class AccountDetailView : ContentPage
    {
        ToolbarItem edit = new ToolbarItem { Text = InternationalisationResources.Edit };
        ToolbarItem done = new ToolbarItem { Text = InternationalisationResources.Save };
        ToolbarItem cancel = new ToolbarItem { Text = InternationalisationResources.Cancel };

        CurrencyEntryCell currencyEntryCell;
        public List<ReferenceValueViewCell> ReferenceValueCells;

        Account account;
        Money selectedMoney;
        AccountRepository repository;

        public bool IsNew;

        public AccountDetailView(Account account, AccountRepository repository)
        {
            InitializeComponent();

            this.account = account;
            this.repository = repository;

            currencyEntryCell = new CurrencyEntryCell(Navigation);
            currencyEntryCell.IsAmountEnabled = true;
            AccountSection.Add(currencyEntryCell);

            edit.Clicked += StartEditing;
            selectedMoney = new Money(0, ApplicationSettings.BaseCurrency);

            if (!IsNew && account != null)
            {
                ReferenceValueCells = new List<ReferenceValueViewCell>();
                setToExistingView();
                done.Clicked += DoneEditing;
                ToolbarItems.Add(edit);

                MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedSortOrder, str => SortHelper.ApplySortOrder(ReferenceValueCells, EqualsSection));
                MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedExchangeRates, str => updateReferenceValues());
                MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => updateReferenceValues());
            }
            else
            {
                setToNewView();
                done.Clicked += Save;
                cancel.Clicked += (sender, e) => Navigation.PopModalAsync();
                if (Device.OS != TargetPlatform.Android)
                {
                    ToolbarItems.Add(cancel);
                }
                ToolbarItems.Add(done);
            }

            currencyEntryCell.OnSelected = (c) => Header.TitleText = currencyEntryCell.SelectedMoney.ToString();
            currencyEntryCell.OnTyped = (m) => Header.TitleText = m.ToString();
        }

        public void StartEditing(object sender, EventArgs e)
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
            Title = InternationalisationResources.Editing;
        }

        public async void DoneEditing(object sender, EventArgs e)
        {
            repository.Elements.Remove(account);
            account.Name = AccountName.Text;
            account.Money = currencyEntryCell.SelectedMoney;
            await repository.Add(account);

            MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

            Title = account.Name;
            Header.TitleText = account.Money.ToString();

            DefaultView.IsVisible = true;
            EditView.IsVisible = false;
            ToolbarItems.Clear();
            ToolbarItems.Add(edit);
        }

        public async void Save(object sender, EventArgs e)
        {
            var money = currencyEntryCell.SelectedMoney;
            money = new Money(money.Amount, (await CurrencyStorage.Instance.AllElements()).Find(c => c.Equals(money.Currency)));
            account = new Account(AccountName.Text, money);

            AppTasks.Instance.StartAddAccountTask(account);
            await Navigation.PopModalAsync();
        }

        async void Delete(object sender, EventArgs e)
        {
            AppTasks.Instance.StartDeleteAccountTask(account);
            await AppTasks.Instance.DeleteAccountTask;
            MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

            await Navigation.PopAsync();
        }

        void setToExistingView()
        {
            Title = account.Name;
            Header.TitleText = account.Money.ToString();
            Header.InfoText = string.Format(InternationalisationResources.SourceText, repository.Name);
            currencyEntryCell.SelectedMoney = account.Money;

            updateReferenceValues();
        }

        void updateReferenceValues()
        {
            var table = new ReferenceCurrenciesTableView { BaseMoney = account.Money };
            ReferenceValueCells.Clear();
            foreach (var cell in table.Cells)
            {
                ReferenceValueCells.Add(cell);
            }
            SortHelper.ApplySortOrder(ReferenceValueCells, EqualsSection);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!IsNew && account != null)
            {
				await Task.WhenAll(ReferenceValueCells.Select(async c => await c.Update()));
            }
        }

        void setToNewView()
        {
            EditView.IsVisible = true;
            DefaultView.IsVisible = false;
            DeleteSection.Clear();
            Title = InternationalisationResources.AddAccountTitle;
            Header.TitleText = selectedMoney.ToString();
            currencyEntryCell.SelectedMoney = selectedMoney;
            AccountName.Text = InternationalisationResources.LocalAccount;
            Header.InfoText = AccountName.Text;
            AccountName.Entry.TextChanged += (sender, e) => Header.InfoText = e.NewTextValue;
        }
    }
}

