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
    internal class CryptoIdCurrencySource : ICurrencySource
    {
        private const string UrlCurrencyList = "https://chainz.cryptoid.info/2give/api.dws?q=summary";

        public string Name => ConstantNames.CryptoId;
        public IEnumerable<int> Flags => new[] { CurrencyConstants.FlagCryptoId };


        private const string JsonKeyName = "name";


        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            var uri = new Uri(UrlCurrencyList);

            try
            {
                var response = await uri.GetResponse();

                if (!response.IsSuccessStatusCode) return new List<Currency>();

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
