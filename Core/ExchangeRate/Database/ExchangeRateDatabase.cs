using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Helper;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using SQLite;

namespace MyCryptos.Core.Database
{
    public class ExchangeRateDatabase : AbstractDatabase<ExchangeRateDBM, ExchangeRate, string>
    {
        public override async Task<IEnumerable<ExchangeRateDBM>> GetAllDbObjects()
        {
            return await (await Connection).Table<ExchangeRateDBM>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<ExchangeRateDBM>();
        }

        public async override Task<ExchangeRateDBM> GetDbObject(string id)
        {
            return await (await Connection).FindAsync<ExchangeRateDBM>(p => p.Id.Equals(id));
        }

        protected override ExchangeRateDBM Resolve(ExchangeRate element)
        {
            return new ExchangeRateDBM(element);
        }
    }
}

