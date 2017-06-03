using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Rates.Repositories.Implementations
{
    public class BitPayExchangeRateSource : JsonRateSource
    {
        public override RateSourceId Id => RateSourceId.BitPay;
        public override RateSourceType Type => RateSourceType.CryptoToFiat;
        public override string Name => ConstantNames.BitPay;

        protected override Uri Uri => new Uri("https://bitpay.com/rates/");

        private const string KeyData = "data";
        private const string KeyCoin = "code";
        private const string KeyValue = "rate";


        public override bool IsAvailable(RateDescriptor rateDescriptor) =>
            rateDescriptor.ContainsCurrency(CurrencyConstants.Btc.Id) &&
            (rateDescriptor.ContainsCurrency(CurrencyConstants.Usd.Id) || rateDescriptor.ContainsCurrency(CurrencyConstants.Eur.Id));


        protected override IEnumerable<ExchangeRate> GetRatesFromJson(JToken json)
        {
            var data = json[KeyData] as JArray;
            var rateUsd = data.First(token => token[KeyCoin].ToString().Equals(CurrencyConstants.Usd.Code))[KeyValue].ToDecimal();
            var rateEur = data.First(token => token[KeyCoin].ToString().Equals(CurrencyConstants.Eur.Code))[KeyValue].ToDecimal();

            var rates = new List<ExchangeRate>();
            if (rateUsd.HasValue)
                rates.Add(new ExchangeRate(new RateDescriptor(CurrencyConstants.Btc.Id, CurrencyConstants.Usd.Id), rateUsd.Value, (int)Id, DateTime.Now));
            if (rateEur.HasValue)
                rates.Add(new ExchangeRate(new RateDescriptor(CurrencyConstants.Btc.Id, CurrencyConstants.Eur.Id), rateEur.Value, (int)Id, DateTime.Now));

            return rates;
        }
    }
}

