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
	public class TagDatabase : AbstractEntityDatabase<TagDBM, Tag>
	{
		TagIdentifierDatabase tagIdentifierDatabase;

		public TagDatabase()
		{
			tagIdentifierDatabase = new TagIdentifierDatabase();
		}

		public override async Task<IEnumerable<TagDBM>> GetAllDbObjects()
		{
			return await (await Connection()).Table<TagDBM>().ToListAsync();
		}

		public override async Task Write(IEnumerable<Tag> data)
		{
			await Task.WhenAll(data.Select(async t =>
			{
				var dbObj = new TagDBM(t);
				await DatabaseHelper.InsertOrUpdate(this, dbObj);
				t.Id = dbObj.Id;
			}));
			await tagIdentifierDatabase.Write(data.Select(t => t.Identifier));
		}

		protected override Task<CreateTablesResult> Create()
		{
			return ConnectionWithoutCreate.CreateTableAsync<TagDBM>();
		}
	}
}

