using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.Currency.Repositories;
using SQLite;

namespace MyCryptos.Core.Currency.Database
{
    public class CurrencyRepositoryDatabase : AbstractDatabase<CurrencyRepositoryDBM, CurrencyRepository, int>
    {
        public override async Task<IEnumerable<CurrencyRepositoryDBM>> GetAllDbObjects()
        {
            return await (await Connection).Table<CurrencyRepositoryDBM>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<CurrencyRepositoryDBM>();
        }

        public override async Task<CurrencyRepositoryDBM> GetDbObject(int id)
        {
            return await (await Connection).FindAsync<CurrencyRepositoryDBM>(p => p.Id == id);
        }

        protected override CurrencyRepositoryDBM Resolve(CurrencyRepository element)
        {
            return new CurrencyRepositoryDBM(element);
        }
    }
}

