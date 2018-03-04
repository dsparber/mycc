using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Models.Extensions;
using MyCC.Core.Rates.Sources.Utils;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Sources.Implementations
{
    internal class BitPayExchangeRateSource : JsonRateSource
    {
        public override int Id => (int)RateSourceId.BitPay;
        public override RateSourceType Type => RateSourceType.CryptoToFiat;
        public override string Name => ConstantNames.BitPay;

        protected override Uri Uri => new Uri("https://bitpay.com/rates/");

        public override bool IsAvailable(RateDescriptor rateDescriptor) => rateDescriptor.IsBtcToUsdOrEur();


        protected override IEnumerable<(RateDescriptor rateDescriptor, decimal? rate)> GetRatesFromJson(JToken json)
        {
            var rateUsd = json["data"].First(token => token["code"].ToString().Equals(CurrencyConstants.Usd.Code))["rate"].ToDecimal();
            var rateEur = json["data"].First(token => token["code"].ToString().Equals(CurrencyConstants.Eur.Code))["rate"].ToDecimal();

            return new[]
            {
                (RateDescriptorConstants.BtcUsdDescriptor, rateUsd),
                (RateDescriptorConstants.BtcEurDescriptor, rateEur)
            };
        }
    }
}

