using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies.Models;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;
using Xamarin.Forms;
using MyCC.Core.Helpers;
using MyCC.Ui;
using MyCC.Ui.DataItems;
using MyCC.Ui.Messages;

namespace MyCC.Forms.View.Pages.Settings.Source
{
    public partial class RepositoryView
    {
        private readonly OnlineAccountRepository _repository;
        private readonly CurrencyEntryCell _currencyEntryCell;

        private readonly bool _isEditModal;

        public RepositoryView(OnlineAccountRepository repository, bool isEditModal = false)
        {
            InitializeComponent();
            _repository = repository;
            _isEditModal = isEditModal;

            Header.Data = new HeaderItem(repository.Name, $"{I18N.Source}: {_repository.Description}");

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

            RepositoryNameEntryCell.Entry.TextChanged += (sender, e) => Header.Title = e.NewTextValue;

            SetView(isEditModal);

            Messaging.Update.Balances.Subscribe(this, SetViewAction);

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

        private void SetViewAction() => SetView(false);

        private void SetView(bool isEditable)
        {
            var enableCells = _repository.Elements.Select(balance =>
                new CustomSwitchCell
                {
                    Title = balance.Money.TwoDigits(),
                    Info = balance.Money.Currency.Name,
                    SwitchOn = balance.IsEnabled
                });

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

            await UiUtils.Edit.Delete(_repository);
            Header.IsLoading = false;

            if (_isEditModal) await Navigation.PopOrPopModal();
            else await Navigation.PopAsync();
        }

        private bool IsEditable
        {
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

                var addressRepo = _repository as AddressAccountRepository;
                var address = (AddressEntryCell.Text ?? string.Empty).Contains("...") ? addressRepo?.Address ?? string.Empty : AddressEntryCell.Text;
                var name = RepositoryNameEntryCell.Text ?? I18N.Unnamed;



                Dictionary<int, bool> enabledStates;
                if (addressRepo != null)
                {
                    enabledStates = new Dictionary<int, bool>();
                    enabledStates[addressRepo.Elements.First().Id] = EnableAccountsSection.OfType<CustomSwitchCell>().First().Switch.IsToggled;
                }
                else
                {
                    enabledStates = EnableAccountsSection.OfType<CustomSwitchCell>().ToDictionary(
                        cell => _repository.Elements.First(a => a.Money.Currency.Name.Equals(cell.Info)).Id,
                        cell => cell.Switch.IsToggled);
                }
                await UiUtils.Edit.Update(_repository, address, _currencyEntryCell?.SelectedCurrency.Id, name, enabledStates);

                if (_isEditModal)
                {
                    await Navigation.PopOrPopModal();
                    return;
                }

                TableView.Root.Remove(DeleteSection);

                ToolbarItems.Remove(SaveItem);
                ToolbarItems.Add(EditItem);

                Title = I18N.Details;
                Header.Info = $"{I18N.Source}: {_repository.Description}";

                SaveItem.Clicked += SaveClicked;
                Header.IsLoading = false;

                SetViewAction();
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
