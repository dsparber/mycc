using System.Threading.Tasks;
using System.Linq;
using models;
using System;
using System.Net.Http;
using data.database.models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace data.repositories.exchangerate
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

		// TODO Add Error Handling for all Online FetchTasks => Avoid App chrash
		protected override async Task GetFetchTask(ExchangeRate exchangeRate)
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
					var rate = (decimal)rateJson[RATE_KEY];
					exchangeRate.Rate = rate;
				}
			}
			Elements.Remove(exchangeRate);
			Elements.Add(exchangeRate);
		}

		static string ToUrl(ExchangeRate exchangeRate)
		{
			return exchangeRate.ReferenceCurrency.Code.ToLower() + "-" + exchangeRate.SecondaryCurrency.Code.ToLower();
		}
	}
}

