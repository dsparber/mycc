using System;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Repositories.Implementations
{
    public class ItBitExchangeRateSource : MultiUriJsonRateSource
    {
        private static readonly Uri UriUsd = new Uri("https://api.itbit.com/v1/markets/XBTUSD/ticker");
        private static readonly Uri UriEur = new Uri("https://api.itbit.com/v1/markets/XBTEUR/ticker");

        public override RateSourceType Type => RateSourceType.CryptoToFiat;
        public override RateSourceId Id => RateSourceId.ItBit;
        public override string Name => ConstantNames.ItBit;

        protected override Uri GetUri(RateDescriptor rateDescriptor)
        {
            if (RateConstants.BtcUsdDescriptor.CurrenciesEqual(rateDescriptor)) return UriUsd;
            return RateConstants.BtcEurDescriptor.CurrenciesEqual(rateDescriptor) ? UriEur : null;
        }

        public override bool IsAvailable(RateDescriptor rateDescriptor) => rateDescriptor.IsBtcToUsdOrEur();

        protected override decimal? GetRateFromJson(JToken json) => json["lastPrice"].ToDecimal();
    }
}