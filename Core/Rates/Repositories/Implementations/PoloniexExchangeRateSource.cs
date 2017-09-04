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
    internal class PoloniexExchangeRateSource : JsonRateSource
    {
        public override int Id => (int)RateSourceId.Poloniex;
        public override RateSourceType Type => RateSourceType.Crypto;
        public override string Name => ConstantNames.Poloniex;
        protected override Uri Uri => new Uri("https://poloniex.com/public?command=returnTicker");

        private static IEnumerable<Currency> SupportedCurrencies => CurrencyConstants.FlagPoloniex.Currencies();
        public override bool IsAvailable(RateDescriptor rateDescriptor)
        {
            return SupportedCurrencies.Any(c => c.Id.Equals(rateDescriptor.ReferenceCurrencyId)) &&
                   SupportedCurrencies.Any(c => c.Id.Equals(rateDescriptor.SecondaryCurrencyId));
        }

        protected override IEnumerable<(RateDescriptor rateDescriptor, decimal? rate)> GetRatesFromJson(JToken json) =>
            ((IEnumerable<KeyValuePair<string, JToken>>)json).Select(token => (new RateDescriptor(new Currency(token.Key.Split('_')[1], true).Id, CurrencyConstants.Btc.Id),
            token.Value["last"].ToDecimal())).Where(tuple => tuple.Item2 > 0);

    }
}