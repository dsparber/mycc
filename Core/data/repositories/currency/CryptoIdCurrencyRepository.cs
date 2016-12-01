using data.database.models;
using data.repositories.currency;
using MyCryptos.models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyCryptos.data.repositories.currency
{
    class CryptoIdCurrencyRepository : OnlineCurrencyRepository
    {
        const string URL_CURRENCY_LIST = "http://chainz.cryptoid.info/explorer/api.dws?q=summary";

        const string JSON_KEY_NAME = "name";

        const int BUFFER_SIZE = 256000;
        readonly HttpClient client;

        public CryptoIdCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_CRYPTOID_REPOSITORY, name)
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = BUFFER_SIZE;
        }

        protected async override Task<IEnumerable<Currency>> GetCurrencies()
        {
            var uri = new Uri(URL_CURRENCY_LIST);

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                var currentElements = new List<Currency>();

                foreach (string key in json.Properties().Select(p => p.Name))
                {
                    var name = (string)(json[key] as JObject)[JSON_KEY_NAME];
                    var c = new Currency(key, name);
                    currentElements.Add(c);
                }
                await Task.WhenAll(Elements.Where(e => !currentElements.Contains(e)).Select(e => Remove(e)));

                LastFetch = DateTime.Now;
                return currentElements;
            }
            return null;
        }
    }
}
