using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using data.database.models;
using models;
using SQLite;
using data.database.helper;

namespace data.database
{
	public class ExchangeRateDatabase : AbstractEntityRepositoryIdDatabase<ExchangeRateDBM, ExchangeRate>
	{
		protected override Task<CreateTablesResult> Create()
		{
			return ConnectionWithoutCreate.CreateTableAsync<ExchangeRateDBM>();
		}

		public override async Task<IEnumerable<ExchangeRateDBM>> GetAllDbObjects()
		{
			return await (await Connection()).Table<ExchangeRateDBM>().ToListAsync();
		}

		public override async Task Write(IEnumerable<ExchangeRate> data, int repositoryId)
		{
			await DatabaseHelper.InsertOrUpdate(this, data.Select(e => new ExchangeRateDBM(e, repositoryId)));
		}
	}
}

