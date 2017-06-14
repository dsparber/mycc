using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class CryptoIdCoinInfoRepository : ICoinInfoRepository
    {
        private Uri GetUri(string currencyId, string action)
            => new Uri($"https://chainz.cryptoid.info/{currencyId.Code().ToLower()}/api.dws?q={action}");

        public string WebUrl(string currencyId) => $"https://chainz.cryptoid.info/{currencyId.Code().ToLower()}/#!crypto";

        private const string KeySummary = "summary";
        private const string KeyHashrate = "hashrate";

        private const string JsonKeyAlgorithm = "PoW";
        private const string JsonKeyIsPoS = "PoS";
        private const string JsonKeyHeigth = "height";
        private const string JsonKeyDifficulty = "diff";
        private const string JsonKeySupply = "supply";

        public string Name => ConstantNames.CryptoId;

        public List<string> SupportedCoins
            => CurrencyConstants.FlagCryptoId.CurrencyIds().ToList();

        public async Task<CoinInfoData> GetInfo(string currencyId)
        {
            var client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = 256000 };

            try
            {

                var summary = await client.GetAsync(GetUri(currencyId, KeySummary));
                var hrate = await client.GetAsync(GetUri(currencyId, KeyHashrate));



                var summaryJson = JObject.Parse(await summary.Content.ReadAsStringAsync())[currencyId.Code().ToLower()];
                var hashrate =
                    decimal.Parse(await hrate.Content.ReadAsStringAsync(), CultureInfo.InvariantCulture) as decimal?;
                hashrate = hashrate == 0 ? null : hashrate;

                string ParseOrNull(JToken o)
                {
                    var s = (string)o;
                    return string.IsNullOrEmpty(s?.Trim()) ? null : s;
                }

                return new CoinInfoData(currencyId)
                {
                    Algorithm = ParseOrNull(summaryJson[JsonKeyAlgorithm]),
                    IsProofOfWork = ParseOrNull(summaryJson[JsonKeyAlgorithm]) != null,
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
                return new CoinInfoData(currencyId);
            }
        }
    }
}
