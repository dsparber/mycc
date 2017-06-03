using System;
using System.Collections.Generic;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Repositories.Implementations
{
    public class QuadrigaCxExchangeRateSource : JsonRateSource
    {
        public override RateSourceId Id => RateSourceId.QuadrigaCx;
        public override RateSourceType Type => RateSourceType.CryptoToFiat;
        public override string Name => ConstantNames.QuadrigaCx;

        protected override Uri Uri => new Uri("https://api.quadrigacx.com/v2/ticker?book=btc_usd");

        public override bool IsAvailable(RateDescriptor rateDescriptor) => rateDescriptor.IsBtcToUsd();

        protected override IEnumerable<(RateDescriptor rateDescriptor, decimal? rate)> GetRatesFromJson(JToken json) => new[]
        {
            (RateConstants.BtcUsdDescriptor, json["last"].ToDecimal()),
        };
    }
}