using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.CoinInfo.Repositories;
using MyCC.Core.Settings;
using SQLite;
using Xamarin.Forms;

namespace MyCC.Core.CoinInfo
{
	public class CoinInfoStorage
	{
		private readonly List<CoinInfoData> _elements;

		private readonly SQLiteAsyncConnection _dbConnection;

		private readonly List<ICoinInfoRepository> _repositories;

		private static bool alreadyCalled;

		private CoinInfoStorage()
		{
			_elements = new List<CoinInfoData>();

			_dbConnection = DependencyService.Get<ISqLiteConnection>().GetConnection();
			_dbConnection.CreateTableAsync<CoinInfoData>();

			Task.Run(async () =>
			{
				if (alreadyCalled) return;
				alreadyCalled = true;

				if (ApplicationSettings.FirstLaunch)
				{
					await _dbConnection.ExecuteAsync("CREATE TABLE CoinInfos ( Code varchar primary key not null, " +
													 "Algorithm varchar, " +
													 "Hashrate float, " +
													 "Difficulty float, " +
													 "CoinSupply float, " +
													 "MaxSupply float, " +
													 "Height integer, " +
													 "PoS integer, " +
													 "PoW integer, " +
													 "Blocktime float , " +
													 "Blockreward float, " +
													 "LastUpdate integer)");
				}
				if (ApplicationSettings.VersionLastLaunch < new Version("0.5.0"))
				{
					await _dbConnection.ExecuteAsync("ALTER TABLE CoinInfos ADD COLUMN MaxSupply float;");
					await _dbConnection.ExecuteAsync("ALTER TABLE CoinInfos ADD COLUMN Blockreward float;");
				}
				if (ApplicationSettings.VersionLastLaunch < new Version("0.5.23"))
				{
					await _dbConnection.ExecuteAsync("ALTER TABLE CoinInfos ADD COLUMN LastUpdate INTEGER;");
				}
				_elements.AddRange(await _dbConnection.Table<CoinInfoData>().ToListAsync());
			});

			_repositories = new List<ICoinInfoRepository> {
				new CryptoIdCoinInfoRepository(),
				new BlockExpertsCoinInfoRepository(),
				new BlockchainCoinInfoRepository(),
				new BlockrCoinInfoRepository()
			};
		}

		public async Task<CoinInfoData> FetchInfo(Currency.Model.Currency currency)
		{
			var info = new CoinInfoData(currency);

			foreach (var r in GetExplorer(currency))
			{
				info = info.AddUpdate(await r.GetInfo(currency));
			}

			if (_elements.Contains(info))
			{
				_elements.Remove(info);
				await _dbConnection.UpdateAsync(info);
			}
			else
			{
				await _dbConnection.InsertOrReplaceAsync(info);
			}
			_elements.Add(info);

			return info;
		}

		public IEnumerable<ICoinInfoRepository> GetExplorer(Currency.Model.Currency currency) => _repositories.Where(r => r.SupportedCoins.Contains(currency));

		public CoinInfoData Get(Currency.Model.Currency currency) => _elements.Find(e => string.Equals(e.CurrencyCode, currency.Code));

		public static readonly CoinInfoStorage Instance = new CoinInfoStorage();
	}
}
