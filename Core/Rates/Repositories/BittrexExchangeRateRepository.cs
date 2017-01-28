using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyCC.Core.Rates.Repositories.Interfaces;
using Newtonsoft.Json.Linq;
using SQLite;

namespace MyCC.Core.Rates.Repositories
{
	public class BittrexExchangeRateRepository : IMultipleRatesRepository
	{
		private const string Url = "https://bittrex.com/api/v1.1/public/getmarketsummaries";

		private const string ResultKey = "result";
		private const string RateKey = "Last";
		private const string MarketKey = "MarketName";

		private const int BufferSize = 256000;

		private readonly HttpClient _client;
		private readonly SQLiteAsyncConnection _connection;


		public BittrexExchangeRateRepository(SQLiteAsyncConnection connection)
		{
			_client = new HttpClient { MaxResponseContentBufferSize = BufferSize };
			_connection = connection;
			Rates = new List<ExchangeRate>();

		}
		public async Task<IEnumerable<ExchangeRate>> FetchRates()
		{
			var uri = new Uri(Url);
			var response = await _client.GetAsync(uri);

			if (!response.IsSuccessStatusCode) return null;

			var content = await response.Content.ReadAsStringAsync();
			var resultJson = JObject.Parse(content)[ResultKey];

			var fetchedRates = (from r in resultJson let market = ((string)r[MarketKey]).Split('-') let rate = 1 / decimal.Parse((string)r[RateKey], NumberStyles.Float) select new ExchangeRate(market[0], true, market[1], true, rate) { RepositoryId = TypeId }).ToList();

			var old = Rates.Except(fetchedRates).ToList();

			Rates.Clear();
			Rates.AddRange(fetchedRates);

			await Task.WhenAll(old.Select(_connection.DeleteAsync));
			await Task.WhenAll(fetchedRates.Select(_connection.InsertOrReplaceAsync));

			return Rates;
		}

		public int TypeId => (int)RatesRepositories.Bittrex;

		public Task FetchAvailableRates() => FetchRates();

		public bool IsAvailable(ExchangeRate rate) => Rates.Contains(rate);

		public Task UpdateRates() => FetchRates();

		public List<ExchangeRate> Rates { get; }

		public RateRepositoryType RatesType => RateRepositoryType.CryptoRates;
	}
}
