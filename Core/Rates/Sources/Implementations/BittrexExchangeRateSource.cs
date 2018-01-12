using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Repositories.Utils;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Repositories.Implementations
{
    internal class BittrexExchangeRateSource : JsonRateSource
    {
        public override int Id => (int)RateSourceId.Bittrex;
        public override RateSourceType Type => RateSourceType.Crypto;
        public override string Name => ConstantNames.Bittrex;

        protected override Uri Uri => new Uri("https://bittrex.com/api/v1.1/public/getmarketsummaries");

        private const string ResultKey = "result";
        private const string RateKey = "Last";
        private const string MarketKey = "MarketName";

        public override bool IsAvailable(RateDescriptor rateDescriptor)
        {
            if (!rateDescriptor.ContainsCurrency(CurrencyConstants.Btc.Id)) return false;

            var currency = rateDescriptor.GetCurrencyApartFrom(CurrencyConstants.Btc.Id).Find();
            return currency.IsSet(CurrencyConstants.FlagRatesBittrex);
        }

        protected override IEnumerable<(RateDescriptor rateDescriptor, decimal? rate)> GetRatesFromJson(JToken json)
        {
            return json[ResultKey].Select(token =>
            {
                var market = token[MarketKey].ToString().Split('-');
                var rate = token[RateKey].ToDecimal();
                var referenceCurrencyCode = market[1];
                var secondaryCurrencyCode = market[0];

                referenceCurrencyCode = "BCC".Equals(referenceCurrencyCode) ? "BCH" : referenceCurrencyCode;
                secondaryCurrencyCode = "BCC".Equals(secondaryCurrencyCode) ? "BCH" : secondaryCurrencyCode;

                var rateDescriptor = new RateDescriptor(new Currency(referenceCurrencyCode, true).Id, new Currency(secondaryCurrencyCode, true).Id);

                return (rateDescriptor, rate);
            });
        }
    }
}