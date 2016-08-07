using System;
using System.Net.Http;
using System.Threading.Tasks;
using data.database.models;
using models;
using Newtonsoft.Json.Linq;

namespace data.repositories.currency
{
	public class BittrexCurrencyRepository : OnlineCurrencyRepository
	{
		const string URL_CURRENCY_LIST = "https://bittrex.com/api/v1.1/public/getcurrencies";

		const string CURRENCY_LIST_RESULT = "result";
		const string CURRENCY_LIST_RESULT_NAME = "CurrencyLong";
		const string CURRENCY_LIST_RESULT_CURRENCY = "Currency";

		const int BUFFER_SIZE = 256000;

		readonly HttpClient client;

		public BittrexCurrencyRepository() : base(CurrencyRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY)
		{
			client = new HttpClient();
			client.MaxResponseContentBufferSize = BUFFER_SIZE;
		}

		public override async Task Fetch()
		{
			var uri = new Uri(URL_CURRENCY_LIST);

			var response = await client.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var json = JObject.Parse(content);
				var result = (JArray)json[CURRENCY_LIST_RESULT];

				foreach (JToken token in result)
				{
					var name = (string)token[CURRENCY_LIST_RESULT_NAME];
					var code = (string)token[CURRENCY_LIST_RESULT_CURRENCY];
					var c = new Currency(name, code);

					Elements.Remove(c);
					Elements.Add(c);
				}
			}
			await WriteToDatabase();
		}
	}
}

