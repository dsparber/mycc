using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Currency.Repositories;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Rates.Repositories.Interfaces;
using Newtonsoft.Json.Linq;
using SQLite;

namespace MyCC.Core.Rates.Repositories
{
    public class CryptonatorExchangeRateRepository : ISingleRateRepository
    {
        private const string UrlRate = "https://api.cryptonator.com/api/ticker/{0}";

        private const string ResultKey = "ticker";
        private const string RateKey = "price";

        private const int BufferSize = 256000;

        private readonly HttpClient _client;

        private List<string> _supportedCurrencies;
        private readonly SQLiteAsyncConnection _connection;


        public CryptonatorExchangeRateRepository(SQLiteAsyncConnection connection)
        {
            _client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = BufferSize };
            Rates = new List<ExchangeRate>();
            _supportedCurrencies = new List<string>();
            _connection = connection;
        }

        public async Task<ExchangeRate> FetchRate(ExchangeRate exchangeRate)
        {
            var uri = new Uri(string.Format(UrlRate, ToUrl(exchangeRate)));
            var response = await _client.GetAsync(uri);

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var rateJson = json[ResultKey];
            if (rateJson == null || rateJson.ToList().Count <= 0) return null;

            var rate = decimal.Parse((string)rateJson[RateKey], CultureInfo.InvariantCulture);
            exchangeRate.Rate = rate;
            exchangeRate.LastUpdate = DateTime.Now;
            exchangeRate.RepositoryId = TypeId;

            if (Rates.Contains(exchangeRate))
            {
                Rates.Remove(exchangeRate);
                await _connection.UpdateAsync(exchangeRate);
            }
            else
            {
                await _connection.InsertOrReplaceAsync(exchangeRate);
            }
            Rates.Add(exchangeRate);

            return exchangeRate;
        }

        private static string ToUrl(ExchangeRate exchangeRate)
        {
            return exchangeRate.ReferenceCurrencyCode?.ToLower() + "-" + exchangeRate.SecondaryCurrencyCode?.ToLower();
        }

        public int TypeId => (int)RatesRepositories.Cryptonator;

        public Task FetchAvailableRates()
        {
            var id = CurrencyStorage.Instance.RepositoryOfType<CryptonatorCurrencyRepository>().Id;
            var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.ParentId == id).Select(e => e.Code);
            _supportedCurrencies = CurrencyStorage.Instance.AllElements.Where(c => codes.Any(x => x.Equals(c?.Code))).Select(c => c.Code).ToList();

            return new Task(() => { });
        }

        public bool IsAvailable(ExchangeRate rate)
        {
            return _supportedCurrencies.Contains(rate.ReferenceCurrencyCode) &&
                   _supportedCurrencies.Contains(rate.SecondaryCurrencyCode);
        }

        public List<ExchangeRate> Rates { get; }

        public RateRepositoryType RatesType => RateRepositoryType.CryptoRates;
        public Task UpdateRates() => Task.WhenAll(Rates.Where(r => r != null).Select(FetchRate));
    }
}

