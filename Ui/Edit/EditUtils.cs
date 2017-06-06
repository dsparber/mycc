using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;

namespace MyCC.Ui.Edit
{
    internal class EditUtils : IEditUtils
    {
        public async Task Update(FunctionalAccount account)
        {
            await AccountStorage.Update(account);
            UiUtils.Update.CreateAssetsData();
        }

        public async Task Delete(FunctionalAccount account)
        {
            await AccountStorage.Instance.LocalRepository.Remove(account);
            UiUtils.Update.CreateAssetsData();
        }

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