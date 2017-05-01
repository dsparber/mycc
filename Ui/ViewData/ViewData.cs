using MyCC.Ui.Messages;
using MyCC.Ui.Tasks;

namespace MyCC.Ui.ViewData
{
    public class ViewData
    {
        public static RatesViewData Rates => Instance._ratesViewData;
        public static AssetsViewData Assets => Instance._assetsViewData;
        public static CoinInfoViewData CoinInfo => Instance._coinInfoViewData;
        public static AccountDetailViewData AccountDetail => Instance._accountDetailViewData;
        public static AccountsGroupViewData AccountGroup => Instance._accountsGroupViewData;

        private readonly RatesViewData _ratesViewData;
        private readonly AssetsViewData _assetsViewData;
        private readonly CoinInfoViewData _coinInfoViewData;
        private readonly AccountDetailViewData _accountDetailViewData;
        private readonly AccountsGroupViewData _accountsGroupViewData;

        private ViewData()
        {
            _ratesViewData = new RatesViewData();
            _assetsViewData = new AssetsViewData();
            _coinInfoViewData = new CoinInfoViewData();
            _accountDetailViewData = new AccountDetailViewData();
            _accountsGroupViewData = new AccountsGroupViewData();

            Messaging.Update.Rates.Subscribe(this, () =>
            {
                _ratesViewData.UpdateRateItems();
                Messaging.UiUpdate.ViewsWithRate.Send();
            });
            Messaging.Update.Assets.Subscribe(this, () =>
            {
                _assetsViewData.UpdateRateItems();
                Messaging.UiUpdate.Accounts.Send();
            });


            Messaging.Request.AllRates.Subscribe(this, TaskHelper.UpdateAllRates);
            Messaging.Request.AllAssetsAndRates.Subscribe(this, TaskHelper.UpdateAllAssetsAndRates);

            Messaging.Request.DataForNewAccount.Subscribe(this, TaskHelper.UpdateDataForNewAccount);

            Messaging.Request.SingleAccount.Subscribe(this, TaskHelper.UpdateBalanceAndRatesForAccount);
            Messaging.Request.AccountsByCurrency.Subscribe(this, TaskHelper.UpdateBalancesAndRatesForCurrency);

            Messaging.Request.InfoForCurrency.Subscribe(this, TaskHelper.FetchCoinInfo);
            Messaging.Request.RateAndInfo.Subscribe(this, TaskHelper.FetchCoinInfoAndRates);
        }


        private static readonly ViewData Instance = new ViewData();

    }
}