using System;
using System.Linq;
using data.database.models;
using data.storage;
using message;
using MyCryptos.resources;
using Xamarin.Forms;
using MyCryptos.view.addrepositoryviews;
using System.Collections.Generic;

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
            }

            addViews = new List<AbstractAddRepositoryView>();
            addViews.Add(new AddBittrexRepositoryView());

            addViews = addViews.OrderBy(v => v.DefaultName).ToList();
            RepositorySpecificView = addViews[0];
            Stack.Children.Add(RepositorySpecificView);

            foreach (var item in addViews.Select(v => v.DefaultName))
            {
                TypePickerCell.Picker.Items.Add(item);
            }
            TypePickerCell.Picker.SelectedIndex = 0;
            TypePickerCell.Picker.SelectedIndexChanged += (sender, e) => { RepositorySpecificView = addViews[TypePickerCell.Picker.SelectedIndex]; };

            Header.TitleText = RepositorySpecificView.DefaultName;
            RepositoryNameEntryCell.Text = RepositorySpecificView.DefaultName;
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
            RepositorySpecificView.Enabled = false;

            var nameText = RepositoryNameEntryCell.Text.Trim();
            var name = nameText.Equals(string.Empty) ? InternationalisationResources.Bittrex : nameText;

            var repository = RepositorySpecificView.GetRepository(name);

            var success = await repository.Fetch();
            if (success)
            {
                Header.LoadingText = InternationalisationResources.Fetching;
                await AccountStorage.Instance.Add(new AccountRepositoryDBM(repository));
                await AccountStorage.Instance.Fetch();
                MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

                await Navigation.PopModalAsync();
            }
            else
            {
                Header.IsLoading = false;
                await DisplayAlert(InternationalisationResources.Error, InternationalisationResources.FetchingNoSuccessText, InternationalisationResources.Ok);

                RepositoryNameEntryCell.IsEditable = true;
                RepositorySpecificView.Enabled = true;
            }
        }
    }
}
