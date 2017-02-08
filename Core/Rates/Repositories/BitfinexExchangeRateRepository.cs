using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Rates.Repositories.Interfaces;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;
using SQLite;

namespace MyCC.Core.Rates.Repositories
{
    public class BitfinexExchangeRateRepository : ISingleRateRepository
    {
        private const string UrlUsd = "https://api.bitfinex.com/v2/ticker/tBTCUSD";
        private const int PositionValue = 6;

        private const int BufferSize = 256000;

        private readonly HttpClient _client;
        private readonly SQLiteAsyncConnection _connection;

        public BitfinexExchangeRateRepository(SQLiteAsyncConnection connection)
        {
            _client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = BufferSize };
            _connection = connection;
            Rates = new List<ExchangeRate>();
        }

        public int TypeId => (int)RatesRepositories.Bitfinex;

        public Task FetchAvailableRates() => new Task(() => { });

        public bool IsAvailable(ExchangeRate rate)
        {
            return rate.ReferenceCurrencyCode.Equals("BTC") && rate.SecondaryCurrencyCode.Equals("USD");
        }

        public List<ExchangeRate> Rates { get; }

        public Task UpdateRates() => Task.WhenAll(Rates.Where(r => r != null).Select(FetchRate));

        public RateRepositoryType RatesType => RateRepositoryType.CryptoToFiat;

        public string Name => I18N.Bitfinex;

        public async Task<ExchangeRate> FetchRate(ExchangeRate rate)
        {
            if (!IsAvailable(rate)) return null;

            var uri = new Uri(UrlUsd);
            var response = await _client.GetAsync(uri);

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var rateString = content.Split(',')[PositionValue];
            var rateValue = decimal.Parse(rateString, CultureInfo.InvariantCulture);

            rate.Rate = rateValue;
            rate.LastUpdate = DateTime.Now;
            rate.RepositoryId = TypeId;

            if (Rates.Contains(rate))
            {
                Rates.RemoveAll(r => r.Equals(rate));
                await _connection.UpdateAsync(rate);
            }
            else
            {
                await _connection.InsertOrReplaceAsync(rate);
            }
            Rates.Add(rate);

            return rate;
        }
    }
}

