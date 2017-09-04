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
    public class PoloniexCurrencySource : ICurrencySource
    {
        private const string UrlCurrencyList = "https://poloniex.com/public?command=returnCurrencies";

        public string Name => ConstantNames.Poloniex;


        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            try
            {
                var response = (IEnumerable<KeyValuePair<string, JToken>>)await new Uri(UrlCurrencyList).GetJson();
                return response.Select(token => new Currency(token.Key, (string)token.Value["name"], true) { BalanceSourceFlags = CurrencyConstants.FlagPoloniex });
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }
    }
}