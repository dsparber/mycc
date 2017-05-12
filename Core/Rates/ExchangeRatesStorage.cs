using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Rates.Repositories;
using MyCC.Core.Rates.Repositories.Interfaces;
using MyCC.Core.Settings;
using SQLite;
using Xamarin.Forms;

namespace MyCC.Core.Rates
{
    public class ExchangeRatesStorage
    {
        public IEnumerable<ExchangeRate> StoredRates => Repositories.SelectMany(r => r.Rates.ToList()).ToList();
        private readonly SQLiteAsyncConnection _connection;
        public readonly IEnumerable<IRateRepository> Repositories;

        private ExchangeRatesStorage()
        {
            _connection = DependencyService.Get<ISqLiteConnection>().GetConnection();
            _connection.CreateTableAsync<ExchangeRate>();

            Repositories = new List<IRateRepository> {
                new BittrexExchangeRateRepository(_connection),
                new BtceExchangeRateRepository(_connection),
                new CryptonatorExchangeRateRepository(_connection),
                new FixerIoExchangeRateRepository(_connection),
                new BitstampExchangeRateRepository(_connection),
                new KrakenExchangeRateRepository(_connection),
                new QuadrigaCxExchangeRateRepository(_connection),
                new CoinbaseExchangeRateRepository(_connection),
                new BitPayExchangeRateRepository(_connection),
                new BitfinexExchangeRateRepository(_connection),
                new CoinapultExchangeRateRepository(_connection),
                new ItBitExchangeRateRepository(_connection)
            };
        }

        public async Task LoadRates()
        {
            var all = await _connection.Table<ExchangeRate>().ToListAsync();

            foreach (var r in Repositories)
            {
                r.Rates.AddRange(all.Where(e => e != null && e.RepositoryId == r.TypeId));
            }
        }

        public static readonly ExchangeRatesStorage Instance = new ExchangeRatesStorage();

        private static IRateRepository GetRepository(RatesRepositories repository)
            => Instance.Repositories.FirstOrDefault(r => r.TypeId == (int)repository);

        public static List<IRateRepository> BitcoinRepositories => Instance.Repositories
            .Where(r => r.RatesType == RateRepositoryType.CryptoToFiat).ToList();

        public static IMultipleRatesRepository FixerIo => (IMultipleRatesRepository)GetRepository(RatesRepositories.FixerIo);
        public static IMultipleRatesRepository Btce => (IMultipleRatesRepository)GetRepository(RatesRepositories.Btce);

        public async Task UpdateRates(Action<double> progressCallback)
        {
            var i = 0;
            await Task.WhenAll(Repositories.Select(async r =>
            {
                await r.UpdateRates();
                i += 1;
                progressCallback?.Invoke((double)i / Repositories.Count());
            }));
        }
        public async Task FetchAvailableRates()
        {
            await Task.WhenAll(Repositories.Select(r => r.FetchAvailableRates()).Where(t => t != null));
        }

        public static IRateRepository PreferredBtcRepository
            => Instance.Repositories.First(r => r.TypeId == ApplicationSettings.PreferredBitcoinRepository);
    }
}