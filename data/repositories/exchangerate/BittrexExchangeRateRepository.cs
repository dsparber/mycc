using System.Threading.Tasks;
using System.Linq;
using MyCryptos.models;
using System;
using System.Net.Http;
using data.database.models;
using Newtonsoft.Json.Linq;

namespace data.repositories.exchangerate
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

		protected async override Task GetFetchTask(ExchangeRate exchangeRate)
		{
			var uri = new Uri(string.Format(URL_RATE, ToUrl(exchangeRate)));
			var response = await client.GetAsync(uri);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var json = JObject.Parse(content);
				JToken rateJson = json[RESULT_KEY];
				if (rateJson.ToList().Count != 0)
				{
					var rate = 1 / (decimal)rateJson[RATE_KEY];
					exchangeRate.Rate = rate;
				}
			}
		}

		static string ToUrl(ExchangeRate exchangeRate)
		{
			return exchangeRate.ReferenceCurrency.Code.ToUpper() + "-" + exchangeRate.SecondaryCurrency.Code.ToUpper();
		}
	}
}

