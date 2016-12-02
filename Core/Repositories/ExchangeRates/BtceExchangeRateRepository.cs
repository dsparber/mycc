using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using Newtonsoft.Json.Linq;

namespace MyCryptos.Core.Repositories.ExchangeRates
{
    public class BtceExchangeRateRepository : OnlineExchangeRateRepository
    {
        const string URL = "https://btc-e.com/api/3/ticker/{0}";
        const string KEY = "last";

        const int BUFFER_SIZE = 256000;

        readonly HttpClient client;

        public BtceExchangeRateRepository(string name) : base(ExchangeRateRepositoryDBM.DB_TYPE_BTCE_REPOSITORY, name)
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = BUFFER_SIZE;
        }

        protected async override Task GetFetchTask(ExchangeRate exchangeRate)
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

        string RateToUrl(ExchangeRate exchangeRate)
        {
            return exchangeRate.ReferenceCurrency.Code.ToLower() + "_" + exchangeRate.SecondaryCurrency.Code.ToLower();
        }
    }
}
