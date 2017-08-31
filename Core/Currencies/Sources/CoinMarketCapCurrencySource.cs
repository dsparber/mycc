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
    public class CoinMarketCapCurrencySource : ICurrencySource
    {
        private const string UrlCurrencyList = "https://api.coinmarketcap.com/v1/ticker/";

        public string Name => ConstantNames.CoinMarketCap;


        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            try
            {
                var response = (JArray)await new Uri(UrlCurrencyList).GetJson();
                return response.Select(token => new Currency((string)token["symbol"], (string)token["name"], true));
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }
    }
}