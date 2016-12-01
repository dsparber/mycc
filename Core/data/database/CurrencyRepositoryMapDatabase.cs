using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using data.repositories.currency;
using MyCryptos.data.database.helper;
using SQLite;

namespace data.database
{
	public class CurrencyRepositoryMapDatabase : AbstractDatabase<CurrencyRepositoryMapDBM, CurrencyRepositoryMap, int>
	{
		public override async Task<IEnumerable<CurrencyRepositoryMapDBM>> GetAllDbObjects()
		{
			return await (await Connection).Table<CurrencyRepositoryMapDBM>().ToListAsync();
		}

		protected override async Task Create(SQLiteAsyncConnection connection)
		{
			await connection.CreateTableAsync<CurrencyRepositoryMapDBM>();
		}

		public async override Task<CurrencyRepositoryMapDBM> GetDbObject(int id)
		{
			return await (await Connection).FindAsync<CurrencyRepositoryMapDBM>(p => p.Id == id);
		}

		protected override CurrencyRepositoryMapDBM Resolve(CurrencyRepositoryMap element)
		{
			return new CurrencyRepositoryMapDBM(element);
		}
	}
}

