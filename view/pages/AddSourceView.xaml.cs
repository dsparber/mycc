using System;
using System.Linq;
using data.storage;
using message;
using MyCryptos.resources;
using Xamarin.Forms;
using MyCryptos.view.addrepositoryviews;
using System.Collections.Generic;
using MyCryptos.helpers;
using constants;

namespace view
{
	public partial class AddSourceView : ContentPage
	{
		private List<AddSourceSubview> addViews;
		private AddSourceSubview specificAddView;

		public AddSourceView(bool local = false)
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

			addViews = new List<AddSourceSubview>();
			addViews.Add(new AddLocalAccountSubview(Navigation));
			addViews.Add(new AddAddressSubview(Navigation)
			{
				NameChanged = () =>
				{
					Header.InfoText = specificAddView.DefaultName;
					NameEntryCell.Placeholder = specificAddView.DefaultName;
				}
			});
			addViews.Add(new AddBittrexSubview());

			specificAddView = addViews[local ? 0 : 1];
			TableViewComponent.Root.Add(specificAddView.InputSection);

			SegmentedControl.BackgroundColor = AppConstants.TableBackgroundColor;
			SegmentedControl.Tabs = addViews.Select(v => v.Description).ToList();
			SegmentedControl.SelectedIndex = local ? 0 : 1;
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

			if (specificAddView is AddRepositorySubview)
			{
				var repository = ((AddRepositorySubview)specificAddView).GetRepository(name);

				if (repository == null)
				{
					await DisplayAlert(I18N.Error, I18N.VerifyInput, I18N.Cancel);
				}
				else
				{
					var success = await repository.Test();
					if (success)
					{
						Header.LoadingText = I18N.Fetching;
						await AccountStorage.Instance.Add(repository);
						await AccountStorage.Instance.Fetch();
						MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

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
			}
			else if (specificAddView is AddAccountSubview)
			{
				var account = ((AddAccountSubview)specificAddView).GetAccount(name);

				if (account != null)
				{
					await AccountStorage.Instance.LocalRepository.Add(account);
					MessagingCenter.Send(string.Empty, MessageConstants.UpdatedAccounts);

					await Navigation.PopOrPopModal();
				}
				else
				{
					Header.IsLoading = false;
					await DisplayAlert(I18N.Error, I18N.VerifyInput, I18N.Ok);

					NameEntryCell.IsEditable = true;
					specificAddView.Enabled = true;
				}
			}
		}

		private void UnfocusAll()
		{
			NameEntryCell.Entry.Unfocus();
			specificAddView.Unfocus();
		}
	}
}
