using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Models;
using Newtonsoft.Json.Linq;

namespace MyCryptos.Core.Repositories.Currency
{
    public class BittrexCurrencyRepository : OnlineCurrencyRepository
    {
        const string URL_CURRENCY_LIST = "https://bittrex.com/api/v1.1/public/getcurrencies";

        const string CURRENCY_LIST_RESULT = "result";
        const string CURRENCY_LIST_RESULT_NAME = "CurrencyLong";
        const string CURRENCY_LIST_RESULT_CURRENCY = "Currency";

        const int BUFFER_SIZE = 256000;

        readonly HttpClient client;

        public BittrexCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY, name)
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = BUFFER_SIZE;
        }

        protected async override Task<IEnumerable<Models.Currency>> GetCurrencies()
        {
            var uri = new Uri(URL_CURRENCY_LIST);

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                var result = (JArray)json[CURRENCY_LIST_RESULT];

                var currentElements = new List<Models.Currency>();

                foreach (var token in result)
                {
                    var name = (string)token[CURRENCY_LIST_RESULT_NAME];
                    var code = (string)token[CURRENCY_LIST_RESULT_CURRENCY];
                    var c = new Models.Currency(code, name);
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