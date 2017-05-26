using MyCC.Core.Currencies.Models;

namespace MyCC.Core.Currencies
{
    public static class CurrencyConstants
    {
        public static readonly Currency Btc = new Currency("BTC", "Bitcoin", true);
        public static readonly Currency Eur = new Currency("EUR", "Euro", false);
        public static readonly Currency Usd = new Currency("USD", "US Dollar", false);

        public const int FlagBittrex = 1 << 0;
        public const int FlagBlockExperts = 1 << 1;
        public const int FlagCryptoId = 1 << 2;
        public const int FlagCryptonator = 1 << 3;
    }
}