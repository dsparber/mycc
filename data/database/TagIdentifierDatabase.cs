using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using data.database.helper;
using System.Linq;
using models;
using SQLite;

namespace data.database
{
	public class TagIdentifierDatabase : AbstractDatabase<TagIdentifierDBM, TagIdentifier>
	{
		public override async Task<IEnumerable<TagIdentifierDBM>> GetAllDbObjects()
		{
			await Create();
			return await Connection.Table<TagIdentifierDBM>().ToListAsync();
		}

		public override async Task Write(IEnumerable<TagIdentifier> data)
		{
			await Task.WhenAll(data.Select(async i =>
			{
				var dbObj = new TagIdentifierDBM(i);
				await DatabaseHelper.InsertOrUpdate(this, dbObj);
			}));
		}

		public override Task<CreateTablesResult> Create()
		{
			return Connection.CreateTableAsync<TagIdentifierDBM>();
		}
	}
}