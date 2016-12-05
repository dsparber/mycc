using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using SQLite;

namespace MyCryptos.Core.Currency.Database
{
    public class CurrencyMapDatabase : AbstractDatabase<CurrencyMapDBM, CurrencyMapDBM, string>
    {
        public override async Task<IEnumerable<CurrencyMapDBM>> GetAllDbObjects()
        {
            return await (await Connection).Table<CurrencyMapDBM>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<CurrencyMapDBM>();
        }

        public override async Task<CurrencyMapDBM> GetDbObject(string id)
        {
            return await (await Connection).FindAsync<CurrencyMapDBM>(p => p.Id.Equals(id));
        }

        protected override CurrencyMapDBM Resolve(CurrencyMapDBM element)
        {
            return element;
        }
    }
}