using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyCryptos.Core.ExchangeRate.Database;
using Newtonsoft.Json.Linq;

namespace MyCryptos.Core.ExchangeRate.Repositories
{
    public class BittrexExchangeRateRepository : OnlineExchangeRateRepository
    {
        const string URL_RATE = "https://bittrex.com/api/v1.1/public/getticker?market={0}";

        const string RESULT_KEY = "result";
        const string RATE_KEY = "Last";

        const int BUFFER_SIZE = 256000;

        readonly HttpClient client;

        public BittrexExchangeRateRepository(string name) : base(ExchangeRateRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY, name)
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = BUFFER_SIZE;
        }

        protected override async Task GetFetchTask(Model.ExchangeRate exchangeRate)
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
                    var rate = 1 / decimal.Parse((string)rateJson[RATE_KEY], CultureInfo.InvariantCulture);
                    exchangeRate.Rate = rate;
                }
            }
        }

        static string ToUrl(Model.ExchangeRate exchangeRate)
        {
            return exchangeRate.ReferenceCurrency.Code.ToUpper() + "-" + exchangeRate.SecondaryCurrency.Code.ToUpper();
        }
    }
}

