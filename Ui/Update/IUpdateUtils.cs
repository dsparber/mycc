using System.Threading.Tasks;

namespace MyCC.Ui.Update
{
    public interface IUpdateUtils
    {
        Task LoadNeededDataFromDatabaseAsync();

        void FetchAllRates();

        void FetchCurrencies();

        void FetchAllAssetsAndRates();

        void FetchNeededButNotLoadedRates();

        void FetchBalancesAndRatesFor(string currencyId);

        void FetchBalanceAndRatesFor(int accountId);

        void FetchCoinInfoAndRateFor(string currencyId);

        void FetchCoinInfoFor(string currencyId);

        void FetchCryptoToFiatRates();

        void ConnectivityChanged(bool connected);
    }
}