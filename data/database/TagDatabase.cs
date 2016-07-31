using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using data.database.models;
using models;
using SQLite;
using Xamarin.Forms;
using System;
using data.database.helper;

namespace data.database
{
	public class TagDatabase
	{
		SQLiteAsyncConnection database;
		TagIdentifierDatabase tagIdentifierDatabase;

		public TagDatabase()
		{
			database = DependencyService.Get<ISQLiteConnection>().GetConnection();
			database.CreateTableAsync<TagDBM>().RunSynchronously();

			tagIdentifierDatabase = new TagIdentifierDatabase();
		}

		public async Task<IEnumerable<Tag>> GetAll()
		{
			var query = database.Table<TagDBM>().ToListAsync();

			Func<TagDBM, Tag> toTag = t => t.ToTag(tagIdentifierDatabase.Get(t.Id).Result);

			return await query.ContinueWith(l => l.Result.Select(toTag));
		}

		public async Task Write(IEnumerable<Tag> tags)
		{
			foreach (Tag t in tags)
			{
				var dbObj = new TagDBM(t);

				await DatabaseHelper.InsertOrUpdate(database, dbObj);

				t.Id = dbObj.Id;
			}
			await tagIdentifierDatabase.Write(tags.Select(t => t.Identifier));
		}
	}
}

