using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using MyCC.Core.ExchangeRate.Database;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.ExchangeRate.Repositories
{
    public class BtceExchangeRateRepository : OnlineExchangeRateRepository
    {
        private const string URL = "https://btc-e.com/api/3/ticker/{0}";
        private const string KEY = "last";

        private const int BUFFER_SIZE = 256000;

        private readonly HttpClient client;

        public BtceExchangeRateRepository(int id) : base(id)
        {
            client = new HttpClient { MaxResponseContentBufferSize = BUFFER_SIZE };
        }

        public override int RepositoryTypeId => ExchangeRateRepositoryDbm.DB_TYPE_BTCE_REPOSITORY;

        protected override async Task GetFetchTask(Model.ExchangeRate exchangeRate)
        {
            var uri = new Uri(string.Format(URL, RateToUrl(exchangeRate)));
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                var rateJson = json[RateToUrl(exchangeRate)];
                var rate = decimal.Parse((string)rateJson[KEY], CultureInfo.InvariantCulture);
                exchangeRate.Rate = rate;
            }
        }

        private string RateToUrl(Model.ExchangeRate exchangeRate)
        {
            return exchangeRate.ReferenceCurrency.Code.ToLower() + "_" + exchangeRate.SecondaryCurrency.Code.ToLower();
        }
    }
}

