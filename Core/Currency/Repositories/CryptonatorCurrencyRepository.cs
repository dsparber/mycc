using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Currency.Database;
using MyCC.Core.Helpers;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Currency.Repositories
{
    public class CryptonatorCurrencyRepository : OnlineCurrencyRepository
    {
        private const string UrlCurrencyList = "https://api.cryptonator.com/api/currencies";

        public override string Description => "Cryptonator";


        private const string CurrencyListResult = "rows";
        private const string CurrencyListResultName = "name";
        private const string CurrencyListResultCurrency = "code";

        private const int BufferSize = 256000;

        private readonly HttpClient _client;

        private readonly IEnumerable<string> _currencyBlacklist = new[] { "CAD", "CNY", "EUR", "GBP", "JPY", "UAH", "USD" };


        public CryptonatorCurrencyRepository(int id) : base(id)
        {
            _client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = BufferSize };
        }

        public override int RepositoryTypeId => CurrencyRepositoryDbm.DbTypeCryptonatorRepository;

        protected override async Task<IEnumerable<Model.Currency>> GetCurrencies()
        {
            var uri = new Uri(UrlCurrencyList);

            try
            {
                var response = await _client.GetAsync(uri);
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                var result = (JArray)json[CurrencyListResult];


                var currencies = (from token in result
                                  let name = (string)token[CurrencyListResultName]
                                  let code = (string)token[CurrencyListResultCurrency]
                                  select new Model.Currency(code, name, true))
                                .Where(c => !_currencyBlacklist.Contains(c.Code)).ToList();

                return currencies;
            }
            catch (Exception e)
            {
                e.LogError();
                return new List<Model.Currency>();
            }
        }
    }
}