using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using SQLite;

namespace MyCryptos.Core.ExchangeRate.Database
{
    public class ExchangeRateDatabase : AbstractDatabase<ExchangeRateDBM, Model.ExchangeRate, string>
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

        protected override ExchangeRateDBM Resolve(Model.ExchangeRate element)
        {
            return new ExchangeRateDBM(element);
        }
    }
}

