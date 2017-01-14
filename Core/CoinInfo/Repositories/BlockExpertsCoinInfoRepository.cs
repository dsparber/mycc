using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using MyCryptos.Core.Currency.Repositories;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.Resources;

namespace MyCryptos.Core.CoinInfo.Repositories
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
			var client = new HttpClient { MaxResponseContentBufferSize = 256000 };

			var heightTask = client.GetAsync(GetUri(currency, KeyHeight));
			var hashrateTask = client.GetAsync(GetUri(currency, KeyHashrate));
			var diffTask = client.GetAsync(GetUri(currency, KeyDifficulty));
			var supplyTask = client.GetAsync(GetUri(currency, KeySupply));

			var heigh = int.Parse(await (await heightTask).Content.ReadAsStringAsync());
			var hashrate = decimal.Parse(await (await hashrateTask).Content.ReadAsStringAsync(), CultureInfo.InvariantCulture);
			var diff = decimal.Parse(await (await diffTask).Content.ReadAsStringAsync(), CultureInfo.InvariantCulture);
			var supply = decimal.Parse(await (await supplyTask).Content.ReadAsStringAsync(), CultureInfo.InvariantCulture);

			return new CoinInfoData(currency)
			{
				BlockHeight = heigh != 0 ? heigh as int? : null,
				CoinSupply = supply != 0 ? supply as decimal? : null,
				Hashrate = hashrate != 0 ? hashrate as decimal? : null,
				Difficulty = diff != 0 ? diff as decimal? : null
			};
		}
	}
}
