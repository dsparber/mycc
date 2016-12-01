using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using SQLite;
using MyCryptos.data.database.helper;

namespace data.database
{
	public class TagAccountMapDatabase : AbstractDatabase<TagAccountMapDBM, TagAccountMapDBM, int>
	{
		public override async Task<IEnumerable<TagAccountMapDBM>> GetAllDbObjects()
		{
			return await (await Connection).Table<TagAccountMapDBM>().ToListAsync();
		}

		protected override async Task Create(SQLiteAsyncConnection connection)
		{
			await connection.CreateTableAsync<TagAccountMapDBM>();
		}

		public async override Task<TagAccountMapDBM> GetDbObject(int id)
		{
			return await (await Connection).FindAsync<TagAccountMapDBM>(p => p.Id == id);
		}

		protected override TagAccountMapDBM Resolve(TagAccountMapDBM element)
		{
			return new TagAccountMapDBM();
		}
	}
}