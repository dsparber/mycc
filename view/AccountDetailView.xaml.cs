﻿using System;
using data.repositories.account;
using data.settings;
using message;
using models;
using MyCryptos.resources;
using tasks;
using view.components;
using Xamarin.Forms;

namespace view
{
	public partial class AccountDetailView : ContentPage
	{
		ToolbarItem edit = new ToolbarItem { Text = InternationalisationResources.Edit };
		ToolbarItem done = new ToolbarItem { Text = InternationalisationResources.Save };
		ToolbarItem cancel = new ToolbarItem { Text = InternationalisationResources.Cancel };

		CurrencyEntryCell currencyEntryCell;

		Account account;

		public bool IsNew;

		Money selectedMoney;

		public AccountDetailView(Account account, AccountRepository repository)
		{
			InitializeComponent();

			this.account = account;

			currencyEntryCell = new CurrencyEntryCell(Navigation);
			currencyEntryCell.IsAmountEnabled = true;
			AccountSection.Add(currencyEntryCell);

			edit.Clicked += StartEditing;
			selectedMoney = new Money(0, ApplicationSettings.BaseCurrency);

			if (!IsNew && account != null)
			{
				setToExistingView(repository);
				done.Clicked += DoneEditing;
				ToolbarItems.Add(edit);
			}
			else {
				setToNewView();
				done.Clicked += Save;
				cancel.Clicked += (sender, e) => Navigation.PopModalAsync();
				ToolbarItems.Add(cancel);
				ToolbarItems.Add(done);
			}

			currencyEntryCell.OnSelected = (c) => Header.TitleText = currencyEntryCell.SelectedMoney.ToString();
			currencyEntryCell.OnTyped = (m) => Header.TitleText = m.ToString();
		}

		public void StartEditing(object sender, EventArgs e)
		{

			AccountName.Text = account.Name;
			currencyEntryCell.SelectedMoney = account.Money;

			EditView.IsVisible = true;
			DefaultView.IsVisible = false;
			ToolbarItems.Clear();
			ToolbarItems.Add(done);
			Title = InternationalisationResources.Editing;
		}

		public async void DoneEditing(object sender, EventArgs e)
		{
			account.Name = AccountName.Text;
			account.Money = currencyEntryCell.SelectedMoney;

			AppTasks.Instance.StartAddAccountTask(account);
			await AppTasks.Instance.AddAccountTask;
			MessagingCenter.Send(string.Empty, MessageConstants.UpdateAccounts);

			Title = account.Name;
			Header.TitleText = account.Money.ToString();

			DefaultView.IsVisible = true;
			EditView.IsVisible = false;
			ToolbarItems.Clear();
			ToolbarItems.Add(edit);
		}

		public void Save(object sender, EventArgs e)
		{
			account = new Account(AccountName.Text, currencyEntryCell.SelectedMoney);

			AppTasks.Instance.StartAddAccountTask(account);
			Navigation.PopModalAsync();
		}

		async void Delete(object sender, EventArgs e)
		{
			AppTasks.Instance.StartDeleteAccountTask(account);
			await AppTasks.Instance.DeleteAccountTask;
			MessagingCenter.Send(string.Empty, MessageConstants.UpdateAccounts);

			await Navigation.PopAsync();
		}

		void setToExistingView(AccountRepository repository)
		{
			Title = account.Name;
			Header.TitleText = account.Money.ToString();
			Header.InfoText = string.Format(InternationalisationResources.SourceText, repository.Name);
			currencyEntryCell.SelectedMoney = account.Money;
		}

		void setToNewView()
		{
			EditView.IsVisible = true;
			DefaultView.IsVisible = false;
			DeleteSection.Clear();
			Title = InternationalisationResources.AddAccountTitle;
			Header.TitleText = selectedMoney.ToString();
			currencyEntryCell.SelectedMoney = selectedMoney;
			AccountName.Completed += (sender, e) => Header.InfoText = AccountName.Text;
		}
	}
}

