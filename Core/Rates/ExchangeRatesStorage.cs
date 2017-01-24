using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.Rates.Repositories;
using MyCC.Core.Rates.Repositories.Interfaces;
using SQLite;
using Xamarin.Forms;

namespace MyCC.Core.Rates
{
    public class ExchangeRatesStorage
    {
        public IEnumerable<ExchangeRate> StoredRates => Repositories.SelectMany(r => r.Rates);
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
                new FixerIoExchangeRateRepository(_connection)
            };
        }

        public async Task LoadRates()
        {
            var all = await _connection.Table<ExchangeRate>().ToListAsync();

            foreach (var r in Repositories)
            {
                r.Rates.AddRange(all.Where(e => e.RepositoryId == r.TypeId));
            }
        }

        public static readonly ExchangeRatesStorage Instance = new ExchangeRatesStorage();

        private static IRateRepository GetRepository(RatesRepositories repository)
            => Instance.Repositories.FirstOrDefault(r => r.TypeId == (int)repository);

        public static IMultipleRatesRepository FixerIo => (IMultipleRatesRepository)GetRepository(RatesRepositories.FixerIo);
        public static IMultipleRatesRepository Btce => (IMultipleRatesRepository)GetRepository(RatesRepositories.Btce);

        public async Task UpdateRates()
        {
            await Task.WhenAll(Repositories.Select(r => r.UpdateRates()));
        }
        public async Task FetchAvailableRates()
        {
            await Task.WhenAll(Repositories.Select(r => r.FetchAvailableRates()));
        }
    }
}