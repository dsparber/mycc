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
    internal class CoinbaseExchangeRateSource : MultiUriJsonRateSource
    {
        public override int Id => (int)RateSourceId.Coinbase;
        public override RateSourceType Type => RateSourceType.CryptoToFiat;
        public override string Name => ConstantNames.Coinbase;

        private static readonly Uri UriUsd = new Uri("https://api.coinbase.com/v2/prices/BTC-USD/spot");
        private static readonly Uri UriEur = new Uri("https://api.coinbase.com/v2/prices/BTC-EUR/spot");

        protected override Uri GetUri(RateDescriptor rateDescriptor)
        {
            if (RateDescriptorConstants.BtcUsdDescriptor.CurrenciesEqual(rateDescriptor)) return UriUsd;
            return RateDescriptorConstants.BtcEurDescriptor.CurrenciesEqual(rateDescriptor) ? UriEur : null;
        }


        public CoinbaseExchangeRateSource()
        {
            HttpHeader = ("CB-VERSION", "2016-02-07");
        }


        public override bool IsAvailable(RateDescriptor rateDescriptor) => rateDescriptor.IsBtcToUsdOrEur();
        protected override (decimal? rate, bool inverse) GetRateFromJson(JToken json, RateDescriptor rateDescriptor) => (json["data"]["amount"].ToDecimal(), !rateDescriptor.ReferenceCurrencyId.Equals(CurrencyConstants.Btc.Id));
    }
}