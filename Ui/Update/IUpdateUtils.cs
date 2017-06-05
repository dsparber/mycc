using MyCC.Core.Account.Models.Base;
using MyCC.Ui.Messages;

namespace MyCC.Ui.Update
{
    public interface IUpdateUtils
    {
        void FetchAllRates();

        void FetchAllAssetsAndRates();

        void FetchNeededButNotLoaded();

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