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
    public class CryptonatorCurrencySource : ICurrencySource
    {
        private const string UrlCurrencyList = "https://api.cryptonator.com/api/currencies";

        public string Name => ConstantNames.Cryptonator;
        public IEnumerable<int> Flags => new[] { CurrencyConstants.FlagCryptonator };


        private const string CurrencyListResult = "rows";
        private const string CurrencyListResultName = "name";
        private const string CurrencyListResultCurrency = "code";

        private readonly IEnumerable<string> _currencyBlacklist = new[] { "CAD", "CNY", "EUR", "GBP", "JPY", "UAH", "USD" };

        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            var uri = new Uri(UrlCurrencyList);

            try
            {
                var response = await HttpHelper.GetResponse(uri);
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                var result = (JArray)json[CurrencyListResult];


                return (from token in result
                        let name = (string)token[CurrencyListResultName]
                        let code = (string)token[CurrencyListResultCurrency]
                        select new Currency(code, name, true) { BalanceSourceFlags = CurrencyConstants.FlagCryptonator })
                        .Where(c => !_currencyBlacklist.Contains(c.Code)).ToList();
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }
    }
}