using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.helper;
using data.database.models;
using SQLite;

namespace data.database
{
	public class AccountRepositoryDatabase : AbstractRepositoryDatabase<AccountRepositoryDBM>
	{
		public override async Task<IEnumerable<AccountRepositoryDBM>> GetRepositories()
		{
			var query = (await Connection()).Table<AccountRepositoryDBM>();
			return await query.ToListAsync();
		}

		protected override Task<CreateTablesResult> Create()
		{
			return ConnectionWithoutCreate.CreateTableAsync<AccountRepositoryDBM>();
		}
	}
}

