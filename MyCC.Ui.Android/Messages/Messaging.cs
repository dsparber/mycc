namespace MyCC.Ui.Android.Messages
{
    public static class Messaging
    {
        public const string Progress = "Progress";

        // Request Data
        public static class Request
        {
            public const string Rates = "RequestRates";
        }

        // Update Data
        public static class Update
        {
            public static readonly string[] AllItems = new[] { Rates };
            public const string Rates = "UpdateRateItems";
        }


        // Update UIs
        public static class UiUpdate
        {
            public const string RatesOverview = "UiUpdateRatesOverview";
        }

    }
}

