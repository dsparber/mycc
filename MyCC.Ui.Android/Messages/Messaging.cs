namespace MyCC.Ui.Android.Messages
{
    public static class Messaging
    {
        // Request Data
        public static class Request
        {
            public const string Rates = "RequestRates";
            public const string Assets = "RequestAssets";
        }

        // Update Data
        public static class Update
        {
            public static readonly string[] AllItems = new[] { Rates, Assets };
            public const string Rates = "UpdateRateItems";
            public const string Assets = "UpdateAssets";
        }


        // Update UIs
        public static class UiUpdate
        {
            public const string RatesOverview = "UiUpdateRatesOverview";
            public const string AssetsTable = "UiUpdateAssetsTable";
        }

    }
}

