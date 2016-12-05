using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Database;
using MyCryptos.Core.Currency.Repositories;
using SQLite;

namespace MyCryptos.Core.Currency.Database
{
	public class CurrencyRepositoryDatabase : AbstractDatabase<CurrencyRepositoryDbm, CurrencyRepository, int>
	{
		public override async Task<IEnumerable<CurrencyRepositoryDbm>> GetAllDbObjects()
		{
			return await (await Connection).Table<CurrencyRepositoryDbm>().ToListAsync();
		}

		protected override async Task Create(SQLiteAsyncConnection connection)
		{
			await connection.CreateTableAsync<CurrencyRepositoryDbm>();
		}

		public override async Task<CurrencyRepositoryDbm> GetDbObject(int id)
		{
			return await (await Connection).FindAsync<CurrencyRepositoryDbm>(p => p.Id == id);
		}

		protected override CurrencyRepositoryDbm Resolve(CurrencyRepository element)
		{
			return new CurrencyRepositoryDbm(element);
		}
	}
}

