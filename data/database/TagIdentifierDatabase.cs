using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using MyCryptos.models;
using SQLite;
using MyCryptos.data.database.helper;

namespace data.database
{
	public class TagIdentifierDatabase : AbstractDatabase<TagIdentifierDBM, TagIdentifier>
	{
		public override async Task<IEnumerable<TagIdentifierDBM>> GetAllDbObjects()
		{
			return await (await Connection).Table<TagIdentifierDBM>().ToListAsync();
		}

		protected override async Task Create(SQLiteAsyncConnection connection)
		{
			await connection.CreateTableAsync<TagIdentifierDBM>();
		}

		public async override Task<TagIdentifierDBM> GetDbObject(int id)
		{
			return await (await Connection).FindAsync<TagIdentifierDBM>(p => p.Id == id);
		}

		protected override TagIdentifierDBM Resolve(TagIdentifier element)
		{
			return new TagIdentifierDBM(element);
		}
	}
}