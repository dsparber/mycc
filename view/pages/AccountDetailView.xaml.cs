using System;
using System.Collections.Generic;
using data.repositories.account;
using data.settings;
using data.storage;
using message;
using MyCryptos.models;
using MyCryptos.resources;
using tasks;
using Xamarin.Forms;
using MyCryptos.view.components;
using System.Linq;
using MyCryptos.helpers;

namespace view
{
	public partial class AccountDetailView : ContentPage
	{
		ToolbarItem edit = new ToolbarItem { Text = I18N.Edit };
		ToolbarItem done = new ToolbarItem { Text = I18N.Save };
		ToolbarItem cancel = new ToolbarItem { Text = I18N.Cancel };

		CurrencyEntryCell currencyEntryCell;
		public List<ReferenceValueViewCell> ReferenceValueCells;

		Account account;
		Money selectedMoney;
		AccountRepository repository;

		public bool IsNew;

		public AccountDetailView() : this(null, null)
		{
			IsNew = true;
		}

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
			DeleteButtonCell.Tapped += Delete;

			if (!IsNew && account != null)
			{
				ReferenceValueCells = new List<ReferenceValueViewCell>();
				setToExistingView();
				done.Clicked += DoneEditing;
				if (repository is LocalAccountRepository)
				{
					ToolbarItems.Add(edit);
				}

				MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedExchangeRates, str => updateReferenceValues());
				MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => updateReferenceValues());
				MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrencies, str => updateReferenceValues());
			}
			else
			{
				setToNewView();
				done.Clicked += Save;
				cancel.Clicked += (sender, e) => Navigation.PopOrPopModal();
				if (Device.OS != TargetPlatform.Android)
				{
					ToolbarItems.Add(cancel);
				}
				ToolbarItems.Add(done);
			}

			currencyEntryCell.OnSelected = (c) => Header.TitleText = currencyEntryCell.SelectedMoney.ToString();
			currencyEntryCell.OnTyped = (m) => Header.TitleText = m.ToString();

			if (Device.OS == TargetPlatform.Android)
			{
				Title = string.Empty;
			}
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
			if (Device.OS != TargetPlatform.Android)
			{
				Title = I18N.Editing;
			}
		}

		public async void DoneEditing(object sender, EventArgs e)
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

			DefaultView.IsVisible = true;
			EditView.IsVisible = false;
			ToolbarItems.Clear();
			ToolbarItems.Add(edit);
		}

		public void Save(object sender, EventArgs e)
		{
			AccountName.Entry.Unfocus();
			currencyEntryCell.Unfocus();

			var money = currencyEntryCell.SelectedMoney;
			var name = (AccountName.Text ?? I18N.LocalAccount).Trim();

			account = new Account(name, money);
			account.RepositoryId = AccountStorage.Instance.LocalRepository.Id;

			AppTasks.Instance.StartAddAccountTask(account);

			Navigation.PopOrPopModal();
		}

		async void Delete(object sender, EventArgs e)
		{
			AccountName.Entry.Unfocus();
			currencyEntryCell.Unfocus();

			AppTasks.Instance.StartDeleteAccountTask(account);
			await AppTasks.Instance.DeleteAccountTask;
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

			await Navigation.PopAsync();
		}

		void setToExistingView()
		{
			if (Device.OS != TargetPlatform.Android)
			{
				Title = account.Name;
			}
			Header.InfoText = string.Format(I18N.SourceText, repository.Name);
			currencyEntryCell.SelectedMoney = account.Money;

			updateReferenceValues();
		}

		void updateReferenceValues()
		{
			var table = new ReferenceCurrenciesSection(account.Money);
			ReferenceValueCells.Clear();
			EqualsSection.Clear();
			foreach (var cell in table.Cells)
			{
				ReferenceValueCells.Add(cell);
				EqualsSection.Add(cell);
			}
			Header.TitleText = account.Money.ToString();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (!IsNew && account != null)
			{
				var neededRates = ReferenceValueCells.Where(c => c.IsLoading).Select(c => c.ExchangeRate);
				AppTasks.Instance.StartMissingRatesTask(neededRates);
			}
		}

		void setToNewView()
		{
			EditView.IsVisible = true;
			DefaultView.IsVisible = false;
			DeleteSection.Clear();
			if (Device.OS != TargetPlatform.Android)
			{
				Title = I18N.AddAccountTitle;
			}
			Header.TitleText = selectedMoney.ToString();
			currencyEntryCell.SelectedMoney = selectedMoney;
			Header.InfoText = I18N.LocalAccount;
			AccountName.Entry.Placeholder = I18N.LocalAccount;
			AccountName.Entry.TextChanged += (sender, e) => Header.InfoText = (e.NewTextValue.Length != 0) ? e.NewTextValue : I18N.LocalAccount;
		}
	}
}

