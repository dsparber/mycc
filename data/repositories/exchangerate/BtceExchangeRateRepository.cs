using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using data.database.models;
using data.repositories.currency;
using data.storage;
using models;
using Newtonsoft.Json.Linq;

namespace data.repositories.exchangerate
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

		public override async Task Fetch()
		{
			var currencyRepository = new BtceCurrencyRepository(null);
			await currencyRepository.Fetch();

			var btc = currencyRepository.Elements.Find(c => c.Equals(Currency.BTC));
			Elements = currencyRepository.Elements.Select(e => new ExchangeRate(btc, e)).ToList();
			await WriteToDatabase();
			LastFetch = DateTime.Now;
		}

		public override async Task FetchExchangeRate(ExchangeRate exchangeRate)
		{
			var uri = new Uri(string.Format(URL, RateToUrl(exchangeRate)));
			var response = await client.GetAsync(uri);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var json = JObject.Parse(content);
				JToken rateJson = json[RateToUrl(exchangeRate)];
				var rate = (decimal)rateJson[KEY];
				exchangeRate.Rate = rate;
			}

			await WriteToDatabase();
			LastExchangeRateFetch = DateTime.Now;
		}

		string RateToUrl(ExchangeRate exchangeRate)
		{
			return exchangeRate.ReferenceCurrency.Code.ToLower() + "_" + exchangeRate.SecondaryCurrency.Code.ToLower();
		}
	}
}

