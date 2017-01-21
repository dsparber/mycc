using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using MyCC.Core.Currency.Repositories;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class CryptoIdCoinInfoRepository : ICoinInfoRepository
    {
        private Uri GetUri(Currency.Model.Currency coin, string action) => new Uri($"http://chainz.cryptoid.info/{coin.Code.ToLower()}/api.dws?q={action}");
        private const string KeySummary = "summary";
        private const string KeyHashrate = "hashrate";

        private const string JsonKeyAlgorithm = "PoW";
        private const string JsonKeyIsPoS = "PoS";
        private const string JsonKeyHeigth = "height";
        private const string JsonKeyDifficulty = "diff";
        private const string JsonKeySupply = "supply";

        public string Name => I18N.CryptoId;

        public List<Currency.Model.Currency> SupportedCoins => CurrencyStorage.Instance.RepositoryOfType<CryptoIdCurrencyRepository>().Currencies;

        public async Task<CoinInfoData> GetInfo(Currency.Model.Currency currency)
        {
            var client = new HttpClient { MaxResponseContentBufferSize = 256000 };

            var summary = await client.GetAsync(GetUri(currency, KeySummary));
            var hrate = await client.GetAsync(GetUri(currency, KeyHashrate));



            var summaryJson = JObject.Parse(await summary.Content.ReadAsStringAsync())[currency.Code.ToLower()];
            var hashrate = decimal.Parse(await hrate.Content.ReadAsStringAsync(), CultureInfo.InvariantCulture) as decimal?;
            hashrate = hashrate == 0 ? null : hashrate;

            Func<JToken, string> ParseOrNull = o =>
            {
                var s = (string)o;
                return string.IsNullOrEmpty(s?.Trim()) ? null : s;
            };

            return new CoinInfoData(currency)
            {
                Algorithm = ParseOrNull(summaryJson[JsonKeyAlgorithm]),
                IsProofOfWork = ParseOrNull(summaryJson[JsonKeyAlgorithm]) != null,
                IsProofOfStake = bool.Parse((string)summaryJson[JsonKeyIsPoS]),
                BlockHeight = int.Parse((string)summaryJson[JsonKeyHeigth]),
                Difficulty = decimal.Parse((string)summaryJson[JsonKeyDifficulty], CultureInfo.InvariantCulture),
                CoinSupply = decimal.Parse((string)summaryJson[JsonKeySupply], CultureInfo.InvariantCulture),
                Hashrate = hashrate
            };
        }
    }
}
