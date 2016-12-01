using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Helper;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Repositories.AvailableRates;
using SQLite;

namespace MyCryptos.Core.Database
{
    public class AvailableRatesRepositoryDatabase : AbstractDatabase<AvailableRatesRepositoryDBM, AvailableRatesRepository, int>
    {
        public override async Task<IEnumerable<AvailableRatesRepositoryDBM>> GetAllDbObjects()
        {
            return await (await Connection).Table<AvailableRatesRepositoryDBM>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<AvailableRatesRepositoryDBM>();
        }

        public async override Task<AvailableRatesRepositoryDBM> GetDbObject(int id)
        {
            return await (await Connection).FindAsync<AvailableRatesRepositoryDBM>(p => p.Id == id);
        }

        protected override AvailableRatesRepositoryDBM Resolve(AvailableRatesRepository element)
        {
            return new AvailableRatesRepositoryDBM(element);
        }
    }
}

