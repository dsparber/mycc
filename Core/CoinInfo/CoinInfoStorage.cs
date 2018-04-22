using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.CoinInfo.Repositories;
using MyCC.Core.Database;
using SQLite;

namespace MyCC.Core.CoinInfo
{
    public class CoinInfoStorage
    {
        private readonly List<CoinInfoData> _elements;

        private readonly SQLiteAsyncConnection _dbConnection;

        private readonly List<ICoinInfoRepository> _repositories;

        private CoinInfoStorage()
        {
            _elements = new List<CoinInfoData>();

            _dbConnection = DatabaseUtil.OldConnection;
            _dbConnection.CreateTableAsync<CoinInfoData>();

            _repositories = new List<ICoinInfoRepository> {
                new BlockchainCoinInfoRepository(),
                new BlockrCoinInfoRepository(),
                new EtherchainCoinInfoRepository(),
                new EtcchainCoinInfoRepository(),
                new ReddCoinInfoRepository(),
                new BulwarkInfoRepository(),
                new CoinMarketCapInfoRepository(),
                new CryptoIdCoinInfoRepository(),
                new BlockExpertsCoinInfoRepository()
            };
        }

        public async Task<CoinInfoData> FetchInfo(string currencyId)
        {
            var info = new CoinInfoData(currencyId);

            foreach (var r in GetExplorer(currencyId))
            {
                info = info.AddUpdate(await r.GetInfo(currencyId));
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

        public IEnumerable<ICoinInfoRepository> GetExplorer(string currencyId) => _repositories.Where(r => r.SupportedCoins.Contains(currencyId));

        public CoinInfoData Get(string currencyId) => _elements.Find(e => string.Equals(e.CurrencyId, currencyId));

        public static readonly CoinInfoStorage Instance = new CoinInfoStorage();

        public static IEnumerable<string> SupportetCurrencies
            => Instance._repositories.SelectMany(r => r.SupportedCoins).Distinct();
    }
}
