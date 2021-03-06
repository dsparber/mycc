using System;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Models.Extensions;
using MyCC.Core.Rates.Sources.Utils;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Sources.Implementations
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

        protected override (decimal? rate, bool inverse) GetRateFromJson(JToken json, RateDescriptor rateDescriptor) => (json["last"].ToDecimal(), !rateDescriptor.ReferenceCurrencyId.Equals(CurrencyConstants.Btc.Id));
    }
}