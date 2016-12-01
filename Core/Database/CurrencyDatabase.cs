using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Helper;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using SQLite;

namespace MyCryptos.Core.Database
{
    public class CurrencyDatabase : AbstractDatabase<CurrencyDBM, Currency, string>
    {
        public override async Task<IEnumerable<CurrencyDBM>> GetAllDbObjects()
        {
            return await (await Connection).Table<CurrencyDBM>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<CurrencyDBM>();
        }

        public async override Task<CurrencyDBM> GetDbObject(string id)
        {
            return await (await Connection).FindAsync<CurrencyDBM>(p => p.Id.Equals(id));
        }

        protected override CurrencyDBM Resolve(Currency element)
        {
            return new CurrencyDBM(element);
        }
    }
}