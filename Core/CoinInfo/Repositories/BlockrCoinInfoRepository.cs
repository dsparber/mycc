﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;
using Newtonsoft.Json.Linq;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class BlockrCoinInfoRepository : ICoinInfoRepository
    {
        private static Uri GetUri(Currency coin) => new Uri($"https://{coin.Code.ToLower()}.blockr.io/api/v1/coin/info");

        private const string JsonKeyData = "data";

        private const string JsonKeyVolume = "volume";
        private const string JsonKeyCurrent = "current";
        private const string JsonKeyAll = "all";

        private const string JsonKeyLastBlock = "last_block";
        private const string JsonKeyBlockNumber = "nb";

        private const string JsonKeyNextDiff = "next_difficulty";
        private const string JsonKeyDiff = "difficulty";

        public string Name => ConstantNames.Blockr;

        public List<Currency> SupportedCoins => new List<string> { "btc", "ltc", "ppc", "mec", "qrk", "dgc", "tbtc" }
            .Select(s => new Currency(s, true)).ToList();

        public async Task<CoinInfoData> GetInfo(Currency currency)
        {
            var client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = 256000 };

            var data = await client.GetAsync(GetUri(currency));
            try
            {
                var dataJson = JObject.Parse(await data.Content.ReadAsStringAsync())[JsonKeyData];

                var volumeJson = dataJson[JsonKeyVolume];
                var volumeCurrent = (string)volumeJson[JsonKeyCurrent];
                var volumeMax = (string)volumeJson[JsonKeyAll];

                var lastBlockJson = dataJson[JsonKeyLastBlock];
                var height = (string)lastBlockJson[JsonKeyBlockNumber];

                var diffJson = dataJson[JsonKeyNextDiff];
                var diff = (string)diffJson[JsonKeyDiff];

                return new CoinInfoData(currency)
                {
                    CoinSupply = decimal.Parse(volumeCurrent, CultureInfo.InvariantCulture),
                    MaxCoinSupply = decimal.Parse(volumeMax, CultureInfo.InvariantCulture),
                    BlockHeight = int.Parse(height, CultureInfo.InvariantCulture),
                    Difficulty = decimal.Parse(diff, CultureInfo.InvariantCulture),
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
