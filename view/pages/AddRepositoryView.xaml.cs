using System;
using System.Linq;
using data.storage;
using message;
using MyCryptos.resources;
using Xamarin.Forms;
using MyCryptos.view.addrepositoryviews;
using System.Collections.Generic;
using MyCryptos.models;
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
			Title = I18N.AddRepositoryTitle;
			Header.InfoText = I18N.AddSource;
			Header.LoadingText = I18N.Testing;

			if (Device.OS == TargetPlatform.Android)
			{
				ToolbarItems.Remove(CancelItem);
                Title = string.Empty;
			}

			addViews = new List<AbstractAddRepositoryView>();
			addViews.Add(new AddBittrexRepositoryView());
			addViews.Add(new AddBlockExpertsRepositoryView(Navigation));
            addViews.Add(new AddBlockchainRepositoryView());
            addViews.Add(new AddEtherchainRepositoryView());
            addViews.Add(new AddCryptoIdRepositoryView(Navigation));

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
                RepositoryNameEntryCell.Placeholder = RepositorySpecificView.DefaultName;
                var txt = RepositoryNameEntryCell.Text?.Trim();
                Header.TitleText = (string.Empty.Equals(txt) || txt == null) ?  RepositorySpecificView.DefaultName : txt;

				TableView.Root.Remove(old.InputSection);
				TableView.Root.Add(RepositorySpecificView.InputSection);
			};

			Header.TitleText = RepositorySpecificView.DefaultName;
            RepositoryNameEntryCell.Placeholder = RepositorySpecificView.DefaultName;
            RepositoryNameEntryCell.Entry.TextChanged += (sender, e) => Header.TitleText = (e.NewTextValue.Length != 0) ? e.NewTextValue : RepositorySpecificView.DefaultName;
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

			var nameText = (RepositoryNameEntryCell.Text?? string.Empty).Trim();
			var name = nameText.Equals(string.Empty) ? RepositorySpecificView.DefaultName : nameText;

			var repository = RepositorySpecificView.GetRepository(name);

			var success = await repository.Test();
			if (success)
			{
				Header.LoadingText = I18N.Fetching;
				await AccountStorage.Instance.Add(repository);
				await AccountStorage.Instance.Fetch();
				MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

				var newRates = repository.Elements.Select(a => new ExchangeRate(a.Money.Currency, ApplicationSettings.BaseCurrency));
				AppTasks.Instance.StartMissingRatesTask(newRates);

				await Navigation.PopOrPopModal();
			}
			else
			{
				Header.IsLoading = false;
				await DisplayAlert(I18N.Error, I18N.FetchingNoSuccessText, I18N.Ok);

				RepositoryNameEntryCell.IsEditable = true;
				TypePickerCell.IsEditable = true;
				RepositorySpecificView.Enabled = true;
			}
		}
	}
}
