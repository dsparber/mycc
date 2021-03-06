﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;
using System.Globalization;
using MyCC.Core.Helpers;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class EtcchainCoinInfoRepository : ICoinInfoRepository
    {
        private static readonly Uri Url = new Uri("https://etcchain.com/api/v1/getIndex");

        public string WebUrl(string currencyId) => "https://etcchain.com/explorer";

        private const string KeyEtc = "etc";
        private const string KeyDifficulty = "difficulty";
        private const string KeyHashrate = "hash_rate";
        private const string KeySupply = "available_supply";


        public List<string> SupportedCoins => new List<string> { "ETC1" };

        public string Name => ConstantNames.EtcChain;

        public async Task<CoinInfoData> GetInfo(string currencyId)
        {
            var client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = 256000 };

            try
            {
                var json = JObject.Parse(await (await client.GetAsync(Url)).Content.ReadAsStringAsync());


                return new CoinInfoData(currencyId)
                {
                    LastUpdate = DateTime.Now,
                    Difficulty = decimal.TryParse((string)json[KeyEtc][KeyDifficulty], NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d as decimal? : null,
                    Hashrate = decimal.TryParse((string)json[KeyEtc][KeyHashrate], NumberStyles.Float, CultureInfo.InvariantCulture, out d) ? d as decimal? : null,
                    CoinSupply = decimal.TryParse((string)json[KeyEtc][KeySupply], NumberStyles.Float, CultureInfo.InvariantCulture, out d) ? d as decimal? : null,
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
