using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages.Settings
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

                _currencyEntryCell = new CurrencyEntryCell(Navigation)
                {
                    IsAmountEnabled = false,
                    IsEditable = false,
                    SelectedCurrency = addressAccountRepository.Currency,
                    IsFormRepresentation = true,
                    CurrenciesToSelect = AddressAccountRepository.AllSupportedCurrencies
                };
                GeneralSection.Add(_currencyEntryCell);
            }
            else
            {
                GeneralSection.Remove(AddressEntryCell);
            }

            if (repository is BittrexAccountRepository)
            {
                EnableAccountsSection.Title = $"{I18N.Accounts} ({repository.Elements.Count(r => r.IsEnabled)} {I18N.Enabled})";
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

            SetView();

            Messaging.UpdatingAccounts.SubscribeFinished(this, SetView);
            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, SetView);

            IsEditable = false;

            if (!_isEditModal) return;

            EditClicked(null, null);
            var cancel = new ToolbarItem { Text = I18N.Cancel };
            cancel.Clicked += (s, e) => Navigation.PopOrPopModal();
            ToolbarItems.Add(cancel);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            AddressEntryCell.Text = (_repository as AddressAccountRepository)?.Address;

            if (!AccountStorage.Instance.Repositories.Contains(_repository))
            {
                Navigation.PopAsync();
            }
        }

        private void SetView()
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
                EnableAccountsSection.Clear();
                EnableAccountsSection.Add(enableCells);
                Footer.Text = _repository.LastFetch.LastUpdateString();
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
            get { return RepositoryNameEntryCell.IsEditable; }
            set
            {
                if (_currencyEntryCell != null) _currencyEntryCell.IsEditable = value;
                RepositoryNameEntryCell.IsEditable = value;
                AddressEntryCell.IsEditable = value;
                foreach (var c in EnableAccountsSection.OfType<CustomSwitchCell>()) c.Switch.IsEnabled = value;
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
            UnfocusAll();

            // Disable Editing
            Header.LoadingText = I18N.Testing;
            SaveItem.Clicked -= SaveClicked; Header.IsLoading = true; IsEditable = false;

            // Test if data is valid
            var addressRepo = _repository as AddressAccountRepository;
            if (addressRepo != null && (!addressRepo.Address.Equals(AddressEntryCell.Text) || !addressRepo.Currency.Equals(_currencyEntryCell.SelectedCurrency)))
            {
                var testRepo = AddressAccountRepository.CreateAddressAccountRepository(addressRepo.Name, _currencyEntryCell.SelectedCurrency, AddressEntryCell.Text ?? string.Empty);

                if (testRepo == null || !await testRepo.Test())
                {
                    SaveItem.Clicked += SaveClicked; Header.IsLoading = false; IsEditable = true;
                    await DisplayAlert(I18N.Error, I18N.FetchingNoSuccessText, I18N.Ok);
                    return;
                }
                if (!addressRepo.Currency.Equals(_currencyEntryCell.SelectedCurrency))
                {
                    await AccountStorage.Instance.Remove(_repository);
                    await testRepo.FetchOnline();
                    await AccountStorage.Instance.Add(testRepo);
                }
                testRepo.Id = _repository.Id;
                _repository = testRepo;
                await _repository.FetchOnline();
            }

            // Apply name and enabled status
            _repository.Name = RepositoryNameEntryCell.Text ?? I18N.Unnamed;
            foreach (var a in _repository.Elements) a.Name = _repository.Name;
            foreach (var a in _changedAccounts) a.Item1.IsEnabled = a.Item2;

            // Save changes
            await AccountStorage.Instance.Update(_repository);
            await Task.WhenAll(_repository.Elements.Select(AccountStorage.Update));

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

        private void UnfocusAll()
        {
            RepositoryNameEntryCell.Entry.Unfocus();
        }
    }
}
