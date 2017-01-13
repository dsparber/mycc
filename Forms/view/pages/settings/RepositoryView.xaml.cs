using System;
using System.Linq;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Account.Repositories.Implementations;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.Types;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.Tasks;
using MyCryptos.view.components;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
	public partial class RepositoryView
	{
		private readonly AccountRepository repository;

		public RepositoryView(AccountRepository repository)
		{
			InitializeComponent();
			this.repository = repository;

			Header.TitleText = repository is LocalAccountRepository ? I18N.ManuallyAdded : repository.Name;
			if (repository is AddressAccountRepository)
			{
				Header.InfoText = $"{I18N.Source}: {repository.Description}";
			}
			else
			{
				Header.InfoText = PluralHelper.GetTextCoins(repository.ElementsCount);
				GeneralSection.Remove(AddressEntryCell);
			}

			if (repository is LocalAccountRepository)
			{
				TableView.Root.Remove(GeneralSection);
				ToolbarItems.Remove(EditItem);
			}

			RepositoryNameEntryCell.Text = repository.Name;
			DeleteButtonCell.Tapped += Delete;

			TableView.Root.Remove(DeleteSection);
			ToolbarItems.Remove(SaveItem);

			RepositoryNameEntryCell.Entry.TextChanged += (sender, e) => Header.TitleText = e.NewTextValue;

			if (Device.OS == TargetPlatform.Android)
			{
				Title = string.Empty;
			}

			SetView();

			Messaging.UpdatingAccounts.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => Header.IsLoading = true), SetView);
			Messaging.UpdatingAccountsAndRates.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => Header.IsLoading = true), SetView);
		}

		private void SetView()
		{
			var cells = repository.Elements.OrderBy(e => e.Money.Currency.Code).Select(e =>
			{
				var cell = new CustomViewCell { Text = e.Money.ToString(), Detail = e.Money.Currency.Name };
				if (repository is LocalAccountRepository)
				{
					cell.Image = "more.png";
					cell.Detail = e.Name;
					cell.Tapped += (sender, eventArgs) => Navigation.PushAsync(new AccountEditView(e, repository as LocalAccountRepository));
				}
				return cell;
			}).ToList();

			if (cells.Count == 0)
			{
				cells.Add(new CustomViewCell
				{
					Text = I18N.NoAccounts
				});
			}
			Device.BeginInvokeOnMainThread(() =>
			{
				Header.IsLoading = false;
				SortHelper.ApplySortOrder(cells, AccountsSection, SortOrder.None, SortDirection.Ascending);
			});

			if (repository is AddressAccountRepository)
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					AddressEntryCell.Text = (repository as AddressAccountRepository).Address;
					Header.IsLoading = false;
				});
			}
		}

		private async void Delete(object sender, EventArgs e)
		{
			UnfocusAll();

			Header.LoadingText = I18N.Deleting;
			Header.IsLoading = true;
			RepositoryNameEntryCell.IsEditable = false;

			await AccountStorage.Instance.Remove(repository);
			Messaging.UpdatingAccounts.SendFinished();
			await Navigation.PopAsync();
		}

		private void EditClicked(object sender, EventArgs e)
		{
			RepositoryNameEntryCell.IsEditable = true;
			if (!(repository is LocalAccountRepository))
			{
				TableView.Root.Add(DeleteSection);
			}
			TableView.Root.Remove(AccountsSection);

			Title = I18N.Editing;
			RepositoryNameEntryCell.IsEditable = true;
			AddressEntryCell.IsEditable = true;

			ToolbarItems.Remove(EditItem);
			ToolbarItems.Add(SaveItem);
		}

		private async void SaveClicked(object sender, EventArgs e)
		{
			UnfocusAll();

			SaveItem.Clicked -= SaveClicked;
			Header.IsLoading = true;
			RepositoryNameEntryCell.IsEditable = false;
			AddressEntryCell.IsEditable = false;

			Action revertChanges = () => { };
			var success = true;
			var changed = false;

			repository.Name = RepositoryNameEntryCell.Text ?? string.Empty;
			if (repository is AddressAccountRepository)
			{
				var addressReposiotry = repository as AddressAccountRepository;
				var oldAddress = addressReposiotry.Address;
				var addressText = AddressEntryCell.Text ?? string.Empty;
				if (!addressText.Equals(addressReposiotry.Address))
				{
					addressReposiotry.Address = addressText;
					changed = true;
				}
				revertChanges = () => addressReposiotry.Address = oldAddress;
			}
			else if (repository is BittrexAccountRepository)
			{
				// TODO Api Keys
			}

			if (changed && repository is OnlineAccountRepository)
			{
				var successful = await AccountStorage.AddRepository(repository as OnlineAccountRepository);
				if (successful)
				{
					await AppTaskHelper.FetchMissingRates();
				}
				else
				{
					await DisplayAlert(I18N.Error, I18N.FetchingNoSuccessText, I18N.Ok);
					success = false;

					revertChanges();
				}
			}

			if (success)
			{

				await AccountStorage.Instance.Update(repository);
				Messaging.UpdatingAccounts.SendFinished();

				RepositoryNameEntryCell.IsEditable = false;
				AddressEntryCell.IsEditable = false;

				TableView.Root.Add(AccountsSection);
				TableView.Root.Remove(DeleteSection);

				ToolbarItems.Remove(SaveItem);
				ToolbarItems.Add(EditItem);

				Title = I18N.Accounts;
			}
			else {
				RepositoryNameEntryCell.IsEditable = true;
				AddressEntryCell.IsEditable = true;
			}
			SaveItem.Clicked += SaveClicked;
			Header.IsLoading = false;
		}

		private void UnfocusAll()
		{
			RepositoryNameEntryCell.Entry.Unfocus();
		}
	}
}