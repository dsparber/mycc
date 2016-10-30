using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using data.repositories.exchangerate;
using MyCryptos.data.database.helper;
using SQLite;

namespace data.database
{
	public class ExchangeRateRepositoryDatabase : AbstractDatabase<ExchangeRateRepositoryDBM, ExchangeRateRepository, int>
	{
		public async override Task<IEnumerable<ExchangeRateRepositoryDBM>> GetAllDbObjects()
		{
			return await (await Connection).Table<ExchangeRateRepositoryDBM>().ToListAsync();
		}

		public async override Task<ExchangeRateRepositoryDBM> GetDbObject(int id)
		{
			return await (await Connection).FindAsync<ExchangeRateRepositoryDBM>(p => p.Id == id);
		}

		protected override Task Create(SQLiteAsyncConnection connection)
		{
			return connection.CreateTableAsync<ExchangeRateRepositoryDBM>();

		}

		protected override ExchangeRateRepositoryDBM Resolve(ExchangeRateRepository element)
		{
			return new ExchangeRateRepositoryDBM(element);
		}
	}
}

