using System;
using System.Collections.Generic;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.Data;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Repositories.Utils;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Repositories.Implementations
{
    internal class KrakenExchangeRateSource : JsonRateSource
    {
        public override int Id => (int)RateSourceId.Kraken;
        public override RateSourceType Type => RateSourceType.CryptoToFiat;
        public override string Name => ConstantNames.Kraken;

        protected override Uri Uri => new Uri("https://api.kraken.com/0/public/Ticker?pair=XXBTZEUR,XXBTZUSD");


        public override bool IsAvailable(RateDescriptor rateDescriptor) => rateDescriptor.IsBtcToUsdOrEur();


        protected override IEnumerable<(RateDescriptor rateDescriptor, decimal? rate)> GetRatesFromJson(JToken json) => new[]
        {
            (RateDescriptorConstants.BtcUsdDescriptor, json["result"]["XXBTZUSD"]["a"][0].ToDecimal()),
            (RateDescriptorConstants.BtcEurDescriptor, json["result"]["XXBTZEUR"]["a"][0].ToDecimal())
        };
    }
}