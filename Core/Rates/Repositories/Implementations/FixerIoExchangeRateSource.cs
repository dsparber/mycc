using System;
using System.Collections.Generic;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Repositories.Implementations
{
    public class FixerIoExchangeRateSource : JsonRateSource
    {
        public override RateSourceType Type => RateSourceType.Fiat;
        public override RateSourceId Id => RateSourceId.FixerIo;
        public override string Name => ConstantNames.FixerIo;

        protected override Uri Uri => new Uri("http://api.fixer.io/latest");

        private const string JsonKeyRates = "rates";

        public override bool IsAvailable(RateDescriptor rateDescriptor)
        {
            return rateDescriptor.ContainsCurrency(CurrencyConstants.Eur.Id) && rateDescriptor.GetCurrencyApartFrom(CurrencyConstants.Eur.Id).IsFiat();
        }

        protected override IEnumerable<(RateDescriptor rateDescriptor, decimal? rate)> GetRatesFromJson(JToken json)
        {
            var result = new List<(RateDescriptor rateDescriptor, decimal? rate)>();
            foreach (var token in (JObject)json[JsonKeyRates])
            {
                var rate = token.Value.ToDecimal();
                var rateDescriptor = new RateDescriptor(CurrencyConstants.Eur.Id, new Currency(token.Key, false).Id);
                result.Add((rateDescriptor, rate));
            }
            return result;
        }
    }
}