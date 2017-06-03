using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;
using SQLite;

namespace MyCC.Core.Rates.Repositories.Implementations
{
    public class BitfinexExchangeRateSource : JsonRateSource
    {
        public override RateSourceId Id => RateSourceId.Bitfinex;
        public override RateSourceType Type => RateSourceType.CryptoToFiat;
        public override string Name => ConstantNames.Bitfinex;

        protected override Uri Uri => new Uri("https://api.bitfinex.com/v2/ticker/tBTCUSD");

        private const int PositionValue = 6;


        public override bool IsAvailable(RateDescriptor rateDescriptor) =>
            rateDescriptor.ContainsCurrency(CurrencyConstants.Btc.Id) &&
            rateDescriptor.ContainsCurrency(CurrencyConstants.Usd.Id);


        protected override IEnumerable<ExchangeRate> GetRatesFromJson(JToken json)
        {
            var rate = json[PositionValue].ToDecimal();
            var descriptor = new RateDescriptor(CurrencyConstants.Btc.Id, CurrencyConstants.Eur.Id);
            return rate.HasValue ? new[] { new ExchangeRate(descriptor, rate.Value, (int)Id, DateTime.Now) } : null;
        }
    }
}

