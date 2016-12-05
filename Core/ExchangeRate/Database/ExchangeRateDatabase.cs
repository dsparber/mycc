using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using SQLite;

namespace MyCryptos.Core.ExchangeRate.Database
{
    public class ExchangeRateDatabase : AbstractDatabase<ExchangeRateDbm, Model.ExchangeRate, string>
    {
        public override async Task<IEnumerable<ExchangeRateDbm>> GetAllDbObjects()
        {
            return await (await Connection).Table<ExchangeRateDbm>().ToListAsync();
        }

        protected override async Task Create(SQLiteAsyncConnection connection)
        {
            await connection.CreateTableAsync<ExchangeRateDbm>();
        }

        public override async Task<ExchangeRateDbm> GetDbObject(string id)
        {
            return await (await Connection).FindAsync<ExchangeRateDbm>(p => p.Id.Equals(id));
        }

        protected override ExchangeRateDbm Resolve(Model.ExchangeRate element)
        {
            return new ExchangeRateDbm(element);
        }
    }
}

