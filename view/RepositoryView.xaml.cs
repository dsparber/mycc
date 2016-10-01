using System;
using data.database.models;
using data.repositories.account;
using data.storage;
using message;
using MyCryptos.resources;
using Xamarin.Forms;

namespace view
{
	public partial class RepositoryView : ContentPage
	{
		ToolbarItem saveItem;

		AccountRepository repository;

		public RepositoryView(AccountRepository repository)
		{
			InitializeComponent();
			this.repository = repository;

			Title = repository.Name;
			Header.TitleText = repository.Name;
			Header.InfoText = string.Format("{0}: {1}", InternationalisationResources.RepositoryType, repository.Type);
			RepositoryNameEntryCell.Text = repository.Name;

			saveItem = new ToolbarItem { Text = InternationalisationResources.Save };
			saveItem.Clicked += save;

			if (repository is LocalAccountRepository)
			{
				TableView.Root.Remove(DeleteSection);
			}

			RepositoryNameEntryCell.Entry.TextChanged += (sender, e) =>
			{
				Title = e.NewTextValue;
				Header.TitleText = e.NewTextValue;

				if (!e.NewTextValue.Equals(repository.Name))
				{
					if (ToolbarItems.Count == 0)
					{
						ToolbarItems.Add(saveItem);
					}
				}
				else {
					ToolbarItems.Clear();
				}
			};
		}

		async void save(object sender, EventArgs e)
		{
			Header.IsLoading = true;
			DeleteButton.IsEnabled = false;
			RepositoryNameEntryCell.IsEditable = false;

			await AccountStorage.Instance.Remove(new AccountRepositoryDBM(repository));
			repository.Name = RepositoryNameEntryCell.Text ?? string.Empty;
			await AccountStorage.Instance.Add(new AccountRepositoryDBM(repository));
			await AccountStorage.Instance.Fetch();
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);
			await Navigation.PopAsync();
		}

		async void Delete(object sender, EventArgs e)
		{
			Header.LoadingText = InternationalisationResources.Deleting;
			Header.IsLoading = true;
			DeleteButton.IsEnabled = false;
			RepositoryNameEntryCell.IsEditable = false;

			await AccountStorage.Instance.Remove(new AccountRepositoryDBM(repository));
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);
			await Navigation.PopAsync();
		}
	}
}
