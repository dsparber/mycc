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
    internal class CoinapultExchangeRateSource : MultiUriJsonRateSource
    {
        public override RateSourceType Type => RateSourceType.CryptoToFiat;
        public override int Id => (int)RateSourceId.Coinapult;
        public override string Name => ConstantNames.Coinapult;

        private static readonly Uri UriUsd = new Uri("https://api.coinapult.com/api/ticker?market=USD_BTC");
        private static readonly Uri UriEur = new Uri("https://api.coinapult.com/api/ticker?market=EUR_BTC");

        protected override Uri GetUri(RateDescriptor rateDescriptor)
        {
            if (RateDescriptorConstants.BtcUsdDescriptor.CurrenciesEqual(rateDescriptor)) return UriUsd;
            return RateDescriptorConstants.BtcEurDescriptor.CurrenciesEqual(rateDescriptor) ? UriEur : null;
        }

        public override bool IsAvailable(RateDescriptor rateDescriptor) => rateDescriptor.IsBtcToUsdOrEur();
        protected override (decimal? rate, bool inverse) GetRateFromJson(JToken json, RateDescriptor rateDescriptor) => (json["small"]["ask"].ToDecimal(), !rateDescriptor.ReferenceCurrencyId.Equals(CurrencyConstants.Btc.Id));
    }
}