using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using data.database.helper;
using System.Linq;
using models;
using SQLite;

namespace data.database
{
	public class TagIdentifierDatabase : AbstractEntityDatabase<TagIdentifierDBM, TagIdentifier>
	{
		public override async Task<IEnumerable<TagIdentifierDBM>> GetAllDbObjects()
		{
			return await (await Connection()).Table<TagIdentifierDBM>().ToListAsync();
		}

		public override async Task Write(IEnumerable<TagIdentifier> data)
		{
			await Task.WhenAll(data.Select(async i =>
			{
				var dbObj = new TagIdentifierDBM(i);
				await DatabaseHelper.InsertOrUpdate(this, dbObj);
			}));
		}

		protected override Task<CreateTablesResult> Create()
		{
			return ConnectionWithoutCreate.CreateTableAsync<TagIdentifierDBM>();
		}
	}
}