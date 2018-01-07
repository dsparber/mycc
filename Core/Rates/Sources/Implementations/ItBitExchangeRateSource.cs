using System;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Repositories.Utils;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Repositories.Implementations
{
    internal class ItBitExchangeRateSource : MultiUriJsonRateSource
    {
        private static readonly Uri UriUsd = new Uri("https://api.itbit.com/v1/markets/XBTUSD/ticker");
        private static readonly Uri UriEur = new Uri("https://api.itbit.com/v1/markets/XBTEUR/ticker");

        public override RateSourceType Type => RateSourceType.CryptoToFiat;
        public override int Id => (int)RateSourceId.ItBit;
        public override string Name => ConstantNames.ItBit;

        protected override Uri GetUri(RateDescriptor rateDescriptor)
        {
            if (RateDescriptorConstants.BtcUsdDescriptor.CurrenciesEqual(rateDescriptor)) return UriUsd;
            return RateDescriptorConstants.BtcEurDescriptor.CurrenciesEqual(rateDescriptor) ? UriEur : null;
        }

        public override bool IsAvailable(RateDescriptor rateDescriptor) => rateDescriptor.IsBtcToUsdOrEur();

        protected override (decimal? rate, bool inverse) GetRateFromJson(JToken json, RateDescriptor rateDescriptor) => (json["lastPrice"].ToDecimal(), !rateDescriptor.ReferenceCurrencyId.Equals(CurrencyConstants.Btc.Id));
    }
}