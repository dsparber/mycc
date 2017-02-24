using System;
using System.Linq;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Account.Storage;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components.Cells;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages.Settings
{
    public partial class RepositoryView
    {
        private readonly AccountRepository _repository;

        private readonly bool _isEditModal;

        public RepositoryView(AccountRepository repository, bool isEditModal = false)
        {
            InitializeComponent();
            _repository = repository;
            _isEditModal = isEditModal;

            Header.TitleText = repository.Name;
            if (repository is AddressAccountRepository)
            {
                Header.InfoText = $"{I18N.Source}: {repository.Description}";
                AccountsSection.Title = I18N.Amount;
            }
            else
            {
                Header.InfoText = PluralHelper.GetTextCurrencies(repository.ElementsCount);
                GeneralSection.Remove(AddressEntryCell);
            }

            if (repository is BittrexAccountRepository)
            {
                AccountsSection.Title = I18N.Accounts;
            }

            RepositoryNameEntryCell.Text = repository.Name;
            DeleteButtonCell.Tapped += Delete;

            TableView.Root.Remove(DeleteSection);
            ToolbarItems.Remove(SaveItem);

            RepositoryNameEntryCell.Entry.TextChanged += (sender, e) => Header.TitleText = e.NewTextValue;

            SetView();

            if (!_isEditModal) return;

            EditClicked(null, null);
            var cancel = new ToolbarItem { Text = I18N.Cancel };
            cancel.Clicked += (s, e) => Navigation.PopOrPopModal();
            ToolbarItems.Add(cancel);
        }

        private void SetView()
        {
            var cells = _repository.Elements.OrderBy(e => e.Money.Currency.Code).Select(e =>
            {
                var cell = new CustomViewCell { Text = e.Money.ToString(), Detail = e.Money.Currency.Name };
                if (!(_repository is LocalAccountRepository)) return cell;

                cell.Image = "more.png";
                cell.Detail = e.Name;
                cell.Tapped += (sender, eventArgs) => Navigation.PushAsync(new AccountEditView(e, _repository as LocalAccountRepository));
                return cell;
            }).ToList();

            if (cells.Count == 0)
            {
                cells.Add(new CustomViewCell
                {
                    Text = I18N.NoAccounts
                });
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                AccountsSection.Clear();
                AccountsSection.Add(cells);
            });

            if (_repository is AddressAccountRepository)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    AddressEntryCell.Text = (_repository as AddressAccountRepository)?.Address;
                });
            }
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
            await Navigation.PopAsync();
        }

        private void EditClicked(object sender, EventArgs e)
        {
            RepositoryNameEntryCell.IsEditable = true;
            if (!(_repository is LocalAccountRepository))
            {
                TableView.Root.Add(DeleteSection);
            }
            TableView.Root.Remove(AccountsSection);

            Title = I18N.Editing;
            RepositoryNameEntryCell.IsEditable = true;
            AddressEntryCell.IsEditable = true;

            ToolbarItems.Remove(EditItem);
            ToolbarItems.Add(SaveItem);
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            UnfocusAll();

            SaveItem.Clicked -= SaveClicked;
            Header.IsLoading = true;
            RepositoryNameEntryCell.IsEditable = false;
            AddressEntryCell.IsEditable = false;

            Action revertChanges = () => { };
            var success = true;
            var changed = false;

            _repository.Name = RepositoryNameEntryCell.Text ?? string.Empty;
            foreach (var a in _repository.Elements) a.Name = _repository.Name;

            var repository = _repository as AddressAccountRepository;
            if (repository != null)
            {
                var addressReposiotry = repository;
                var oldAddress = addressReposiotry.Address;
                var addressText = AddressEntryCell.Text ?? string.Empty;
                if (!addressText.Equals(addressReposiotry.Address))
                {
                    addressReposiotry.Address = addressText;
                    changed = true;
                }
                revertChanges = () => addressReposiotry.Address = oldAddress;
            }
            else if (_repository is BittrexAccountRepository)
            {
                // TODO Api Keys
            }

            if (changed)
            {
                var successful = await AccountStorage.AddRepository((OnlineAccountRepository)_repository);
                if (successful)
                {
                    await AppTaskHelper.FetchMissingRates();
                }
                else
                {
                    await DisplayAlert(I18N.Error, I18N.FetchingNoSuccessText, I18N.Ok);
                    success = false;

                    revertChanges();
                }
            }

            if (success)
            {

                await AccountStorage.Instance.Update(_repository);
                Messaging.UpdatingAccounts.SendFinished();

                if (_isEditModal)
                {
                    await Navigation.PopOrPopModal();
                }

                RepositoryNameEntryCell.IsEditable = false;
                AddressEntryCell.IsEditable = false;

                TableView.Root.Add(AccountsSection);
                TableView.Root.Remove(DeleteSection);

                ToolbarItems.Remove(SaveItem);
                ToolbarItems.Add(EditItem);

                Title = I18N.Accounts;
            }
            else
            {
                RepositoryNameEntryCell.IsEditable = true;
                AddressEntryCell.IsEditable = true;
            }
            SaveItem.Clicked += SaveClicked;
            Header.IsLoading = false;
        }

        private void UnfocusAll()
        {
            RepositoryNameEntryCell.Entry.Unfocus();
        }
    }
}
