namespace MyCC.Ui.Messages
{
    public static class Messaging
    {
        public static class Status
        {
            public const string Progress = "StatusProgress";
            public const string Network = "StatusNetwork";
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
