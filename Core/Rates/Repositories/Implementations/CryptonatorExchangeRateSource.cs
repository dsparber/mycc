using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Core.Rates.Models;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;
using SQLite;

namespace MyCC.Core.Rates.Repositories.Implementations
{
    public class CryptonatorExchangeRateSource : IRateSource
    {
        private const string UrlRate = "https://api.cryptonator.com/api/ticker/{0}";

        private const string ResultKey = "ticker";
        private const string RateKey = "price";

        private const int BufferSize = 256000;

        private readonly HttpClient _client;

        private IEnumerable<Currency> _supportedCurrencies => CurrencyConstants.FlagCryptonator.Currencies();
        private readonly SQLiteAsyncConnection _connection;


        public CryptonatorExchangeRateSource(SQLiteAsyncConnection connection)
        {
            _client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = BufferSize };
            Rates = new List<ExchangeRate>();
            _connection = connection;
        }

        public async Task<ExchangeRate> FetchRate(ExchangeRate exchangeRate)
        {
            var uri = new Uri(string.Format(UrlRate, ToUrl(exchangeRate)));
            try
            {
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
                    Rates.RemoveAll(r => r.Equals(exchangeRate));
                    await _connection.UpdateAsync(exchangeRate);
                }
                else
                {
                    await _connection.InsertOrReplaceAsync(exchangeRate);
                }
                Rates.Add(exchangeRate);

                return exchangeRate;
            }
            catch (Exception e)
            {
                e.LogError();
                return null;
            }
        }

        private static string ToUrl(ExchangeRate exchangeRate)
        {
            return exchangeRate.ReferenceCurrencyCode?.ToLower() + "-" + exchangeRate.SecondaryCurrencyCode?.ToLower();
        }

        public int TypeId => (int)RateSourceId.Cryptonator;

        public bool IsAvailable(ExchangeRate rate)
        {
            return _supportedCurrencies.Any(c => c.Id.Equals(rate.ReferenceCurrency.Id)) &&
                   _supportedCurrencies.Any(c => c.Id.Equals(rate.SecondaryCurrency.Id));
        }

        public List<ExchangeRate> Rates { get; }

        public RateSourceType Type => RateSourceType.Crypto;

        public string Name => ConstantNames.Cryptonator;
    }
}

