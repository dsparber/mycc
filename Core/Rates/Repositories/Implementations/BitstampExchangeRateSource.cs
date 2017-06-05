using System;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.Data;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Repositories.Utils;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Repositories.Implementations
{
    internal class BitstampExchangeRateSource : MultiUriJsonRateSource
    {
        public override RateSourceType Type => RateSourceType.CryptoToFiat;
        public override int Id => (int)RateSourceId.Bitstamp;
        public override string Name => ConstantNames.Bitstamp;

        private static readonly Uri UriUsd = new Uri("https://www.bitstamp.net/api/v2/ticker/btcusd");
        private static readonly Uri UriEur = new Uri("https://www.bitstamp.net/api/v2/ticker/btceur");

        protected override Uri GetUri(RateDescriptor rateDescriptor)
        {
            if (RateDescriptorConstants.BtcUsdDescriptor.CurrenciesEqual(rateDescriptor)) return UriUsd;
            return RateDescriptorConstants.BtcEurDescriptor.CurrenciesEqual(rateDescriptor) ? UriEur : null;
        }

        public override bool IsAvailable(RateDescriptor rateDescriptor) => rateDescriptor.IsBtcToUsdOrEur();

        protected override decimal? GetRateFromJson(JToken json) => json["last"].ToDecimal();
    }
}