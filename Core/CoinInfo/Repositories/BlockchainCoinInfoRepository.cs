using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Resources;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class BlockchainCoinInfoRepository : ICoinInfoRepository
    {
        private Uri GetUri(string action) => new Uri($"https://blockchain.info/de/q/{action}");

        private const string KeyHeight = "getblockcount";
        private const string KeyDifficulty = "getdifficulty";
        private const string KeyBlockReward = "bcperblock";
        private const string KeySupply = "totalbc";
        private const string KeyHashrate = "hashrate";

        public List<Currency.Model.Currency> SupportedCoins => new List<Currency.Model.Currency> { Currency.Model.Currency.Btc };

        public string Name => I18N.Blockchain;

        public async Task<CoinInfoData> GetInfo(Currency.Model.Currency currency)
        {
            var client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = 256000 };

            var heightTask = client.GetAsync(GetUri(KeyHeight));
            var hashrateTask = client.GetAsync(GetUri(KeyHashrate));
            var diffTask = client.GetAsync(GetUri(KeyDifficulty));
            var supplyTask = client.GetAsync(GetUri(KeySupply));
            var blockRewardTask = client.GetAsync(GetUri(KeyBlockReward));

            Func<Task<HttpResponseMessage>, Task<string>> getString = async (m) =>
            {
                var s = (await (await m).Content.ReadAsStringAsync());
                return string.IsNullOrWhiteSpace(s) ? null : s;
            };

            var stringHeight = await getString(heightTask);
            var stringHashrate = await getString(hashrateTask);
            var stringDiff = await getString(diffTask);
            var stringSupply = await getString(supplyTask);
            var stringBlockReward = await getString(blockRewardTask);

            var heigh = stringHeight != null ? int.Parse(stringHeight) as int? : null;
            var hashrate = stringHashrate != null ? decimal.Parse(stringHashrate, CultureInfo.InvariantCulture) as decimal? : null;
            var diff = stringDiff != null ? decimal.Parse(stringDiff, CultureInfo.InvariantCulture) as decimal? : null;
            var supply = stringSupply != null ? decimal.Parse(stringSupply, CultureInfo.InvariantCulture) as decimal? : null;
            var blockReward = stringBlockReward != null ? decimal.Parse(stringBlockReward, CultureInfo.InvariantCulture) as decimal? : null;

            return new CoinInfoData(currency)
            {
                BlockHeight = heigh != 0 ? heigh : null,
                CoinSupply = supply != 0 ? supply : null,
                Hashrate = hashrate != 0 ? hashrate : null,
                Difficulty = diff != 0 ? diff : null,
                Blockreward = blockReward != 0 ? blockReward / 100000000 : null,
                Algorithm = "SHA-256",
                IsProofOfStake = false,
                IsProofOfWork = true,
                LastUpdate = DateTime.Now
            };
        }
    }
}
