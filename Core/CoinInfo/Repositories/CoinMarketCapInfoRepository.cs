using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class CoinMarketCapInfoRepository : ICoinInfoRepository
    {
        public string WebUrl(string currencyId) => $"https://coinmarketcap.com/currencies/{currencyId.FindName().ToLower()}/";

        public List<string> SupportedCoins => CurrencyConstants.FlagCoinMarketCap.CurrencyIds().ToList();

        public string Name => ConstantNames.CoinMarketCap;

        public async Task<CoinInfoData> GetInfo(string currencyId)
        {
            var json = await new Uri("https://api.coinmarketcap.com/v1/ticker/").GetJson();
            var code = currencyId.Code();

            var item = json.FirstOrDefault(token => code.Equals((string)token["symbol"]));

            var supply = item["available_supply"].ToDecimal() ?? item["total_supply"].ToDecimal();

            return new CoinInfoData(currencyId)
            {
                LastUpdate = DateTime.Now,
                CoinSupply = supply
            };
        }
    }
}
