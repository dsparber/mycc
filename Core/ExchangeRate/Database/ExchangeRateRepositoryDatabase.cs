using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using MyCC.Core.ExchangeRate.Repositories;
using SQLite;

namespace MyCC.Core.ExchangeRate.Database
{
    public class ExchangeRateRepositoryDatabase : AbstractDatabase<ExchangeRateRepositoryDbm, ExchangeRateRepository, int>
    {
        public override async Task<IEnumerable<ExchangeRateRepositoryDbm>> GetAllDbObjects()
        {
            return await (await Connection).Table<ExchangeRateRepositoryDbm>().ToListAsync();
        }

        public override async Task<ExchangeRateRepositoryDbm> GetDbObject(int id)
        {
            return await (await Connection).FindAsync<ExchangeRateRepositoryDbm>(p => p.Id == id);
        }

        protected override Task Create(SQLiteAsyncConnection connection)
        {
            return connection.CreateTableAsync<ExchangeRateRepositoryDbm>();

        }

        protected override ExchangeRateRepositoryDbm Resolve(ExchangeRateRepository element)
        {
            return new ExchangeRateRepositoryDbm(element);
        }
    }
}

