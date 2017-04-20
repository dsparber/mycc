namespace MyCC.Ui.Android.Messages
{
    public static class Messaging
    {
        // Request Data
        public static class Request
        {
            public const string Rates = "RequestRates";
            public const string MissingRates = "RequestMissingRates";
            public const string Assets = "RequestAssets";

            public const string CoinInfo = "RequestCoinInfo";
            public const string Account = "RequestRates";
        }

        // Update Data
        public static class Update
        {
            public static readonly string[] AllItems = { Rates, Assets };
            public const string Rates = "UpdateRateItems";
            public const string Assets = "UpdateAssets";
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

            public static readonly string[] ReferenceTables = { CoinInfo, AccountDetail, AccountGroup };
            public static readonly string[] AccountTables = { AccountGroup };

            public static readonly string[] Accounts = { AssetsTable, AssetsGraph, AccountDetail, AccountGroup };

            public static readonly string[] ViewsWithRate = { RatesOverview, AssetsTable, AssetsGraph, CoinInfo };
        }

    }
}

