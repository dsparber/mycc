using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyCC.Core.Currency.Database;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Currency.Repositories
{
    internal class CryptoIdCurrencyRepository : OnlineCurrencyRepository
    {
        private const string UrlCurrencyList = "http://chainz.cryptoid.info/explorer/api.dws?q=summary";

        private const string JsonKeyName = "name";

        private const int BufferSize = 256000;
        private readonly HttpClient _client;

        public CryptoIdCurrencyRepository(int id) : base(id)
        {
            _client = new HttpClient { MaxResponseContentBufferSize = BufferSize };
        }
        public override int RepositoryTypeId => CurrencyRepositoryDbm.DB_TYPE_CRYPTOID_REPOSITORY;

        protected override async Task<IEnumerable<Model.Currency>> GetCurrencies()
        {
            var uri = new Uri(UrlCurrencyList);

            var response = await _client.GetAsync(uri);

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);

            var currentElements = (from key in json.Properties().Select(p => p.Name) let name = (string)json[key][JsonKeyName] select new Model.Currency(key, name)).ToList();

            await Task.WhenAll(Elements.Where(e => !currentElements.Contains(e)).Select(Remove));

            LastFetch = DateTime.Now;
            return currentElements;
        }
    }
}
