using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Repositories.Utils;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Repositories.Implementations
{
    internal class CoinMarketCapExchangeRateSource : JsonRateSource
    {
        public override int Id => (int)RateSourceId.CoinMarketCap;
        public override RateSourceType Type => RateSourceType.Crypto;
        public override string Name => ConstantNames.CoinMarketCap;
        protected override Uri Uri => new Uri("https://api.coinmarketcap.com/v1/ticker/?limit=0");

        private static IEnumerable<Currency> SupportedCurrencies => CurrencyConstants.FlagCoinMarketCap.Currencies();
        public override bool IsAvailable(RateDescriptor rateDescriptor)
        {
            return SupportedCurrencies.Any(c => c.Id.Equals(rateDescriptor.ReferenceCurrencyId)) &&
                   SupportedCurrencies.Any(c => c.Id.Equals(rateDescriptor.SecondaryCurrencyId));
        }

        protected override IEnumerable<(RateDescriptor rateDescriptor, decimal? rate)> GetRatesFromJson(JToken json) =>
            json.Select(token => (new RateDescriptor(new Currency((string)token["symbol"], true).Id, CurrencyConstants.Btc.Id),
            token["price_btc"].ToDecimal())).Where(tuple => tuple.Item2 > 0);

    }
}