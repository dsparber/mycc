using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using MyCryptos.models;
using MyCryptos.data.database.helper;
using SQLite;

namespace data.database
{
	public class AccountDatabase : AbstractDatabase<AccountDBM, Account, int>
	{
		public override async Task<IEnumerable<AccountDBM>> GetAllDbObjects()
		{
			return await (await Connection).Table<AccountDBM>().ToListAsync();
		}

		protected override async Task Create(SQLiteAsyncConnection connection)
		{
			await connection.CreateTableAsync<AccountDBM>();
		}

		public async override Task<AccountDBM> GetDbObject(int id)
		{
			return await (await Connection).FindAsync<AccountDBM>(p => p.Id == id);
		}

		protected override AccountDBM Resolve(Account element)
		{
			return new AccountDBM(element);
		}
	}
}