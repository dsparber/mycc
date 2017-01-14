using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using constants;
using MyCryptos.Core.Account.Storage;
using MyCryptos.Core.ExchangeRate.Model;
using MyCryptos.Core.settings;
using MyCryptos.Core.tasks;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.Tasks;
using MyCryptos.Forms.view.addsource;
using MyCryptos.Forms.view.overlays;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
	public partial class AddSourceView
	{
		private readonly List<AddSourceSubview> addViews;
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

			addViews = new List<AddSourceSubview>
			{
				new AddAddressSubview(Navigation, NameEntryCell.Entry),
				new AddBittrexSubview(),
				new AddLocalAccountSubview(Navigation)
			};

			specificAddView = addViews[local ? 2 : 0];
			TableViewComponent.Root.Clear();
			foreach (var s in specificAddView.InputSections)
			{
				TableViewComponent.Root.Add(s);
			}
			TableViewComponent.Root.Add(NameSection);

			SegmentedControl.BackgroundColor = AppConstants.TableBackgroundColor;
			SegmentedControl.Tabs = addViews.Select(v => v.Description).ToList();
			SegmentedControl.SelectedIndex = local ? 2 : 0;
			SegmentedControl.SelectionChanged = (index) =>
			{
				specificAddView = addViews[index];
				NameEntryCell.Placeholder = specificAddView.DefaultName;
				var txt = NameEntryCell.Text?.Trim();
				Header.InfoText = (string.Empty.Equals(txt) || txt == null) ? specificAddView.DefaultName : txt;

				TableViewComponent.Root.Clear();
				foreach (var s in specificAddView.InputSections)
				{
					TableViewComponent.Root.Add(s);
				}
				TableViewComponent.Root.Add(NameSection);
			};

			Header.InfoText = specificAddView.DefaultName;
			NameEntryCell.Placeholder = specificAddView.DefaultName;
			NameEntryCell.Entry.TextChanged += (sender, e) => Header.InfoText = (e.NewTextValue.Length != 0) ? e.NewTextValue : specificAddView.DefaultName;
		}

		private void Cancel(object sender, EventArgs e)
		{
			UnfocusAll();
			Navigation.PopOrPopModal();
		}

		private async void Save(object sender, EventArgs e)
		{
			try
			{
				UnfocusAll();
				Header.IsLoading = true;

				NameEntryCell.IsEditable = false;
				specificAddView.Enabled = false;

				var nameText = (NameEntryCell.Text ?? string.Empty).Trim();
				var name = nameText.Equals(string.Empty) ? specificAddView.DefaultName : nameText;

				var addView = specificAddView as AddRepositorySubview;
				if (addView != null)
				{
					addView.Enabled = false;
					var repository = addView.GetRepository(name);

					if (repository == null)
					{
						await DisplayAlert(I18N.Error, I18N.VerifyInput, I18N.Cancel);
					}
					else if (AccountStorage.Instance.RepositoriesOfType(repository.GetType()).Any(r => r.Data.Equals(repository.Data)))
					{
						await DisplayAlert(I18N.Error, I18N.RepositoryAlreadyAdded, I18N.Cancel);
						await Navigation.PopOrPopModal();
					}
					else
					{

						var success = await AccountStorage.AddRepository(repository);
						if (success)
						{
							Header.LoadingText = I18N.Fetching;
							Messaging.UpdatingAccounts.SendFinished();
							await AppTaskHelper.FetchMissingRates();
							await Navigation.PopOrPopModal();

						}
						else
						{
							Header.IsLoading = false;
							await DisplayAlert(I18N.Error, I18N.FetchingNoSuccessText, I18N.Ok);
						}
					}
					Header.IsLoading = false;

					NameEntryCell.IsEditable = true;
					specificAddView.Enabled = true;
					addView.Enabled = true;
				}
				else if (specificAddView is AddAccountSubview)
				{
					var account = ((AddAccountSubview)specificAddView).GetAccount(name);

					if (account != null)
					{

						await AccountStorage.Instance.LocalRepository.Add(account);
						Messaging.UpdatingAccounts.SendFinished();

						var referenceCurrencies = ApplicationSettings.AllReferenceCurrencies.ToList();
						var neededRates = referenceCurrencies.Select(c => new ExchangeRate(account.Money.Currency, c)).ToList();

						if (neededRates.Count > 0)
						{
							await AppTaskHelper.FetchMissingRates();
						}
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
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		private void UnfocusAll()
		{
			NameEntryCell.Entry.Unfocus();
			specificAddView.Unfocus();
		}
	}
}
