using System;
using System.Collections.Generic;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Repositories.Implementations
{
    public class BtceExchangeRateSource : JsonRateSource
    {
        public override RateSourceId Id => RateSourceId.Btce;
        public override RateSourceType Type => RateSourceType.CryptoToFiat;
        public override string Name => ConstantNames.Btce;

        protected override Uri Uri => new Uri("https://btc-e.com/api/3/ticker/btc_usd-btc_eur");
        private const string Key = "last";


        public override bool IsAvailable(RateDescriptor rateDescriptor) => rateDescriptor.IsBtcToUsdOrEur();


        protected override IEnumerable<(RateDescriptor rateDescriptor, decimal? rate)> GetRatesFromJson(JToken json) => new[]
        {
            (RateConstants.BtcUsdDescriptor, json["btc_usd"][Key].ToDecimal()),
            (RateConstants.BtcEurDescriptor, json["btc_eur"][Key].ToDecimal())
        };
    }
}