using System;
using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;

namespace MyCC.Ui.Edit
{
    public interface IEditUtils
    {
        Task<bool> Add(OnlineAccountRepository repository, Action testingStarted = null, Action alreadyAdded = null, Action testingFailed = null);
        Task Add(LocalAccount account);


        Task Update(FunctionalAccount account);
        Task Delete(FunctionalAccount account);

        void AddWatchedCurrency(string currencyId);
        void RemoveWatchedCurrency(string currencyId);

        void AddReferenceCurrency(string currencyId);
        bool RemoveReferenceCurrency(string currencyId);
        bool ToggleReferenceCurrencyStar(string currencyId);
    }
}