using System.Threading.Tasks;
using MyCC.Core.Account.Models.Base;

namespace MyCC.Ui.Update
{
    public interface IUpdateUtils
    {
        Task LoadNeededDataFromDatabase();

        void FetchAllRates();

        void FetchCurrencies();

        void FetchAllAssetsAndRates();

        void FetchNeededButNotLoadedRates();

        void FetchBalancesAndRatesFor(string currencyId);

        void FetchBalanceAndRatesFor(FunctionalAccount account);

        void FetchCoinInfoAndRatesFor(string currencyId);

        void FetchCoinInfoFor(string currencyId);

        void FetchCryptoToFiatRates();

        void CreateRatesData(); // TODO make internal

        void CreateAssetsData(); // TODO make internal

        void ConnectivityChanged(bool connected);
    }
}