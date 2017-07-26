using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;

namespace MyCC.Ui.Edit
{
    public interface IEditUtils
    {
        Task<bool> Add(OnlineAccountRepository repository);
        Task Add(LocalAccount account);


        Task Update(FunctionalAccount account);
        Task Delete(FunctionalAccount account);

        Task Delete(OnlineAccountRepository repository);
        Task Update(OnlineAccountRepository repository, string newAddress, string newCurrencyId, string newName, Dictionary<int, bool> newEnabledStates, Action onError = null);


        void AddWatchedCurrency(string currencyId);
        void RemoveWatchedCurrency(string currencyId);

        void AddReferenceCurrency(string currencyId);
        bool RemoveReferenceCurrency(string currencyId);
        bool ToggleReferenceCurrencyStar(string currencyId);

        string SelectedCryptoToFiatSource { set; }
    }
}