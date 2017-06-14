using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Resources;
using MyCC.Core.Helpers;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class ReddCoinInfoRepository : ICoinInfoRepository
    {
        public string WebUrl(string currencyId) => "http://live.reddcoin.com/status";

        private static readonly Uri InfoUri = new Uri("https://live.reddcoin.com/api/status?q=getInfo");

        private const string KeyInfo = "info";
        private const string KeyBlocks = "blocks";
        private const string KeySupply = "moneysupply";
        private const string KeyDifficulty = "difficulty";

        public List<string> SupportedCoins => new List<string> { "RDD1" };

        public string Name => ConstantNames.ReddCoin;

        public async Task<CoinInfoData> GetInfo(string currencyId)
        {
            try
            {
                var json = await InfoUri.GetJson();

                return new CoinInfoData(currencyId)
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
                return new CoinInfoData(currencyId);
            }
        }
    }
}
