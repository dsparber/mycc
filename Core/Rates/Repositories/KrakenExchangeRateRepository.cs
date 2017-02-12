using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Rates.Repositories.Interfaces;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;
using SQLite;

namespace MyCC.Core.Rates.Repositories
{
    public class KrakenExchangeRateRepository : IMultipleRatesRepository
    {
        private const string Url = "https://api.kraken.com/0/public/Ticker?pair=XXBTZEUR,XXBTZUSD";
        private const string KeyResult = "result";
        private const string KeyEur = "XXBTZEUR";
        private const string KeyUsd = "XXBTZUSD";
        private const string KeyLastPrice = "a";

        private const int BufferSize = 256000;

        private readonly HttpClient _client;
        private readonly SQLiteAsyncConnection _connection;

        public KrakenExchangeRateRepository(SQLiteAsyncConnection connection)
        {
            _client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = BufferSize };
            _connection = connection;
            Rates = new List<ExchangeRate>();
        }

        public async Task<IEnumerable<ExchangeRate>> FetchRates()
        {
            var uri = new Uri(Url);
            var response = await _client.GetAsync(uri);

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var result = JObject.Parse(content)[KeyResult];
            var rateUsd = decimal.Parse((string)result[KeyUsd][KeyLastPrice][0], CultureInfo.InvariantCulture);
            var rateEur = decimal.Parse((string)result[KeyEur][KeyLastPrice][0], CultureInfo.InvariantCulture);

            var itemsCount = Rates.Count;
            Rates.Clear();
            Rates.Add(new ExchangeRate("BTC", true, "EUR", false, DateTime.Now, rateEur) { RepositoryId = TypeId });
            Rates.Add(new ExchangeRate("BTC", true, "USD", false, DateTime.Now, rateUsd) { RepositoryId = TypeId });

            if (itemsCount == 0) await _connection.InsertAllAsync(Rates);
            else await _connection.UpdateAllAsync(Rates.ToArray());

            return Rates;
        }

        public int TypeId => (int)RatesRepositories.Kraken;

        public Task FetchAvailableRates() => new Task(() => { });

        public bool IsAvailable(ExchangeRate rate)
        {
            return rate.ReferenceCurrencyCode.Equals("BTC") &&
                (rate.SecondaryCurrencyCode.Equals("EUR") || rate.SecondaryCurrencyCode.Equals("USD"));
        }

        public List<ExchangeRate> Rates { get; }

        public Task UpdateRates() => FetchRates();

        public RateRepositoryType RatesType => RateRepositoryType.CryptoToFiat;

        public string Name => I18N.Kraken;
    }
}

