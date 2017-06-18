using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies.Models;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;
using Xamarin.Forms;
using MyCC.Core.Helpers;

namespace MyCC.Forms.View.Pages.Settings.Source
{
    public partial class RepositoryView
    {
        private OnlineAccountRepository _repository;
        private readonly List<Tuple<FunctionalAccount, bool>> _changedAccounts;
        private readonly CurrencyEntryCell _currencyEntryCell;

        private readonly bool _isEditModal;

        public RepositoryView(OnlineAccountRepository repository, bool isEditModal = false)
        {
            InitializeComponent();
            _repository = repository;
            _isEditModal = isEditModal;

            _changedAccounts = new List<Tuple<FunctionalAccount, bool>>();

            Header.TitleText = repository.Name;
            Header.InfoText = $"{I18N.Source}: {_repository.Description}";

            var addressAccountRepository = repository as AddressAccountRepository;
            if (addressAccountRepository != null)
            {
                EnableAccountsSection.Title = I18N.EnableAccount;
                AddressEntryCell.Text = addressAccountRepository.Address.MiddleTruncate();

                var currenciesTask = new Func<IEnumerable<Currency>>(() => AddressAccountRepository.AllSupportedCurrencies);

                _currencyEntryCell = new CurrencyEntryCell(Navigation, currenciesTask)
                {
                    IsAmountEnabled = false,
                    IsEditable = false,
                    SelectedCurrency = addressAccountRepository.Currency,
                    IsFormRepresentation = true
                };
                GeneralSection.Add(_currencyEntryCell);
            }
            else
            {
                GeneralSection.Remove(AddressEntryCell);
            }

            if (repository is BittrexAccountRepository || repository is PoloniexAccountRepository)
            {
                EnableAccountsSection.Title = I18N.Accounts;
            }
            if (repository is BlockchainXpubAccountRepository)
            {
                GeneralSection.Remove(AddressEntryCell);
                GeneralSection.Remove(_currencyEntryCell);
            }

            RepositoryNameEntryCell.Text = repository.Name;
            DeleteButtonCell.Tapped += Delete;

            TableView.Root.Remove(DeleteSection);
            ToolbarItems.Remove(SaveItem);

            RepositoryNameEntryCell.Entry.TextChanged += (sender, e) => Header.TitleText = e.NewTextValue;

            SetView(isEditModal);

            Messaging.UpdatingAccounts.SubscribeFinished(this, SetView);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, SetView);

            IsEditable = false;

            AddressEntryCell.Editor.Focused += (sender, args) => AddressEntryCell.Text = (_repository as AddressAccountRepository)?.Address;

            if (!_isEditModal) return;

