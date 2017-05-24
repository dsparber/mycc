using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using MyCC.Core.Rates.Repositories.Interfaces;
using Newtonsoft.Json.Linq;
using SQLite;
using System.Linq;
using ModernHttpClient;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;

namespace MyCC.Core.Rates.Repositories
{
    public class FixerIoExchangeRateRepository : IMultipleRatesRepository
    {
        private const string Url = "http://api.fixer.io/latest";

        private const string JsonKeyRates = "rates";

        private const int BufferSize = 256000;

        private readonly HttpClient _client;
        private readonly SQLiteAsyncConnection _connection;


        public FixerIoExchangeRateRepository(SQLiteAsyncConnection connection)
        {
            _client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = BufferSize };
            _connection = connection;
            Rates = new List<ExchangeRate>();
        }

        public bool IsAvailable(ExchangeRate rate) =>
            Rates.Contains(rate);

        public RateRepositoryType RatesType => RateRepositoryType.FiatRates;

        public int TypeId => (int)RatesRepositories.FixerIo;


        public async Task<IEnumerable<ExchangeRate>> FetchRates()
        {
            try
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
                    var rate = new ExchangeRate(CurrencyConstants.Eur.Id, new Currency(r.Key, false).Id,
                        DateTime.Now, decimal.Parse((string)r.Value, CultureInfo.InvariantCulture))
                    {
                        RepositoryId = TypeId
                    };
                    fetchedRates.Add(rate);
                }

                var old = Rates.Except(fetchedRates).ToList();

                Rates.Clear();
                Rates.AddRange(fetchedRates);

                await Task.WhenAll(old.Select(_connection.DeleteAsync));
                await Task.WhenAll(fetchedRates.Select(_connection.InsertOrReplaceAsync));

                return Rates;
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }

        public Task FetchAvailableRates() => FetchRates();

        public Task UpdateRates() => FetchRates();

        public string Name => ConstantNames.FixerIo;

        public List<ExchangeRate> Rates { get; }
    }
}

