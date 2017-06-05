namespace MyCC.Ui.Messages
{
    public static class Messaging
    {
        // Request Data
        public static class Request
        {
            public const string AllRates = "RequestAllRates";
            public const string AllAssetsAndRates = "RequestAllAssets";

            public const string DataForNewAccount = "RequestDataForNewAccount";

            public const string AccountsByCurrency = "RequestAccountsByCurrency";
            public const string RateAndInfo = "RequestCurrencyAndInfo";
            public const string InfoForCurrency = "RequestInfoForCurrency";
            public const string SingleAccount = "RequestSingleAccount";

            public const string BitcoinExchangeSources = "RequestBitcoinExchangeSources";
        }

        public static class Status
        {
            public const string Progress = "StatusProgress";
            public const string Network = "StatusNetwork";
        }

        // Settings changed
        public static class SettingChange
        {
            public const string MainCurrencies = "SettingChangeMainCurrencies";
        }


        // Update UIs
        public static class UiUpdate
        {
            public const string RatesOverview = "UiUpdateRatesOverview";
            public const string AssetsTable = "UiUpdateAssetsTable";
            public const string AssetsGraph = "UiUpdateAssetsGraph";

            public const string CoinInfo = "UiUpdateCoinInfo";
            public const string AccountDetail = "UiUpdateAccountDetail";
            public const string AccountGroup = "UiUpdateAccountGroup";

            public const string BitcoinExchangeSources = "UiUpdateBitcoinExchangeSources";

            public static readonly string[] ReferenceTables = { CoinInfo, AccountDetail, AccountGroup };

            public static readonly string[] Accounts = { AssetsTable, AssetsGraph, AccountDetail, AccountGroup };

            public static readonly string[] ViewsWithRate = { AssetsTable, AssetsGraph, RatesOverview, CoinInfo, AccountDetail, AccountGroup };
        }

    }
}
