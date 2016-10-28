using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using data.repositories.currency;
using MyCryptos.data.database.helper;
using SQLite;

namespace data.database
{
	public class CurrencyRepositoryDatabase : AbstractDatabase<CurrencyRepositoryDBM, CurrencyRepository>
	{
		public override async Task<IEnumerable<CurrencyRepositoryDBM>> GetAllDbObjects()
		{
			return await (await Connection).Table<CurrencyRepositoryDBM>().ToListAsync();
		}

		protected override async Task Create(SQLiteAsyncConnection connection)
		{
			await connection.CreateTableAsync<CurrencyRepositoryDBM>();
		}

		public async override Task<CurrencyRepositoryDBM> GetDbObject(int id)
		{
			return await (await Connection).FindAsync<CurrencyRepositoryDBM>(p => p.Id == id);
		}

		protected override CurrencyRepositoryDBM Resolve(CurrencyRepository element)
		{
			return new CurrencyRepositoryDBM(element);
		}
	}
}

