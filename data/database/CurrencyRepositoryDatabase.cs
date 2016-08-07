using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.helper;
using data.database.models;
using SQLite;

namespace data.database
{
	public class CurrencyRepositoryDatabase : AbstractRepositoryDatabase<CurrencyRepositoryDBM>
	{
		public override async Task<IEnumerable<CurrencyRepositoryDBM>> GetRepositories()
		{
			var query = (await Connection()).Table<CurrencyRepositoryDBM>();
			return await query.ToListAsync();
		}

		protected override Task<CreateTablesResult> Create()
		{
			return ConnectionWithoutCreate.CreateTableAsync<CurrencyRepositoryDBM>();
		}
	}
}

