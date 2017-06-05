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
    internal class BtceExchangeRateSource : JsonRateSource
    {
        public override int Id => (int)RateSourceId.Btce;
        public override RateSourceType Type => RateSourceType.CryptoToFiat;
        public override string Name => ConstantNames.Btce;

        protected override Uri Uri => new Uri("https://btc-e.com/api/3/ticker/btc_usd-btc_eur");
        private const string Key = "last";


        public override bool IsAvailable(RateDescriptor rateDescriptor) => rateDescriptor.IsBtcToUsdOrEur();


        protected override IEnumerable<(RateDescriptor rateDescriptor, decimal? rate)> GetRatesFromJson(JToken json) => new[]
        {
            (RateDescriptorConstants.BtcUsdDescriptor, json["btc_usd"][Key].ToDecimal()),
            (RateDescriptorConstants.BtcEurDescriptor, json["btc_eur"][Key].ToDecimal())
        };
    }
}