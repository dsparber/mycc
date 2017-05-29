using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Resources;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Helpers;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class ReddCoinInfoRepository : ICoinInfoRepository
    {
        public string WebUrl(Currency currency) => "http://live.reddcoin.com/status";

        private static readonly Uri InfoUri = new Uri("https://live.reddcoin.com/api/status?q=getInfo");

        private const string KeyInfo = "info";
        private const string KeyBlocks = "blocks";
        private const string KeySupply = "moneysupply";
        private const string KeyDifficulty = "difficulty";

        public List<Currency> SupportedCoins => new List<Currency> { "RDD1".Find() };

        public string Name => ConstantNames.ReddCoin;

        public async Task<CoinInfoData> GetInfo(Currency currency)
        {
            try
            {
                var json = await InfoUri.GetJson();

                return new CoinInfoData(currency)
                {
                    LastUpdate = DateTime.Now,
                    Difficulty = json[KeyInfo][KeyDifficulty].ToDecimal(),
                    BlockHeight = json[KeyInfo][KeyBlocks].ToInt(),
                    CoinSupply = json[KeyInfo][KeySupply].ToDecimal()
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
