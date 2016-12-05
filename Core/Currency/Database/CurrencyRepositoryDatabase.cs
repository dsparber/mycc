using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Helper;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Repositories.Currency;
using SQLite;

namespace MyCryptos.Core.Database
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

        public async override Task<CurrencyRepositoryDBM> GetDbObject(int id)
        {
            return await (await Connection).FindAsync<CurrencyRepositoryDBM>(p => p.Id == id);
        }

        protected override CurrencyRepositoryDBM Resolve(CurrencyRepository element)
        {
            return new CurrencyRepositoryDBM(element);
        }
    }
}

