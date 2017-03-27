using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MyCC.Core.Account.Storage;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Addsource;

namespace MyCC.Forms.View.Overlays
{
    public partial class AddSourceOverlay
    {
        private AddSourceSubview _specificAddView;

        public AddSourceOverlay(bool local = false)
        {
            InitializeComponent();
            Header.TitleText = I18N.NewSource;
            Header.LoadingText = I18N.Testing;

            var addViews = new List<AddSourceSubview>
            {
                new AddAddressSubview(Navigation, NameEntryCell.Entry),
                new AddBittrexSubview(),
                new AddLocalAccountSubview(Navigation)
            };

            _specificAddView = addViews[local ? 2 : 0];
            TableViewComponent.Root.Clear();
            foreach (var s in _specificAddView.InputSections)
            {
                TableViewComponent.Root.Add(s);
            }
            TableViewComponent.Root.Add(NameSection);

            SegmentedControl.BackgroundColor = AppConstants.TableBackgroundColor;
            SegmentedControl.Tabs = addViews.Select(v => v.Description).ToList();
            SegmentedControl.SelectedIndex = local ? 2 : 0;
            SegmentedControl.SelectionChanged = index =>
            {
                _specificAddView = addViews[index];
                NameEntryCell.Placeholder = _specificAddView.DefaultName;
                var txt = NameEntryCell.Text?.Trim();
                Header.InfoText = string.Empty.Equals(txt) || txt == null ? _specificAddView.DefaultName : txt;

                TableViewComponent.Root.Clear();
                foreach (var s in _specificAddView.InputSections)
                {
                    TableViewComponent.Root.Add(s);
                }
                TableViewComponent.Root.Add(NameSection);
            };

            Header.InfoText = _specificAddView.DefaultName;
            NameEntryCell.Placeholder = _specificAddView.DefaultName;
            NameEntryCell.Entry.TextChanged += (sender, e) => Header.InfoText = e.NewTextValue.Length != 0 ? e.NewTextValue : _specificAddView.DefaultName;
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
                _specificAddView.Enabled = false;

                var nameText = (NameEntryCell.Text ?? string.Empty).Trim();
                var name = nameText.Equals(string.Empty) ? _specificAddView.DefaultName : nameText;

                var addView = _specificAddView as AddRepositorySubview;
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

                        var success = await repository.Test();
                        if (success)
                        {
                            Header.LoadingText = I18N.Fetching;
                            await AccountStorage.Instance.Add(repository);
                            await repository.FetchOnline();
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
                    _specificAddView.Enabled = true;
                    addView.Enabled = true;
                }
                else if (_specificAddView is AddAccountSubview)
                {
                    var account = ((AddAccountSubview)_specificAddView).GetAccount(name);

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
                        _specificAddView.Enabled = true;
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
            _specificAddView.Unfocus();
        }
    }
}
