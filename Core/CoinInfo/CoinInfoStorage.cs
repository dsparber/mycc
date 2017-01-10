using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.CoinInfo.Repositories;
using SQLite;
using Xamarin.Forms;

namespace MyCryptos.Core.CoinInfo
{
	public class CoinInfoStorage
	{
		public readonly List<CoinInfoData> Elements;

		private readonly SQLiteAsyncConnection _dbConnection;

		private readonly List<ICoinInfoRepository> _repositories;

		private CoinInfoStorage()
		{
			Elements = new List<CoinInfoData>();

			_dbConnection = DependencyService.Get<ISQLiteConnection>().GetConnection();

			_dbConnection.CreateTableAsync<CoinInfoData>();
			Task.Factory.StartNew(async () => Elements.AddRange(await _dbConnection.Table<CoinInfoData>().ToListAsync()));

			_repositories = new List<ICoinInfoRepository> {
				new CryptoIdCoinInfoRepository(),
				new BlockExpertsCoinInfoRepository()
			};
		}

		public async Task<CoinInfoData> GetInfo(Currency.Model.Currency currency)
		{
			if (Elements.Any(i => i.CurrencyCode.Equals(currency.Code)))
			{
				return Elements.Find(i => i.CurrencyCode.Equals(currency.Code));
			}

			foreach (var r in _repositories)
			{
				if (r.SupportedCoins.Contains(currency))
				{
					try
					{
						var info = await r.GetInfo(currency);
						if (info == null) continue;

						if (Elements.Contains(info))
						{
							Elements.Remove(info);
							await _dbConnection.UpdateAsync(info);
						}
						else {
							await _dbConnection.InsertAsync(info);
						}
						Elements.Add(info);

						return info;

					}
					catch (Exception)
					{
						continue;
					}
				}
			}
			return null;
		}

		public List<ICoinInfoRepository> GetExplorer(Currency.Model.Currency currency) => _repositories.Where(r => r.SupportedCoins.Contains(currency)).ToList();

		public static CoinInfoStorage Instance = new CoinInfoStorage();
	}
}
