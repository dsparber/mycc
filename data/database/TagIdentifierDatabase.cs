using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using data.database.helper;
using System.Linq;
using models;
using SQLite;
using Xamarin.Forms;

namespace data.database
{
	public class TagIdentifierDatabase
	{
		readonly SQLiteAsyncConnection database;

		public TagIdentifierDatabase()
		{
			database = DependencyService.Get<ISQLiteConnection>().GetConnection();
			database.CreateTableAsync<TagIdentifierDBM>().RunSynchronously();
		}

		public async Task<IEnumerable<TagIdentifier>> GetAll()
		{
			var query = database.Table<TagIdentifierDBM>().ToListAsync();
			return await query.ContinueWith(l => l.Result.Select(t => t.ToTagIdentifier()));
		}

		public async Task<TagIdentifier> Get(int id)
		{
			var query = database.Table<TagIdentifierDBM>().ToListAsync();
			return await query.ContinueWith(l => l.Result.Single(i => i.Id == id).ToTagIdentifier());
		}

		public async Task Write(IEnumerable<TagIdentifier> tagIdentifiers)
		{
			foreach (TagIdentifier i in tagIdentifiers)
			{
				var dbObj = new TagIdentifierDBM(i);

				await DatabaseHelper.InsertOrUpdate(database, dbObj);
			}
		}
	}
}

