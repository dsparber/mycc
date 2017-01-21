using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Database;
using SQLite;

namespace MyCC.Core.Currency.Database
{
    public class CurrencyMapDatabase : AbstractDatabase<CurrencyMapDbm, CurrencyMapDbm, string>
    {
        public override async Task<IEnumerable<CurrencyMapDbm>> GetAllDbObjects()
        {
            var result = await (await Connection).Table<CurrencyMapDbm>().ToListAsync();
            return result;
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<CurrencyMapDbm>();
        }

        public override async Task<CurrencyMapDbm> GetDbObject(string id)
        {
            return await (await Connection).FindAsync<CurrencyMapDbm>(p => p.Id.Equals(id));
        }

        protected override CurrencyMapDbm Resolve(CurrencyMapDbm element)
        {
            return element;
        }
    }
}