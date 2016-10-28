using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using SQLite;
using MyCryptos.data.database.helper;
using MyCryptos.models;

namespace data.database
{
	public class ExchangeRateDatabase : AbstractDatabase<ExchangeRateDBM, ExchangeRate>
	{
		public override async Task<IEnumerable<ExchangeRateDBM>> GetAllDbObjects()
		{
			return await (await Connection).Table<ExchangeRateDBM>().ToListAsync();
		}

		protected override async Task Create(SQLiteAsyncConnection connection)
		{
			await connection.CreateTableAsync<ExchangeRateDBM>();
		}

		public async override Task<ExchangeRateDBM> GetDbObject(int id)
		{
			return await (await Connection).FindAsync<ExchangeRateDBM>(p => p.Id == id);
		}

		protected override ExchangeRateDBM Resolve(ExchangeRate element)
		{
			return new ExchangeRateDBM(element);
		}
	}
}

