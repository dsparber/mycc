using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.helper;
using data.database.models;
using SQLite;

namespace data.database
{
	public class AvailableRatesRepositoryDatabase : AbstractRepositoryDatabase<AvailableRatesRepositoryDBM>
	{
		public override async Task<IEnumerable<AvailableRatesRepositoryDBM>> GetRepositories()
		{
			var query = (await Connection()).Table<AvailableRatesRepositoryDBM>();
			return await query.ToListAsync();
		}

		protected override Task<CreateTablesResult> Create()
		{
			return ConnectionWithoutCreate.CreateTableAsync<AvailableRatesRepositoryDBM>();
		}
	}
}

