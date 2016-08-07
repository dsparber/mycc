using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using data.database.models;
using models;
using SQLite;
using data.database.helper;

namespace data.database
{
	public class CurrencyDatabase : AbstractEntityRepositoryIdDatabase<CurrencyDBM, Currency>
	{
		protected override async Task<CreateTablesResult> Create()
		{
			return await ConnectionWithoutCreate.CreateTableAsync<CurrencyDBM>();
		}

		public override async Task<IEnumerable<CurrencyDBM>> GetAllDbObjects()
		{
			return await (await Connection()).Table<CurrencyDBM>().ToListAsync();
		}

		public override async Task Write(IEnumerable<Currency> data, int repositoryId)
		{
			await Task.WhenAll(data.Select(async c =>
			{
				var dbObj = new CurrencyDBM(c, repositoryId);
				await DatabaseHelper.InsertOrUpdate(this, dbObj);
				c.Id = dbObj.Id;
			}));
		}
	}
}

