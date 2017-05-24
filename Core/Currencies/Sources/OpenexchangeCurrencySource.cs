using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Currencies.Sources
{
    public class OpenexchangeCurrencySource : ICurrencySource
    {
        private const string UrlCurrencyList = "https://openexchangerates.org/api/currencies.json";

        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            var uri = new Uri(UrlCurrencyList);
            try
            {
                var response = await HttpHelper.GetAsync(uri);

                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                var currentElements = new List<Currency>();

                foreach (var key in json)
                {
                    var name = (string)key.Value;
                    var code = key.Key.ToUpperInvariant();
                    if (!code.Equals("BTC"))
                    {
                        currentElements.Add(new Currency(code, name, false));
                    }
                }

                return currentElements;
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }

        public string Name => ConstantNames.OpenExchangeRates;
    }
}