using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Currency.Repositories;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Resources;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class BlockExpertsCoinInfoRepository : ICoinInfoRepository
    {
        private Uri GetUri(Currency.Model.Currency coin, string action) => new Uri($"https://www.blockexperts.com/api?coin={coin.Code.ToLower()}&action={action}");
        private const string KeyHeight = "getheight";
        private const string KeyDifficulty = "getdifficulty";
        private const string KeyHashrate = "getnetworkghps";
        private const string KeySupply = "getmoneysupply";

        public List<Currency.Model.Currency> SupportedCoins => CurrencyStorage.Instance.RepositoryOfType<BlockExpertsCurrencyRepository>().Currencies;

        public string Name => I18N.BlockExperts;

        public async Task<CoinInfoData> GetInfo(Currency.Model.Currency currency)
        {
            var client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = 256000 };

            var heightTask = client.GetAsync(GetUri(currency, KeyHeight));
            var hashrateTask = client.GetAsync(GetUri(currency, KeyHashrate));
            var diffTask = client.GetAsync(GetUri(currency, KeyDifficulty));
            var supplyTask = client.GetAsync(GetUri(currency, KeySupply));

            int heigh; int.TryParse(await (await heightTask).Content.ReadAsStringAsync(), out heigh);
            var hashrate = TryParse(await (await hashrateTask).Content.ReadAsStringAsync());
            var diff = TryParse(await (await diffTask).Content.ReadAsStringAsync());
            var supply = TryParse(await (await supplyTask).Content.ReadAsStringAsync());

            return new CoinInfoData(currency)
            {
                BlockHeight = heigh != 0 ? heigh as int? : null,
                CoinSupply = supply != 0 ? supply as decimal? : null,
                Hashrate = hashrate != 0 ? hashrate as decimal? : null,
                Difficulty = diff != 0 ? diff as decimal? : null,
                LastUpdate = DateTime.Now
            };
        }

        private static decimal TryParse(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? 0 : decimal.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
