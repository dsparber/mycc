using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.ExchangeRate.Repositories;
using SQLite;

namespace MyCryptos.Core.ExchangeRate.Database
{
    public class AvailableRatesRepositoryDatabase : AbstractDatabase<AvailableRatesRepositoryDbm, AvailableRatesRepository, int>
    {
        public override async Task<IEnumerable<AvailableRatesRepositoryDbm>> GetAllDbObjects()
        {
            return await (await Connection).Table<AvailableRatesRepositoryDbm>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<AvailableRatesRepositoryDbm>();
        }

        public override async Task<AvailableRatesRepositoryDbm> GetDbObject(int id)
        {
            return await (await Connection).FindAsync<AvailableRatesRepositoryDbm>(p => p.Id == id);
        }

        protected override AvailableRatesRepositoryDbm Resolve(AvailableRatesRepository element)
        {
            return new AvailableRatesRepositoryDbm(element);
        }
    }
}

