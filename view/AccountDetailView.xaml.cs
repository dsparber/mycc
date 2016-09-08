using System;
using data.repositories.account;
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

		CurrencyEntryCell currencyEntryCell;

		Account account;

		public AccountDetailView(Account account, AccountRepository repository)
		{
			InitializeComponent();

			this.account = account;

			Title = account.Name;
			Header.TitleText = account.Money.ToString();
			Header.InfoText = string.Format(InternationalisationResources.SourceText, repository.Name);

			currencyEntryCell = new CurrencyEntryCell(Navigation);
			currencyEntryCell.SelectedCurrency = account.Money.Currency;
			MoneySection.Add(currencyEntryCell);

			edit.Clicked += StartEditing;
			done.Clicked += DoneEditing;

			ToolbarItems.Add(edit);
		}

		public void StartEditing(object sender, EventArgs e) {
			
			currencyEntryCell.SelectedCurrency = account.Money.Currency;
			AccountName.Text = account.Name;
			AccountValue.Text = account.Money.ToStringWithoutCurrency();

			EditView.IsVisible = true;
			DefaultView.IsVisible = false;
			ToolbarItems.Clear();
			ToolbarItems.Add(done);
			Title = InternationalisationResources.Editing;
		}

		public async void DoneEditing(object sender, EventArgs e)
		{
			account.Name = AccountName.Text;
			account.Money = new Money(decimal.Parse(AccountValue.Text), currencyEntryCell.SelectedCurrency);
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

		async void Delete(object sender, EventArgs e)
		{
			AppTasks.Instance.StartDeleteAccountTask(account);
			await AppTasks.Instance.DeleteAccountTask;
			MessagingCenter.Send(string.Empty, MessageConstants.UpdateAccounts);

			await Navigation.PopAsync();
		}
	}
}

