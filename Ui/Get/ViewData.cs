using MyCC.Ui.Messages;

namespace MyCC.Ui.Get
{
    public class ViewData
    {
        public static RatesViewData Rates => _instance._ratesViewData;
        public static AssetsViewData Assets => _instance._assetsViewData;
        public static CoinInfoViewData CoinInfo => _instance._coinInfoViewData;
        public static AccountDetailViewData AccountDetail => _instance._accountDetailViewData;
        public static AccountsGroupViewData AccountGroup => _instance._accountsGroupViewData;

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
        }

        public static void Init()
        {
            _instance = new ViewData();
        }

        private static ViewData _instance;
    }
}