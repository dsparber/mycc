using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using data.database.models;
using MyCryptos.models;
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

		public BittrexCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY, name)
		{
			client = new HttpClient();
			client.MaxResponseContentBufferSize = BUFFER_SIZE;
		}

		protected async override Task<IEnumerable<Currency>> GetCurrencies()
		{
			var uri = new Uri(URL_CURRENCY_LIST);

			var response = await client.GetAsync(uri);

			if (response.IsSuccessStatusCode)
			{

				var content = await response.Content.ReadAsStringAsync();
				var json = JObject.Parse(content);
				var result = (JArray)json[CURRENCY_LIST_RESULT];

				var currentElements = new List<Currency>();

				foreach (JToken token in result)
				{
					var name = (string)token[CURRENCY_LIST_RESULT_NAME];
					var code = (string)token[CURRENCY_LIST_RESULT_CURRENCY];
					var c = new Currency(code, name);
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