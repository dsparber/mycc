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
using constants;

namespace view
{
	public partial class AddRepositoryView : ContentPage
	{
		private List<AddSourceView> addViews;
		private AddSourceView specificAddView;

		public AddRepositoryView()
		{
			InitializeComponent();
			Title = I18N.AddRepositoryTitle;
			Header.TitleText = I18N.NewSource;
			Header.LoadingText = I18N.Testing;

			if (Device.OS == TargetPlatform.Android)
			{
				ToolbarItems.Remove(CancelItem);
				Title = string.Empty;
			}

			addViews = new List<AddSourceView>();
			addViews.Add(new AddWithAddressView(Navigation)
			{
				NameChanged = () =>
				{
					Header.InfoText = specificAddView.DefaultName;
					NameEntryCell.Placeholder = specificAddView.DefaultName;
				}
			});
			addViews.Add(new AddWithBittrexView());

			specificAddView = addViews[0];
			TableViewComponent.Root.Add(specificAddView.InputSection);

			SegmentedControl.BackgroundColor = AppConstants.TableBackgroundColor;
			SegmentedControl.Tabs = addViews.Select(v => v.Description).ToList();
			SegmentedControl.SelectionChanged = (index) =>
			{
				var old = specificAddView;

				specificAddView = addViews[index];
				NameEntryCell.Placeholder = specificAddView.DefaultName;
				var txt = NameEntryCell.Text?.Trim();
				Header.InfoText = (string.Empty.Equals(txt) || txt == null) ? specificAddView.DefaultName : txt;

				TableViewComponent.Root.Remove(old.InputSection);
				TableViewComponent.Root.Add(specificAddView.InputSection);
			};

			Header.InfoText = specificAddView.DefaultName;
			NameEntryCell.Placeholder = specificAddView.DefaultName;
			NameEntryCell.Entry.TextChanged += (sender, e) => Header.InfoText = (e.NewTextValue.Length != 0) ? e.NewTextValue : specificAddView.DefaultName;
		}

		void Cancel(object sender, EventArgs e)
		{
			UnfocusAll();
			Navigation.PopOrPopModal();
		}

		async void Save(object sender, EventArgs e)
		{
			UnfocusAll();
			Header.IsLoading = true;

			NameEntryCell.IsEditable = false;
			specificAddView.Enabled = false;

			var nameText = (NameEntryCell.Text ?? string.Empty).Trim();
			var name = nameText.Equals(string.Empty) ? specificAddView.DefaultName : nameText;

			var repository = specificAddView.GetRepository(name);

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

				NameEntryCell.IsEditable = true;
				specificAddView.Enabled = true;
			}
		}

		private void UnfocusAll()
		{
			NameEntryCell.Entry.Unfocus();
			specificAddView.Unfocus();
		}
	}
}
