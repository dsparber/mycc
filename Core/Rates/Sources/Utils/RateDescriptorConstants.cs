using MyCC.Core.Currencies;
using MyCC.Core.Rates.Models;

namespace MyCC.Core.Rates.Repositories.Utils
{
    internal static class RateDescriptorConstants
    {
        public static readonly RateDescriptor BtcUsdDescriptor = new RateDescriptor(CurrencyConstants.Btc.Id, CurrencyConstants.Usd.Id);
        public static readonly RateDescriptor BtcEurDescriptor = new RateDescriptor(CurrencyConstants.Btc.Id, CurrencyConstants.Eur.Id);
    }

}