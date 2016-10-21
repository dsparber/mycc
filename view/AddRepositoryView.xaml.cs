using System;
using System.Linq;
using data.database.models;
using data.storage;
using message;
using MyCryptos.resources;
using Xamarin.Forms;
using MyCryptos.view.addrepositoryviews;
using System.Collections.Generic;
using models;
using data.settings;
using tasks;
using MyCryptos.helpers;

namespace view
{
	public partial class AddRepositoryView : ContentPage
	{
		List<AbstractAddRepositoryView> addViews;
		AbstractAddRepositoryView RepositorySpecificView;

		public AddRepositoryView()
		{
			InitializeComponent();
			Title = InternationalisationResources.AddRepositoryTitle;
			Header.InfoText = InternationalisationResources.AddSource;
			Header.LoadingText = InternationalisationResources.Testing;

			if (Device.OS == TargetPlatform.Android)
			{
				ToolbarItems.Remove(CancelItem);
                Title = string.Empty;
			}

			addViews = new List<AbstractAddRepositoryView>();
			addViews.Add(new AddBittrexRepositoryView());
			addViews.Add(new AddBlockExpertsRepositoryView(Navigation));
            addViews.Add(new AddBlockchainRepositoryView());

			addViews = addViews.OrderBy(v => v.DefaultName).ToList();
			RepositorySpecificView = addViews[0];
			TableView.Root.Add(RepositorySpecificView.InputSection);

			foreach (var item in addViews.Select(v => v.Description))
			{
				TypePickerCell.Picker.Items.Add(item);
			}
			TypePickerCell.Picker.SelectedIndex = 0;
			TypePickerCell.Picker.SelectedIndexChanged += (sender, e) =>
			{
				var old = RepositorySpecificView;

				RepositorySpecificView = addViews[TypePickerCell.Picker.SelectedIndex];
				RepositoryNameEntryCell.Text = (old.DefaultName.Equals(RepositoryNameEntryCell.Text)) ? RepositorySpecificView.DefaultName : RepositoryNameEntryCell.Text;

				TableView.Root.Remove(old.InputSection);
				TableView.Root.Add(RepositorySpecificView.InputSection);
			};

			Header.TitleText = RepositorySpecificView.DefaultName;
			RepositoryNameEntryCell.Text = RepositorySpecificView.DefaultName;
			RepositoryNameEntryCell.Entry.TextChanged += (sender, e) => Header.TitleText = e.NewTextValue;
		}

		void Cancel(object sender, EventArgs e)
		{
            Navigation.PopOrPopModal();
		}

		async void Save(object sender, EventArgs e)
		{
			Header.IsLoading = true;

			RepositoryNameEntryCell.IsEditable = false;
			TypePickerCell.IsEditable = false;
			RepositorySpecificView.Enabled = false;

			var nameText = RepositoryNameEntryCell.Text.Trim();
			var name = nameText.Equals(string.Empty) ? InternationalisationResources.Bittrex : nameText;

			var repository = RepositorySpecificView.GetRepository(name);

			var success = await repository.Fetch();
			if (success)
			{
				Header.LoadingText = InternationalisationResources.Fetching;
				await AccountStorage.Instance.AddRepository(new AccountRepositoryDBM(repository));
				await AccountStorage.Instance.Fetch();
				MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

				var newRates = repository.Elements.Select(a => new ExchangeRate(a.Money.Currency, ApplicationSettings.BaseCurrency));
				AppTasks.Instance.StartMissingRatesTask(newRates);

				await Navigation.PopOrPopModal();
			}
			else
			{
				Header.IsLoading = false;
				await DisplayAlert(InternationalisationResources.Error, InternationalisationResources.FetchingNoSuccessText, InternationalisationResources.Ok);

				RepositoryNameEntryCell.IsEditable = true;
				TypePickerCell.IsEditable = true;
				RepositorySpecificView.Enabled = true;
			}
		}
	}
}
