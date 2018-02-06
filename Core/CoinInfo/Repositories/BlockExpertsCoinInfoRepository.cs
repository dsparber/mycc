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

namespace MyCC.Core.CoinInfo.Repositories
{
    public class BlockExpertsCoinInfoRepository : ICoinInfoRepository
    {
        public string WebUrl(string currencyId) => $"https://www.blockexperts.com/{currencyId.Code().ToLower()}";

        private static Uri GetUri(string currencyId, string action) => new Uri($"https://www.blockexperts.com/api?coin={currencyId.Code().ToLower()}&action={action}");
        private const string KeyHeight = "getheight";
        private const string KeyDifficulty = "getdifficulty";
        private const string KeyHashrate = "getnetworkghps";
        private const string KeySupply = "getmoneysupply";

        public List<string> SupportedCoins => CurrencyConstants.FlagBlockExperts.CurrencyIds().ToList();

        public string Name => ConstantNames.BlockExperts;

        public async Task<CoinInfoData> GetInfo(string currencyId)
        {
            var client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = 256000 };

            var heightTask = client.GetAsync(GetUri(currencyId, KeyHeight));
            var hashrateTask = client.GetAsync(GetUri(currencyId, KeyHashrate));
            var diffTask = client.GetAsync(GetUri(currencyId, KeyDifficulty));
            var supplyTask = client.GetAsync(GetUri(currencyId, KeySupply));

            async Task<string> GetString(Task<HttpResponseMessage> task)
            {
                try
                {
                    var response = await task;
                    var s = await response.Content.ReadAsStringAsync();
                    return s;
                }
                catch (Exception e)
                {
                    e.LogError();
                    return null;
                }
            }

            int heigh; int.TryParse(await GetString(heightTask), out heigh);
            var hashrate = TryParse(await GetString(hashrateTask));
            var diff = TryParse(await GetString(diffTask));
            var supply = TryParse(await GetString(supplyTask));

            return new CoinInfoData(currencyId)
            {
                BlockHeight = heigh != 0 ? heigh as int? : null,
                CoinSupply = supply != 0 ? supply as decimal? : null,
                Hashrate = hashrate != 0 ? hashrate as decimal? : null,
                Difficulty = diff != 0 ? diff as decimal? : null,
                LastUpdate = DateTime.Now
            };
        }

        private static decimal TryParse(string s)
        {
            decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
            return result;
        }
    }
}
