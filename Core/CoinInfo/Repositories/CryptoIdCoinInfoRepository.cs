using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Currency.Repositories;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class CryptoIdCoinInfoRepository : ICoinInfoRepository
    {
        private Uri GetUri(Currency.Model.Currency coin, string action)
            => new Uri($"https://chainz.cryptoid.info/{coin.Code.ToLower()}/api.dws?q={action}");

        private const string KeySummary = "summary";
        private const string KeyHashrate = "hashrate";

        private const string JsonKeyAlgorithm = "PoW";
        private const string JsonKeyIsPoS = "PoS";
        private const string JsonKeyHeigth = "height";
        private const string JsonKeyDifficulty = "diff";
        private const string JsonKeySupply = "supply";

        public string Name => I18N.CryptoId;

        public List<Currency.Model.Currency> SupportedCoins
            => CurrencyStorage.Instance.RepositoryOfType<CryptoIdCurrencyRepository>().Currencies;

        public async Task<CoinInfoData> GetInfo(Currency.Model.Currency currency)
        {
            var client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = 256000 };

            try
            {

                var summary = await client.GetAsync(GetUri(currency, KeySummary));
                var hrate = await client.GetAsync(GetUri(currency, KeyHashrate));



                var summaryJson = JObject.Parse(await summary.Content.ReadAsStringAsync())[currency.Code.ToLower()];
                var hashrate =
                    decimal.Parse(await hrate.Content.ReadAsStringAsync(), CultureInfo.InvariantCulture) as decimal?;
                hashrate = hashrate == 0 ? null : hashrate;

                Func<JToken, string> parseOrNull = o =>
                {
                    var s = (string)o;
                    return string.IsNullOrEmpty(s?.Trim()) ? null : s;
                };

                return new CoinInfoData(currency)
                {
                    Algorithm = parseOrNull(summaryJson[JsonKeyAlgorithm]),
                    IsProofOfWork = parseOrNull(summaryJson[JsonKeyAlgorithm]) != null,
                    IsProofOfStake = bool.Parse((string)summaryJson[JsonKeyIsPoS]),
                    BlockHeight = int.Parse((string)summaryJson[JsonKeyHeigth]),
                    Difficulty = decimal.Parse((string)summaryJson[JsonKeyDifficulty], CultureInfo.InvariantCulture),
                    CoinSupply = decimal.Parse((string)summaryJson[JsonKeySupply], CultureInfo.InvariantCulture),
                    Hashrate = hashrate,
                    LastUpdate = DateTime.Now
                };

            }
            catch (Exception e)
            {
                e.LogError();
                return new CoinInfoData(currency);
            }
        }
    }
}
