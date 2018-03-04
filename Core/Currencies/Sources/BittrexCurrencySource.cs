using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Currencies.Sources
{
    public class BittrexCurrencySource : ICurrencySource
    {
        private const string UrlCurrencyList = "https://bittrex.com/api/v1.1/public/getcurrencies";

        public string Name => ConstantNames.Bittrex;

        public IEnumerable<int> Flags => new[]{CurrencyConstants.FlagRatesBittrex};

        private const string CurrencyListResult = "result";
        private const string CurrencyListResultName = "CurrencyLong";
        private const string CurrencyListResultCurrency = "Currency";

        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            try
            {
                var response = await new Uri(UrlCurrencyList).GetResponse();

                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                var result = (JArray)json[CurrencyListResult];

                var rates = (await new Uri("https://bittrex.com/api/v1.1/public/getmarketsummaries").GetJson())["result"];
                var rateCodes = rates.Select(token => (string)token["MarketName"])
                                     .Where(market => market.StartsWith("BTC-", StringComparison.Ordinal))
                                     .Select(market => market.Split('-')[1].ToUpper()).ToList();

                return (from token in result
                        let name = (string)token[CurrencyListResultName]
                        let code = (string)token[CurrencyListResultCurrency]
                        let flag = rateCodes.Contains(code) ? CurrencyConstants.FlagRatesBittrex : 0
                        let codeFixed = "BCC".Equals(code) ? "BCH" : code
                        select new Currency(codeFixed, name, true) { BalanceSourceFlags = flag }).ToList();
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }
    }
}