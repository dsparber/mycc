using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyCC.Core.Currency.Database;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Currency.Repositories
{
	public class BittrexCurrencyRepository : OnlineCurrencyRepository
	{
		private const string UrlCurrencyList = "https://bittrex.com/api/v1.1/public/getcurrencies";

		private const string CurrencyListResult = "result";
		private const string CurrencyListResultName = "CurrencyLong";
		private const string CurrencyListResultCurrency = "Currency";

		private const int BufferSize = 256000;

		private readonly HttpClient _client;

		public BittrexCurrencyRepository(int id) : base(id)
		{
			_client = new HttpClient { MaxResponseContentBufferSize = BufferSize };
		}

		public override int RepositoryTypeId => CurrencyRepositoryDbm.DbTypeBittrexRepository;

		protected override async Task<IEnumerable<Model.Currency>> GetCurrencies()
		{
			var uri = new Uri(UrlCurrencyList);

			var response = await _client.GetAsync(uri);

			if (!response.IsSuccessStatusCode) return null;

			var content = await response.Content.ReadAsStringAsync();
			var json = JObject.Parse(content);
			var result = (JArray)json[CurrencyListResult];

			var currentElements = (from token in result let name = (string)token[CurrencyListResultName] let code = (string)token[CurrencyListResultCurrency] select new Model.Currency(code, name, true)).ToList();

			await Task.WhenAll(Elements.Where(e => !currentElements.Contains(e)).Select(Remove));

			LastFetch = DateTime.Now;
			return currentElements;
		}
	}
}