using MyCC.Core.Currencies;

namespace MyCC.Core.Rates
{
    public static class ExchangeConstants
    {
        public static readonly string DefaultFiatCurrencyId = CurrencyConstants.Eur.Id;
        public static readonly string DefaultCryptoCurrencyId = CurrencyConstants.Btc.Id;
    }
}