using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Helper;
using MyCryptos.Core.Database.Models;
using SQLite;

namespace MyCryptos.Core.Database
{
    public class CurrencyMapDatabase : AbstractDatabase<CurrencyRepositoryElementDBM, CurrencyRepositoryElementDBM, string>
    {
        public override async Task<IEnumerable<CurrencyRepositoryElementDBM>> GetAllDbObjects()
        {
            return await (await Connection).Table<CurrencyRepositoryElementDBM>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<CurrencyRepositoryElementDBM>();
        }

        public async override Task<CurrencyRepositoryElementDBM> GetDbObject(string id)
        {
            return await (await Connection).FindAsync<CurrencyRepositoryElementDBM>(p => p.Id.Equals(id));
        }

        protected override CurrencyRepositoryElementDBM Resolve(CurrencyRepositoryElementDBM element)
        {
            return element;
        }
    }
}