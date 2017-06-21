using System;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;
using MyCC.Ui.Messages;

namespace MyCC.Ui.Edit
{
    internal class EditUtils : IEditUtils
    {
        public Task<bool> Add(OnlineAccountRepository repository)
            => EditAccounts.Add(repository);

        public Task Add(LocalAccount account)
            => EditAccounts.Add(account);

        public async Task Update(FunctionalAccount account)
        {
            await AccountStorage.Update(account);
            UiUtils.AssetsRefresh.ResetCache();
            Messaging.UiUpdate.Assets.Send();
        }

        public async Task Delete(FunctionalAccount account)
        {
            await AccountStorage.Instance.LocalRepository.Remove(account);
            UiUtils.AssetsRefresh.ResetCache();
            Messaging.UiUpdate.Assets.Send();
        }

        public async Task Delete(OnlineAccountRepository repository)
        {
            await AccountStorage.Instance.Remove(repository);
            UiUtils.AssetsRefresh.ResetCache();
            Messaging.UiUpdate.Assets.Send();
        }

        public Task Update(OnlineAccountRepository repository, string newAddress, string newCurrencyId, string newName, bool newEnabledState, Action testingFailed = null)
        => EditRepository.Update(repository, newAddress, newCurrencyId, newName, newEnabledState, testingFailed);

        public void AddWatchedCurrency(string currencyId)
            => EditWatchedCurrencies.Add(currencyId);


        public void RemoveWatchedCurrency(string currencyId)
            => EditWatchedCurrencies.Remove(currencyId);


        public void AddReferenceCurrency(string currencyId)
            => EditReferenceCurrencies.AddReferenceCurrency(currencyId);

        public bool RemoveReferenceCurrency(string currencyId)
            => EditReferenceCurrencies.RemoveReferenceCurrency(currencyId);

        public bool ToggleReferenceCurrencyStar(string currencyId)
            => EditReferenceCurrencies.ToggleReferenceCurrencyStar(currencyId);
    }
}