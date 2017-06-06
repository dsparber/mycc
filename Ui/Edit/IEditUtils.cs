using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;

namespace MyCC.Ui.Edit
{
    public interface IEditUtils
    {
        Task Update(FunctionalAccount account);
        Task Delete(FunctionalAccount account);

        void AddWatchedCurrency(string currencyId);
        void RemoveWatchedCurrency(string currencyId);

        void AddReferenceCurrency(string currencyId);
        bool RemoveReferenceCurrency(string currencyId);
        bool ToggleReferenceCurrencyStar(string currencyId);
    }
}