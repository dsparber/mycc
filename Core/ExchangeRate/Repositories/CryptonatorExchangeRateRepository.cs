using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using Newtonsoft.Json.Linq;

namespace MyCryptos.Core.Repositories.ExchangeRates
{
    public class CryptonatorExchangeRateRepository : OnlineExchangeRateRepository
    {
        const string URL_RATE = "https://api.cryptonator.com/api/ticker/{0}";

        const string RESULT_KEY = "ticker";
        const string RATE_KEY = "price";

        const int BUFFER_SIZE = 256000;

        readonly HttpClient client;

        public CryptonatorExchangeRateRepository(string name) : base(ExchangeRateRepositoryDBM.DB_TYPE_CRYPTONATOR_REPOSITORY, name)
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = BUFFER_SIZE;
        }

        protected override async Task GetFetchTask(ExchangeRate exchangeRate)
        {
            var uri = new Uri(string.Format(URL_RATE, ToUrl(exchangeRate)));
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                var rateJson = json[RESULT_KEY];
                if (rateJson.ToList().Count != 0)
                {
                    var rate = decimal.Parse((string)rateJson[RATE_KEY], CultureInfo.InvariantCulture);
                    exchangeRate.Rate = rate;
                }
            }
        }

        static string ToUrl(ExchangeRate exchangeRate)
        {
            return exchangeRate.ReferenceCurrency.Code.ToLower() + "-" + exchangeRate.SecondaryCurrency.Code.ToLower();
        }
    }
}

