using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
			var hashrateTask = client.GetAsync(GetUri(currency, KeyHeight));
			var diffTask = client.GetAsync(GetUri(currency, KeyHeight));
			var supplyTask = client.GetAsync(GetUri(currency, KeyHeight));

			var heigh = await (await heightTask).Content.ReadAsStringAsync();
			var hashrate = await (await heightTask).Content.ReadAsStringAsync();
			var diff = await (await heightTask).Content.ReadAsStringAsync();
			var supply = await (await heightTask).Content.ReadAsStringAsync();

			return new CoinInfoData(currency)
			{
				Height = int.Parse(heigh),
				CoinSupply = decimal.Parse(supply, CultureInfo.InvariantCulture),
				Hashrate = decimal.Parse(hashrate, CultureInfo.InvariantCulture),
				Difficulty = decimal.Parse(diff, CultureInfo.InvariantCulture),
			};
		}
	}
}
