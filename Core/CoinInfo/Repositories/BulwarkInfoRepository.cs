using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Resources;
using MyCC.Core.Helpers;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class BulwarkInfoRepository : ICoinInfoRepository
    {
        public string WebUrl(string currencyId) => "https://explorer.bulwarkcrypto.com";

        private static readonly Uri InfoUri = new Uri("https://explorer.bulwarkcrypto.com/api/coin");


        public List<string> SupportedCoins => new List<string> { "BWK1" };

        public string Name => ConstantNames.Bulwark;

        public async Task<CoinInfoData> GetInfo(string currencyId)
        {
            try
            {
                var json = await InfoUri.GetJson();

                return new CoinInfoData(currencyId)
                {
                    CoinSupply = json["supply"].ToDecimal(),
                    BlockHeight = json["blocks"].ToInt(),
                    Hashrate = json["netHash"].ToDecimal() / 1e9M,
                    Difficulty = json["diff"].ToDecimal()
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
