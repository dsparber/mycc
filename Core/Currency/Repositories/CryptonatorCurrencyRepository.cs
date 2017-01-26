using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyCC.Core.Currency.Database;
using MyCC.Core.Currency.Storage;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.Currency.Repositories
{
	public class CryptonatorCurrencyRepository : OnlineCurrencyRepository
	{
		private const string UrlCurrencyList = "https://api.cryptonator.com/api/currencies";

		private const string CurrencyListResult = "rows";
		private const string CurrencyListResultName = "name";
		private const string CurrencyListResultCurrency = "code";

		private const int BufferSize = 256000;

		private readonly HttpClient _client;

		public CryptonatorCurrencyRepository(int id) : base(id)
		{
			_client = new HttpClient { MaxResponseContentBufferSize = BufferSize };
		}

		public override int RepositoryTypeId => CurrencyRepositoryDbm.DbTypeCryptonatorRepository;

		protected override async Task<IEnumerable<Model.Currency>> GetCurrencies()
		{
			var uri = new Uri(UrlCurrencyList);

			var response = await _client.GetAsync(uri);
			if (!response.IsSuccessStatusCode) return null;

			var content = await response.Content.ReadAsStringAsync();
			var json = JObject.Parse(content);
			var result = (JArray)json[CurrencyListResult];

			var currencies = (from token in result let name = (string)token[CurrencyListResultName] let code = (string)token[CurrencyListResultCurrency] select new Model.Currency(code, name, true)).ToList();

			var id = CurrencyStorage.Instance.RepositoryOfType<OpenexchangeCurrencyRepository>().Id;
			var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.ParentId == id).Select(e => e.Code);
			var fiatCurrencies = CurrencyStorage.Instance.AllElements.Where(c => codes.Any(x => x.Equals(c?.Code))).ToList();

			currencies.RemoveAll(c => fiatCurrencies.Any(f => f.Code.Equals(c.Code)));

			return currencies;
		}
	}
}