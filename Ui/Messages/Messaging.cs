namespace MyCC.Ui.Messages
{
    public static class Messaging
    {
        public static class Status
        {
            public const string Progress = "StatusProgress";
            public const string Network = "StatusNetwork";

            public const string DarkStatusBar = "StatusDarkStatusBar";
            public const string CarouselPosition = "StatusCarouselPosition";
        }

        public static class Update
        {
            public const string Rates = "UpdateRates";
            public const string Balances = "UpdateAssets";
            public const string CoinInfos = "UpdateCoinInfo";
            public const string CryptoToFiatRates = "UpdateBitcoinExchangeSources";
        }

        public static class Modified
        {
            public const string Rates = "ModifiedRates";
            public const string Balances = "ModifiedAssets";
            public const string CoinInfos = "ModifiedCoinInfo";
            public const string CryptoToFiatRates = "ModifiedBitcoinExchangeSources";
        }

        public static class Sort
        {
            public const string ReferenceTables = "SortReferenceTables";
            public const string Rates = "SortRates";
            public const string Assets = "SortAssets";
            public const string Accounts = "SortAccounts";
        }
    }
}
