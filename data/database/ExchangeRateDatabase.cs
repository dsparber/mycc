using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using data.database.models;
using models;
using SQLite;
using data.database.helper;
using System;

namespace data.database
{
	public class ExchangeRateDatabase : AbstractRepositoryIdDatabase<ExchangeRateDBM, ExchangeRate>
	{
		public override Task<CreateTablesResult> Create()
		{
			return Connection.CreateTableAsync<ExchangeRateDBM>();
		}

		public override async Task<IEnumerable<ExchangeRateDBM>> GetAllDbObjects()
		{
			await Create();
			return await Connection.Table<ExchangeRateDBM>().ToListAsync();
		}

		public override async Task Write(IEnumerable<ExchangeRate> data, int repositoryId)
		{
			await Task.WhenAll(data.Select(async e =>
			{
				var dbObj = new ExchangeRateDBM(e, repositoryId);
				await DatabaseHelper.InsertOrUpdate(this, dbObj);
				e.Id = dbObj.Id;
			}));
		}
	}
}

