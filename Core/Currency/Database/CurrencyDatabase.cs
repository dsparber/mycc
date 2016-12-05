using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using SQLite;

namespace MyCryptos.Core.Currency.Database
{
    public class CurrencyDatabase : AbstractDatabase<CurrencyDbm, Model.Currency, string>
    {
        public override async Task<IEnumerable<CurrencyDbm>> GetAllDbObjects()
        {
            return await (await Connection).Table<CurrencyDbm>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<CurrencyDbm>();
        }

        public override async Task<CurrencyDbm> GetDbObject(string id)
        {
            return await (await Connection).FindAsync<CurrencyDbm>(p => p.Id.Equals(id));
        }

        protected override CurrencyDbm Resolve(Model.Currency element)
        {
            return new CurrencyDbm(element);
        }
    }
}