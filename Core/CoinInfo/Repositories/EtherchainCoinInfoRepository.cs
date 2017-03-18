using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class EtherchainCoinInfoRepository : ICoinInfoRepository
    {
        private static readonly Uri UrlMining = new Uri("https://etherchain.org/api/miningEstimator");
        private static readonly Uri UrlBlockCount = new Uri("https://etherchain.org/api/blocks/count");
        private static readonly Uri UrlSupply = new Uri("https://api.etherscan.io/api?module=stats&action=ethsupply");

        private const string KeyData = "data";
        private const string KeyDifficulty = "difficulty";
        private const string KeyBlocktime = "blockTime";
        private const string KeyHashrate = "hashRate";
        private const string KeyResult = "result";
        private const string KeyCount = "count";

        public List<Currency.Model.Currency> SupportedCoins => new List<Currency.Model.Currency> { new Currency.Model.Currency("ETH", "Ethereum", true) };

        public string Name => I18N.Etherchain;

        public async Task<CoinInfoData> GetInfo(Currency.Model.Currency currency)
        {
            var client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = 256000 };

            var jsonMining = JObject.Parse(await (await client.GetAsync(UrlMining)).Content.ReadAsStringAsync());
            var jsonBlockCount = JObject.Parse(await (await client.GetAsync(UrlBlockCount)).Content.ReadAsStringAsync());
            var supply = (string)JObject.Parse(await (await client.GetAsync(UrlSupply)).Content.ReadAsStringAsync())[KeyResult];

            int n1, n5;
            decimal n2, n3, n4;

            return new CoinInfoData(currency)
            {
                BlockHeight = int.TryParse((string)jsonBlockCount[KeyData][0][KeyCount], out n1) ? n1 as int? : null,
                Hashrate = decimal.TryParse((string)jsonMining[KeyData][0][KeyHashrate], NumberStyles.Float, CultureInfo.InvariantCulture, out n2) ? n2 / 10e8m as decimal? : null,
                Difficulty = decimal.TryParse((string)jsonMining[KeyData][0][KeyDifficulty], NumberStyles.Float, CultureInfo.InvariantCulture, out n3) ? n3 as decimal? : null,
                Blocktime = decimal.TryParse((string)jsonMining[KeyData][0][KeyBlocktime], NumberStyles.Float, CultureInfo.InvariantCulture, out n4) ? n4 as decimal? : null,
                CoinSupply = int.TryParse(supply, out n5) ? n5 / 10e18m as decimal? : null,
                Algorithm = "Ethash",
                IsProofOfStake = true,
                IsProofOfWork = true,
                LastUpdate = DateTime.Now
            };
        }
    }
}
