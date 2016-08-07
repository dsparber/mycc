using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.helper;
using data.database.models;
using SQLite;

namespace data.database
{
	public class ExchangeRateRepositoryDatabase : AbstractRepositoryDatabase<ExchangeRateRepositoryDBM>
	{
		public override async Task<IEnumerable<ExchangeRateRepositoryDBM>> GetRepositories()
		{
			var query = (await Connection()).Table<ExchangeRateRepositoryDBM>();
			return await query.ToListAsync();
		}

		protected override Task<CreateTablesResult> Create()
		{
			return ConnectionWithoutCreate.CreateTableAsync<ExchangeRateRepositoryDBM>();
		}
	}
}