            EditClicked(null, null);
            var cancel = new ToolbarItem { Text = I18N.Cancel };
            cancel.Clicked += (s, e) => Navigation.PopOrPopModal();
            ToolbarItems.Insert(0, cancel);
            IsEditable = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!AccountStorage.Instance.Repositories.Contains(_repository))
            {
                Navigation.PopAsync();
            }
        }

        private void SetView() => SetView(false);

        private void SetView(bool isEditable)
        {
            var enableCells = _repository.Elements.OrderBy(e => e.Money.Currency.Name).Select(e =>
             {
                 var cell = new CustomSwitchCell { Info = $"{e.Money.ToString(false)} {e.Money.Currency.Name}", Title = e.Money.Currency.Code, On = e.IsEnabled };
                 cell.Switch.Toggled += (sender, args) =>
                 {
                     _changedAccounts.RemoveAll(t => t.Item1.Equals(e));
                     _changedAccounts.Add(Tuple.Create(e, args.Value));
                 };
                 cell.Switch.IsEnabled = IsEditable;
                 return (ViewCell)cell;
             }).ToList();

            if (enableCells.Count == 0) { enableCells.Add(new CustomViewCell { Text = I18N.NoAccounts }); }

            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    EnableAccountsSection.Clear();
                    EnableAccountsSection.Add(enableCells);
                    Footer.Text = _repository.LastFetch.LastUpdateString();
                    IsEditable = isEditable;
                }
                catch (ObjectDisposedException e) { e.LogError(); }
            });
        }

        private async void Delete(object sender, EventArgs e)
        {
            UnfocusAll();

            Header.LoadingText = I18N.Deleting;
            Header.IsLoading = true;
            RepositoryNameEntryCell.IsEditable = false;

            await AccountStorage.Instance.Remove(_repository);
            Messaging.UpdatingAccounts.SendFinished();
            Header.IsLoading = false;
            if (_isEditModal)
            {
                await Navigation.PopOrPopModal();
            }
            else
            {
                await Navigation.PopAsync();
            }
        }

        private bool IsEditable
        {
            get => RepositoryNameEntryCell.IsEditable;
            set
            {
                if (_currencyEntryCell != null) _currencyEntryCell.IsEditable = value;
                RepositoryNameEntryCell.IsEditable = value;
                AddressEntryCell.IsEditable = value;
                foreach (var c in EnableAccountsSection.OfType<CustomSwitchCell>())
                {
                    c.Switch.IsEnabled = value;
                }
            }
        }

        private void EditClicked(object sender, EventArgs e)
        {
            IsEditable = true;

            TableView.Root.Add(DeleteSection);

            Title = I18N.Editing;

            ToolbarItems.Remove(EditItem);
            ToolbarItems.Add(SaveItem);
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            try
            {
                UnfocusAll();

                // Disable Editing
                Header.LoadingText = I18N.Testing;
                SaveItem.Clicked -= SaveClicked;
                Header.IsLoading = true;
                IsEditable = false;

                // Test if data is valid
                var addressRepo = _repository as AddressAccountRepository;
                if (addressRepo != null &&
                    (!addressRepo.Address.Equals((AddressEntryCell.Text ?? string.Empty).Contains("...")
                         ? addressRepo.Address
                         : AddressEntryCell.Text ?? string.Empty) ||
                     !addressRepo.Currency.Equals(_currencyEntryCell.SelectedCurrency)))
                {
                    var address = AddressEntryCell.Text ?? string.Empty;
                    var testRepo = AddressAccountRepository.CreateAddressAccountRepository(addressRepo.Name,
                        _currencyEntryCell.SelectedCurrency, address.Contains("...") ? addressRepo.Address : address);

                    if (testRepo == null || !await testRepo.Test())
                    {
                        SaveItem.Clicked += SaveClicked;
                        Header.IsLoading = false;
                        IsEditable = true;
                        await DisplayAlert(I18N.Error, I18N.FetchingNoSuccessText, I18N.Ok);
                        return;
                    }
                    if (!addressRepo.Currency.Equals(_currencyEntryCell.SelectedCurrency))
                    {
                        await AccountStorage.Instance.Remove(_repository);
                        await testRepo.FetchOnline();
                        await AccountStorage.Instance.Add(testRepo);
                    }
                    var enabled = EnableAccountsSection.Any(a => ((CustomSwitchCell)a).On);
                    testRepo.Id = _repository.Id;
                    _repository = testRepo;
                    await _repository.FetchOnline();
                    // Set (en-/dis-)abled
                    foreach (var a in _repository.Elements) a.IsEnabled = enabled;
                }

                // Apply name and enabled status
                _repository.Name = RepositoryNameEntryCell.Text ?? I18N.Unnamed;
                foreach (var a in _repository.Elements) a.Name = _repository.Name;
                foreach (var a in _changedAccounts) a.Item1.IsEnabled = a.Item2;

                // Save changes
                await AccountStorage.Instance.Update(_repository);
                await Task.WhenAll(_repository.Elements.ToList().Select(AccountStorage.Update));

                Messaging.UpdatingAccounts.SendFinished();

                if (_isEditModal)
                {
                    await Navigation.PopOrPopModal();
                    return;
                }

                TableView.Root.Remove(DeleteSection);

                ToolbarItems.Remove(SaveItem);
                ToolbarItems.Add(EditItem);

                Title = I18N.Details;
                Header.InfoText = $"{I18N.Source}: {_repository.Description}";

                SaveItem.Clicked += SaveClicked;
                Header.IsLoading = false;

                SetView();
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        private void UnfocusAll()
        {
            RepositoryNameEntryCell.Entry.Unfocus();
        }
    }
}
