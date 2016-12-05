using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Helper;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Repositories.Currency;
using SQLite;

namespace MyCryptos.Core.Database
{
	public class CurrencyMapRepositoryDatabase : AbstractDatabase<CurrencyMapRepositoryDBM, CurrencyRepositoryMap, int>
	{
		public override async Task<IEnumerable<CurrencyMapRepositoryDBM>> GetAllDbObjects()
		{
			return await (await Connection).Table<CurrencyMapRepositoryDBM>().ToListAsync();
		}

		protected override async Task Create(SQLiteAsyncConnection connection)
		{
			await connection.CreateTableAsync<CurrencyMapRepositoryDBM>();
		}

		public async override Task<CurrencyMapRepositoryDBM> GetDbObject(int id)
		{
			return await (await Connection).FindAsync<CurrencyMapRepositoryDBM>(p => p.Id == id);
		}

		protected override CurrencyMapRepositoryDBM Resolve(CurrencyRepositoryMap element)
		{
			return new CurrencyMapRepositoryDBM(element);
		}
	}
}

