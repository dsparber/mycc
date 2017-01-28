using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using MyCC.Core.Rates.Repositories.Interfaces;
using Newtonsoft.Json.Linq;
using SQLite;

namespace MyCC.Core.Rates.Repositories
{
    public class BtceExchangeRateRepository : IMultipleRatesRepository
    {
        private const string Url = "https://btc-e.com/api/3/ticker/btc_usd-btc_eur";
        private const string Key = "last";

        private const int BufferSize = 256000;

        private readonly HttpClient _client;
        private readonly SQLiteAsyncConnection _connection;

        public BtceExchangeRateRepository(SQLiteAsyncConnection connection)
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
            var json = JObject.Parse(content);
            var rateUsd = decimal.Parse((string)json["btc_usd"][Key], CultureInfo.InvariantCulture);
            var rateEur = decimal.Parse((string)json["btc_eur"][Key], CultureInfo.InvariantCulture);

            var itemsCount = Rates.Count;
            Rates.Clear();
            Rates.Add(new ExchangeRate("BTC", true, "EUR", false, rateEur) { RepositoryId = TypeId });
            Rates.Add(new ExchangeRate("BTC", true, "USD", false, rateUsd) { RepositoryId = TypeId });

            if (itemsCount == 0) await _connection.InsertAllAsync(Rates);
            else await _connection.UpdateAllAsync(Rates);

            return Rates;
        }

        public int TypeId => (int)RatesRepositories.Btce;

        public Task FetchAvailableRates() => new Task(() => { });

        public bool IsAvailable(ExchangeRate rate)
        {
            return rate.ReferenceCurrencyCode.Equals("BTC") &&
                (rate.SecondaryCurrencyCode.Equals("EUR") || rate.SecondaryCurrencyCode.Equals("USD"));
        }

        public List<ExchangeRate> Rates { get; }

        public Task UpdateRates() => FetchRates();

        public RateRepositoryType RatesType => RateRepositoryType.CryptoToFiat;
    }
}
