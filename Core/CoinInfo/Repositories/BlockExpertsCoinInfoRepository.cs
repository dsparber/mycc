using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using MyCC.Core.Currencies;
using MyCC.Core.Helpers;
using MyCC.Core.Resources;

namespace MyCC.Core.CoinInfo.Repositories
{
    public class BlockExpertsCoinInfoRepository : ICoinInfoRepository
    {
        private Uri GetUri(Currencies.Model.Currency coin, string action) => new Uri($"https://www.blockexperts.com/api?coin={coin.Code.ToLower()}&action={action}");
        private const string KeyHeight = "getheight";
        private const string KeyDifficulty = "getdifficulty";
        private const string KeyHashrate = "getnetworkghps";
        private const string KeySupply = "getmoneysupply";

        public List<Currencies.Model.Currency> SupportedCoins => CurrencyStorage.CurrenciesOf(CurrencyConstants.FlagBlockExperts).ToList();

        public string Name => I18N.BlockExperts;

        public async Task<CoinInfoData> GetInfo(Currencies.Model.Currency currency)
        {
            var client = new HttpClient(new NativeMessageHandler()) { MaxResponseContentBufferSize = 256000 };

            var heightTask = client.GetAsync(GetUri(currency, KeyHeight));
            var hashrateTask = client.GetAsync(GetUri(currency, KeyHashrate));
            var diffTask = client.GetAsync(GetUri(currency, KeyDifficulty));
            var supplyTask = client.GetAsync(GetUri(currency, KeySupply));

            Func<Task<HttpResponseMessage>, Task<string>> getString = async task =>
            {
                try
                {
                    var response = await task;
                    var s = await response.Content.ReadAsStringAsync();
                    return s;
                }
                catch (Exception e)
                {
                    e.LogError();
                    return null;
                }
            };

            int heigh; int.TryParse(await getString(heightTask), out heigh);
            var hashrate = TryParse(await getString(hashrateTask));
            var diff = TryParse(await getString(diffTask));
            var supply = TryParse(await getString(supplyTask));

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
            return string.IsNullOrWhiteSpace(s) ? 0 : decimal.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
        }
    }
}
