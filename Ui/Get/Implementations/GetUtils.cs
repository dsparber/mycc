namespace MyCC.Ui.Get.Implementations
{
    internal class GetUtils : IGetUtils
    {
        private readonly RatesOverviewData _ratesOverviewData;
        private readonly AssetsOverviewData _assetsOverviewData;
        private readonly CoinInfoViewData _coinInfoViewData;
        private readonly AccountDetailViewData _accountDetailViewData;
        private readonly AccountsGroupViewData _accountsGroupViewData;

        public GetUtils()
        {
            _ratesOverviewData = new RatesOverviewData();
            _assetsOverviewData = new AssetsOverviewData();
            _coinInfoViewData = new CoinInfoViewData();
            _accountDetailViewData = new AccountDetailViewData();
            _accountsGroupViewData = new AccountsGroupViewData();
        }

        public IRatesOverviewData Rates => _ratesOverviewData;
        public ICoinInfoViewData CoinInfoViewData => _coinInfoViewData;
        public IAccountDetailViewData AccountDetail => _accountDetailViewData;
        public IAccountsGroupViewData AccountsGroup => _accountsGroupViewData;
        public IAssetsOverviewData Assets => _assetsOverviewData;
    }
}