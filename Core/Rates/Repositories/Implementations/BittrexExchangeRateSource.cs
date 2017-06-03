using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.ModelExtensions;
using MyCC.Core.Rates.Models;
using MyCC.Core.Resources;

namespace MyCC.Core.Rates.Repositories.Implementations
{
    public class BittrexExchangeRateSource : IRateSource
    {
        private const string Url = "https://bittrex.com/api/v1.1/public/getmarketsummaries";

        private const string ResultKey = "result";
        private const string RateKey = "Last";
        private const string MarketKey = "MarketName";


        public async Task<IEnumerable<ExchangeRate>> FetchRates(IEnumerable<RateDescriptor> rateDescriptors)
        {
            var uri = new Uri(Url);
            try
            {
                var json = await uri.GetJson();

                return json[ResultKey].Select(token =>
                {
                    var market = token[MarketKey].ToString().Split('-');
                    var rate = token[RateKey].ToDecimal();
                    var referenceCurrencyCode = market[0];
                    var secondaryCurrencyCode = market[1];
                    var rateDescriptor = new RateDescriptor(new Currency(referenceCurrencyCode, true).Id, new Currency(secondaryCurrencyCode, true).Id);

                    if (!rate.HasValue || !rateDescriptors.Contains(rateDescriptor)) return null;

                    return new ExchangeRate(rateDescriptor, rate.Value, (int)Id, DateTime.Now);
                }).Where(rate => rate != null);
            }
            catch (Exception e)
            {
                e.LogError();
                return new List<ExchangeRate>();
            }
        }

        public RateSourceId Id => RateSourceId.Bittrex;

        public bool IsAvailable(RateDescriptor rateDescriptor)
        {
            if (!rateDescriptor.ContainsCurrency(CurrencyConstants.Btc.Id)) return false;

            var currency = rateDescriptor.GetCurrencyApartFrom(CurrencyConstants.Btc.Id).Find();
            return currency.IsSet(CurrencyConstants.FlagBittrex);
        }

        public RateSourceType Type => RateSourceType.Crypto;

        public string Name => ConstantNames.Bittrex;

    }
}

