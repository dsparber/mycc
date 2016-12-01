using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using data.repositories.account;
using MyCryptos.data.database.helper;
using SQLite;

namespace data.database
{
	public class AccountRepositoryDatabase : AbstractDatabase<AccountRepositoryDBM, AccountRepository, int>
	{
		public async override Task<IEnumerable<AccountRepositoryDBM>> GetAllDbObjects()
		{
			return await (await Connection).Table<AccountRepositoryDBM>().ToListAsync();
		}

		public async override Task<AccountRepositoryDBM> GetDbObject(int id)
		{
			return await(await Connection).FindAsync<AccountRepositoryDBM>(p => p.Id == id);
		}

		protected override Task Create(SQLiteAsyncConnection connection)
		{
			return connection.CreateTableAsync<AccountRepositoryDBM>();

		}

		protected override AccountRepositoryDBM Resolve(AccountRepository element)
		{
			return new AccountRepositoryDBM(element);
		}
	}
}

