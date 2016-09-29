using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using data.database.models;
using models;
using Newtonsoft.Json.Linq;

namespace data.repositories.currency
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

		public override async Task<bool> Fetch()
		{
			var uri = new Uri(URL_CURRENCY_LIST);

			var response = await client.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				try
				{
					var content = await response.Content.ReadAsStringAsync();
					var json = JObject.Parse(content);
					var result = (JArray)json[CURRENCY_LIST_RESULT];

					foreach (JToken token in result)
					{
						var name = (string)token[CURRENCY_LIST_RESULT_NAME];
						var code = (string)token[CURRENCY_LIST_RESULT_CURRENCY];
						var c = new Currency(code, name);

						Elements.Remove(c);
						Elements.Add(c);
					}
					await WriteToDatabase();
					LastFetch = DateTime.Now;
					return true;
				}
				catch (Exception e)
				{
					Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
					return false;
				}
			}
			return false;
		}
	}
}