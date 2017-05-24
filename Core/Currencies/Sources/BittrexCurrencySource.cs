using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Currencies.Sources
{
    public class BittrexCurrencySource : ICurrencySource
    {
        private const string UrlCurrencyList = "https://bittrex.com/api/v1.1/public/getcurrencies";

        public string Name => ConstantNames.Bittrex;

        private const string CurrencyListResult = "result";
        private const string CurrencyListResultName = "CurrencyLong";
        private const string CurrencyListResultCurrency = "Currency";

        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            try
            {
                var response = await HttpHelper.GetAsync(new Uri(UrlCurrencyList));

                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                var result = (JArray)json[CurrencyListResult];

                return (from token in result
                        let name = (string)token[CurrencyListResultName]
                        let code = (string)token[CurrencyListResultCurrency]
                        select new Currency(code, name, true) { BalanceSourceFlags = CurrencyConstants.FlagBittrex }).ToList();
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }
    }
}