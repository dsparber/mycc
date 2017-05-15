using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Helpers;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Currencies.Sources
{
    internal class CryptoIdCurrencySource : ICurrencySource
    {
        private const string UrlCurrencyList = "https://chainz.cryptoid.info/explorer/api.dws?q=summary";

        public string Name => "CryptoId";


        private const string JsonKeyName = "name";


        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            var uri = new Uri(UrlCurrencyList);

            try
            {
                var response = await HttpHelper.GetAsync(uri);

                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                return (from key in json.Properties().Select(p => p.Name) let name = (string)json[key][JsonKeyName] select new Currency(key, name, true) { BalanceSourceFlags = CurrencyConstants.FlagCryptoId }).ToList();
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }
    }
}
