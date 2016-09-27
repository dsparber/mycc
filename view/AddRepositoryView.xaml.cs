using System;
using data.database.models;
using data.repositories.account;
using data.storage;
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
			RepositoryNameEntryCell.Text = InternationalisationResources.Bittrex;
			RepositoryNameEntryCell.Entry.TextChanged += (sender, e) => Header.TitleText = e.NewTextValue;
		}

		void Cancel(object sender, EventArgs e)
		{
			Navigation.PopModalAsync();
		}

		async void Save(object sender, EventArgs e)
		{
			var nameText = RepositoryNameEntryCell.Text.Trim();
			var name = nameText.Equals(string.Empty) ? InternationalisationResources.Bittrex : nameText;
			var key = ApiKeyEntryCell.Text;
			var secretKey = SecretApiKeyEntryCell.Text;

			// TODO Verify data -> Try to load accounts, set accountview/coinview

			var repository = new BittrexAccountRepository(name, key, secretKey);
			await AccountStorage.Instance.Add(new AccountRepositoryDBM(repository));
			await Navigation.PopModalAsync();
		}
	}
}
