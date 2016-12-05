using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MyCryptos.Core.Currency.Database;
using Newtonsoft.Json.Linq;

namespace MyCryptos.Core.Currency.Repositories
{
    public class CryptonatorCurrencyRepository : OnlineCurrencyRepository
    {
        const string URL_CURRENCY_LIST = "https://api.cryptonator.com/api/currencies";

        const string CURRENCY_LIST_RESULT = "rows";
        const string CURRENCY_LIST_RESULT_NAME = "name";
        const string CURRENCY_LIST_RESULT_CURRENCY = "code";

        const int BUFFER_SIZE = 256000;

        readonly HttpClient client;

        public CryptonatorCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_CRYPTONATOR_REPOSITORY, name)
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = BUFFER_SIZE;
        }

        protected async override Task<IEnumerable<Model.Currency>> GetCurrencies()
        {
            var uri = new Uri(URL_CURRENCY_LIST);

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                var result = (JArray)json[CURRENCY_LIST_RESULT];

                var currentElements = new List<Model.Currency>();

                foreach (var token in result)
                {
                    var name = (string)token[CURRENCY_LIST_RESULT_NAME];
                    var code = (string)token[CURRENCY_LIST_RESULT_CURRENCY];
                    var c = new Model.Currency(code, name);

                    currentElements.Add(c);
                }

                return currentElements;

            }

            return null;
        }
    }
}