using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;
using System.Globalization;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class EtcchainCoinInfoRepository : ICoinInfoRepository
    {
        private static readonly Uri Url = new Uri("https://etcchain.com/api/v1/getIndex");

        private const string KeyEtc = "etc";
        private const string KeyDifficulty = "difficulty";
        private const string KeyHashrate = "hash_rate";
        private const string KeySupply = "available_supply";


        public List<Currency> SupportedCoins => new List<Currency> { "ETC1".Find() };

        public string Name => ConstantNames.EtcChain;

        public async Task<CoinInfoData> GetInfo(Currency currency)
        {
            var client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = 256000 };

            try
            {
                var json = JObject.Parse(await (await client.GetAsync(Url)).Content.ReadAsStringAsync());


                return new CoinInfoData(currency)
                {
                    Difficulty = decimal.TryParse((string)json[KeyEtc][KeyDifficulty], NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d as decimal? : null,
                    Hashrate = decimal.TryParse((string)json[KeyEtc][KeyHashrate], NumberStyles.Float, CultureInfo.InvariantCulture, out d) ? d as decimal? : null,
                    CoinSupply = decimal.TryParse((string)json[KeyEtc][KeySupply], NumberStyles.Float, CultureInfo.InvariantCulture, out d) ? d as decimal? : null,
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
