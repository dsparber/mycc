using System;
using data.database.models;
using data.repositories.account;
using data.storage;
using message;
using MyCryptos.resources;
using Xamarin.Forms;

namespace view
{
	public partial class AddRepositoryView : ContentPage
	{
		public AddRepositoryView()
		{
			InitializeComponent();
			Title = InternationalisationResources.AddRepositoryTitle;
			Header.TitleText = InternationalisationResources.Bittrex;
			Header.InfoText = InternationalisationResources.AddSource;
			Header.LoadingText = InternationalisationResources.Testing;
			RepositoryNameEntryCell.Text = InternationalisationResources.Bittrex;
			RepositoryNameEntryCell.Entry.TextChanged += (sender, e) => Header.TitleText = e.NewTextValue;
		}

		void Cancel(object sender, EventArgs e)
		{
			Navigation.PopModalAsync();
		}

		async void Save(object sender, EventArgs e)
		{
			Header.IsLoading = true;

			RepositoryNameEntryCell.IsEditable = false;
			ApiKeyEntryCell.IsEditable = false;
			SecretApiKeyEntryCell.IsEditable = false;

			var nameText = RepositoryNameEntryCell.Text.Trim();
			var name = nameText.Equals(string.Empty) ? InternationalisationResources.Bittrex : nameText;
			var key = ApiKeyEntryCell.Text ?? string.Empty;
			var secretKey = SecretApiKeyEntryCell.Text ?? string.Empty;

			var repository = new BittrexAccountRepository(name, key, secretKey);

			var success = await repository.Fetch();
			if (success)
			{
				Header.LoadingText = InternationalisationResources.Fetching;
				// TODO Fix every account twice
				await AccountStorage.Instance.Add(new AccountRepositoryDBM(repository));
				await AccountStorage.Instance.Fetch();
				MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

				await Navigation.PopModalAsync();
			}
			else {
				Header.IsLoading = false;
				await DisplayAlert(InternationalisationResources.Error, InternationalisationResources.FetchingNoSuccessText, InternationalisationResources.Ok);

				RepositoryNameEntryCell.IsEditable = true;
				ApiKeyEntryCell.IsEditable = true;
				SecretApiKeyEntryCell.IsEditable = true;
			}
		}
	}
}
