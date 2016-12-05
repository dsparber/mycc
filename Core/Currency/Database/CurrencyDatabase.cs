using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using SQLite;

namespace MyCryptos.Core.Currency.Database
{
    public class CurrencyDatabase : AbstractDatabase<CurrencyDBM, Model.Currency, string>
    {
        public override async Task<IEnumerable<CurrencyDBM>> GetAllDbObjects()
        {
            return await (await Connection).Table<CurrencyDBM>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<CurrencyDBM>();
        }

        public override async Task<CurrencyDBM> GetDbObject(string id)
        {
            return await (await Connection).FindAsync<CurrencyDBM>(p => p.Id.Equals(id));
        }

        protected override CurrencyDBM Resolve(Model.Currency element)
        {
            return new CurrencyDBM(element);
        }
    }
}