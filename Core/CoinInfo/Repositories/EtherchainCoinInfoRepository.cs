using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class EtherchainCoinInfoRepository : ICoinInfoRepository
    {
        private static readonly Uri UrlStats = new Uri("https://etherchain.org/api/basic_stats");
        private static readonly Uri UrlSupply = new Uri("https://api.etherscan.io/api?module=stats&action=ethsupply");

        private const string KeyData = "data";
        private const string KeyDifficulty = "difficulty";
        private const string KeyBlockCount = "blockCount";
        private const string KeyNumber = "number";
        private const string KeyStats = "stats";
        private const string KeyBlocktime = "blockTime";
        private const string KeyHashrate = "hashRate";
        private const string KeyResult = "hashRate";

        public List<Currency.Model.Currency> SupportedCoins => new List<Currency.Model.Currency> { new Currency.Model.Currency("ETH", "Ethereum", true) };

        public string Name => I18N.Etherchain;

        public async Task<CoinInfoData> GetInfo(Currency.Model.Currency currency)
        {
            var client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = 256000 };

            var json = JObject.Parse(await (await client.GetAsync(UrlStats)).Content.ReadAsStringAsync())[KeyData];
            var supply = (string)JObject.Parse(await (await client.GetAsync(UrlSupply)).Content.ReadAsStringAsync())[KeyResult];

            return new CoinInfoData(currency)
            {
                BlockHeight = int.TryParse((string)json[KeyBlockCount][KeyNumber], out int n1) ? n1 as int? : null,
                Hashrate = decimal.TryParse((string)json[KeyStats][KeyHashrate], out decimal n2) ? n2 as decimal? : null,
                Difficulty = decimal.TryParse((string)json[KeyStats][KeyDifficulty], out decimal n3) ? n3 as decimal? : null,
                Blocktime = decimal.TryParse((string)json[KeyStats][KeyBlocktime], out decimal n4) ? n4 as decimal? : null,
                CoinSupply = int.TryParse(supply, out int n5) ? n5 / 10e18m as decimal? : null,
                Algorithm = "Ethash",
                IsProofOfStake = true,
                IsProofOfWork = true,
                LastUpdate = DateTime.Now
            };
        }
    }
}
