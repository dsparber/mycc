using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Helper;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Repositories.Currency;
using SQLite;

namespace MyCryptos.Core.Database
{
    public class CurrencyRepositoryMapDatabase : AbstractDatabase<CurrencyRepositoryMapDBM, CurrencyRepositoryMap, int>
    {
        public override async Task<IEnumerable<CurrencyRepositoryMapDBM>> GetAllDbObjects()
        {
            return await (await Connection).Table<CurrencyRepositoryMapDBM>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<CurrencyRepositoryMapDBM>();
        }

        public async override Task<CurrencyRepositoryMapDBM> GetDbObject(int id)
        {
            return await (await Connection).FindAsync<CurrencyRepositoryMapDBM>(p => p.Id == id);
        }

        protected override CurrencyRepositoryMapDBM Resolve(CurrencyRepositoryMap element)
        {
            return new CurrencyRepositoryMapDBM(element);
        }
    }
}

