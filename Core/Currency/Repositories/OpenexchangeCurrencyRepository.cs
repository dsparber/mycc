using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyCC.Core.Currency.Database;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Currency.Repositories
{
	public class OpenexchangeCurrencyRepository : OnlineCurrencyRepository
	{
		private const string UrlCurrencyList = "https://openexchangerates.org/api/currencies.json";

		private const int BufferSize = 256000;

		private readonly HttpClient _client;

		public OpenexchangeCurrencyRepository(int id) : base(id)
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

			var currentElements = new List<Model.Currency>();

			foreach (var key in json)
			{
				var name = (string)key.Value;
				var code = key.Key.ToUpperInvariant();
				if (!code.Equals("BTC"))
				{
					currentElements.Add(new Model.Currency(code, name, false));
				}
			}

			await Task.WhenAll(Elements.Where(e => !currentElements.Contains(e)).Select(Remove));

			LastFetch = DateTime.Now;
			return currentElements;
		}
	}
}