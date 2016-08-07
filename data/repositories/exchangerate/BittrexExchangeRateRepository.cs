using System.Threading.Tasks;
using System.Linq;
using data.repositories.currency;
using models;
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

		public BittrexExchangeRateRepository() : base(ExchangeRateRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY)
		{
			client = new HttpClient();
			client.MaxResponseContentBufferSize = BUFFER_SIZE;
		}

		public override async Task Fetch()
		{
			var currencyRepository = new BittrexCurrencyRepository();
			await currencyRepository.Fetch();

			Elements = currencyRepository.Elements.Select(e => new ExchangeRate(Currency.BTC, e)).ToList();
			await WriteToDatabase();
		}

		public override async Task FetchExchangeRate(ExchangeRate exchangeRate)
		{
			var uri = new Uri(string.Format(URL_RATE, ToUrl(exchangeRate)));
			var response = await client.GetAsync(uri);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var json = JObject.Parse(content);
				JToken rateJson = json[RESULT_KEY];
				var rate = 1 / (decimal)rateJson[RATE_KEY];
				exchangeRate.Rate = rate;
			}

			await WriteToDatabase();
		}

		static string ToUrl(ExchangeRate exchangeRate)
		{
			return exchangeRate.ReferenceCurrency.Code.ToUpper() + "-" + exchangeRate.SecondaryCurrency.Code.ToUpper();
		}
	}
}

