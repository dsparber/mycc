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
    internal class CryptonatorExchangeRateSource : MultiUriJsonRateSource
    {
        public override int Id => (int)RateSourceId.Cryptonator;
        public override RateSourceType Type => RateSourceType.Crypto;
        public override string Name => ConstantNames.Cryptonator;

        private const string BaseUri = "https://api.cryptonator.com/api/ticker/{0}";
        protected override Uri GetUri(RateDescriptor rateDescriptor) => new Uri(string.Format(BaseUri, ToUrl(rateDescriptor)));

        private static IEnumerable<Currency> SupportedCurrencies => CurrencyConstants.FlagCryptonator.Currencies();
        public override bool IsAvailable(RateDescriptor rateDescriptor)
        {
            return SupportedCurrencies.Any(c => c.Id.Equals(rateDescriptor.ReferenceCurrencyId)) &&
                   SupportedCurrencies.Any(c => c.Id.Equals(rateDescriptor.SecondaryCurrencyId));
        }

        protected override decimal? GetRateFromJson(JToken json) => json["ticker"]["price"].ToDecimal();

        private static string ToUrl(RateDescriptor rateDescriptor)
        {
            return $"{rateDescriptor.ReferenceCurrencyId.Code().ToLower()}-{rateDescriptor.SecondaryCurrencyId.Code().ToLower()}";
        }
    }
}