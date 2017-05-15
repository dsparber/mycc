using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.CoinInfo.Repositories;
using MyCC.Core.Helpers;
using SQLite;
using Xamarin.Forms;

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

            _dbConnection = DependencyService.Get<ISqLiteConnection>().GetOldConnection();
            _dbConnection.CreateTableAsync<CoinInfoData>();

            _repositories = new List<ICoinInfoRepository> {
                new CryptoIdCoinInfoRepository(),
                new BlockExpertsCoinInfoRepository(),
                new BlockchainCoinInfoRepository(),
                new BlockrCoinInfoRepository(),
                new EtherchainCoinInfoRepository()
            };
        }

        public async Task<CoinInfoData> FetchInfo(Currencies.Model.Currency currency)
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

        public IEnumerable<ICoinInfoRepository> GetExplorer(Currencies.Model.Currency currency) => _repositories.Where(r => r.SupportedCoins.Contains(currency));

        public CoinInfoData Get(Currencies.Model.Currency currency) => _elements.Find(e => string.Equals(e.CurrencyCode, currency.Code));

        public static readonly CoinInfoStorage Instance = new CoinInfoStorage();

        public static IEnumerable<Currencies.Model.Currency> SupportetCurrencies
            => Instance._repositories.SelectMany(r => r.SupportedCoins).Distinct();
    }
}
