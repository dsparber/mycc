using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using MyCC.Core.Rates.Repositories.Interfaces;
using Newtonsoft.Json.Linq;
using SQLite;
using System.Linq;

namespace MyCC.Core.Rates.Repositories
{
    public class FixerIoExchangeRateRepository : IMultipleRatesRepository
    {
        private const string Url = "https://api.fixer.io/latest";

        private const string JsonKeyRates = "rates";

        private const int BufferSize = 256000;

        private readonly HttpClient _client;
        private readonly SQLiteAsyncConnection _connection;


        public FixerIoExchangeRateRepository(SQLiteAsyncConnection connection)
        {
            _client = new HttpClient { MaxResponseContentBufferSize = BufferSize };
            _connection = connection;
            Rates = new List<ExchangeRate>();
        }

        public bool IsAvailable(ExchangeRate rate) =>
            Rates.Contains(rate);

        public RateRepositoryType RatesType => RateRepositoryType.FiatRates;

        public int TypeId => (int)RatesRepositories.FixerIo;


        public async Task<IEnumerable<ExchangeRate>> FetchRates()
        {
            var uri = new Uri(Url);
            var response = await _client.GetAsync(uri);

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var ratesJson = (JObject)json[JsonKeyRates];

            var fetchedRates = new List<ExchangeRate>();

            foreach (var r in ratesJson)
            {
                var rate = new ExchangeRate(Currency.Model.Currency.Eur.Code, r.Key, decimal.Parse((string)r.Value, CultureInfo.InvariantCulture)) { RepositoryId = TypeId };
                fetchedRates.Add(rate);
            }

            var newRates = fetchedRates.Except(Rates).ToList();
            var existing = fetchedRates.Intersect(Rates).ToList();
            var old = Rates.Except(fetchedRates).ToList();

            Rates.Clear();
            Rates.AddRange(newRates.Concat(existing));

            await _connection.InsertAllAsync(newRates);
            await _connection.UpdateAllAsync(existing);
            await Task.WhenAll(old.Select(_connection.DeleteAsync));

            return Rates;
        }

        public Task FetchAvailableRates() => FetchRates();

        public Task UpdateRates() => FetchRates();


        public List<ExchangeRate> Rates { get; }
    }
}

