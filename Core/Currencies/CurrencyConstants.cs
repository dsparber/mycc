namespace MyCC.Core.Currencies
{
    public static class CurrencyConstants
    {
        public static readonly Model.Currency Btc = new Model.Currency("BTC", "Bitcoin", true);
        public static readonly Model.Currency Eur = new Model.Currency("EUR", "Euro", false);
        public static readonly Model.Currency Usd = new Model.Currency("USD", "US Dollar", false);

        public const int FlagBittrex = 1 << 0;
        public const int FlagBlockExperts = 1 << 1;
        public const int FlagCryptoId = 1 << 2;
        public const int FlagCryptonator = 1 << 3;
    }
}