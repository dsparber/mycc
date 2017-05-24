using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.Repositories.Interfaces;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;
using SQLite;

namespace MyCC.Core.Rates.Repositories
{
    public class BitPayExchangeRateRepository : IMultipleRatesRepository
    {
        private const string Url = "https://bitpay.com/rates/";
        private const string KeyData = "data";
        private const string KeyCoin = "code";
        private const string KeyValue = "rate";

        private const int BufferSize = 256000;

        private readonly HttpClient _client;
        private readonly SQLiteAsyncConnection _connection;

        public BitPayExchangeRateRepository(SQLiteAsyncConnection connection)
        {
            _client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = BufferSize };
            _connection = connection;
            Rates = new List<ExchangeRate>();
        }

        public async Task<IEnumerable<ExchangeRate>> FetchRates()
        {
            var uri = new Uri(Url);
            try
            {
                var response = await _client.GetAsync(uri);

                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(content)[KeyData] as JArray;

                var rateUsd = decimal.Parse((string)data.First(d => ((string)d[KeyCoin]).Equals("USD"))[KeyValue], CultureInfo.InvariantCulture);
                var rateEur = decimal.Parse((string)data.First(d => ((string)d[KeyCoin]).Equals("EUR"))[KeyValue], CultureInfo.InvariantCulture);

                var itemsCount = Rates.Count;
                Rates.Clear();
                Rates.Add(new ExchangeRate(CurrencyConstants.Btc.Id, CurrencyConstants.Eur.Id, DateTime.Now, rateEur) { RepositoryId = TypeId });
                Rates.Add(new ExchangeRate(CurrencyConstants.Btc.Id, CurrencyConstants.Usd.Id, DateTime.Now, rateUsd) { RepositoryId = TypeId });

                if (itemsCount == 0) await _connection.InsertAllAsync(Rates);
                else await _connection.UpdateAllAsync(Rates.ToArray());

                return Rates;
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }

        public int TypeId => (int)RatesRepositories.BitPay;

        public Task FetchAvailableRates() => new Task(() => { });

        public bool IsAvailable(ExchangeRate rate)
        {
            return rate.ReferenceCurrencyCode.Equals("BTC") &&
                (rate.SecondaryCurrencyCode.Equals("EUR") || rate.SecondaryCurrencyCode.Equals("USD"));
        }

        public List<ExchangeRate> Rates { get; }

        public Task UpdateRates() => FetchRates();

        public RateRepositoryType RatesType => RateRepositoryType.CryptoToFiat;

        public string Name => ConstantNames.BitPay;
    }
}

