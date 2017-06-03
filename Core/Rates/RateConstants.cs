using MyCC.Core.Currencies;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;

namespace MyCC.Core.Rates
{
    public static class RateConstants
    {
        public static readonly string DefaultFiatCurrencyId = CurrencyConstants.Eur.Id;
        public static readonly string DefaultCryptoCurrencyId = CurrencyConstants.Btc.Id;

        public static readonly RateDescriptor BtcUsdDescriptor = new RateDescriptor(CurrencyConstants.Btc.Id, CurrencyConstants.Usd.Id);
        public static readonly RateDescriptor BtcEurDescriptor = new RateDescriptor(CurrencyConstants.Btc.Id, CurrencyConstants.Usd.Id);
    }

}