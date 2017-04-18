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

			public static readonly string[] ReferenceTables = { CoinInfo };
			public static readonly string[] ViewsWithRate = { RatesOverview, AssetsTable, AssetsGraph, CoinInfo };
		}

	}
}

