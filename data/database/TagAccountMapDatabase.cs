using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using System.Linq;
using models;
using SQLite;
using Xamarin.Forms;

namespace data.database
{
	public class TagAccountMapDatabase
	{
		readonly SQLiteAsyncConnection database;

		public TagAccountMapDatabase()
		{
			database = DependencyService.Get<ISQLiteConnection>().GetConnection();
			database.CreateTableAsync<TagAccountMapDBM>().RunSynchronously();
		}

		public async Task<IEnumerable<TagAccountMapDBM>> GetAll()
		{
			return await database.Table<TagAccountMapDBM>().ToListAsync();
		}

		public async Task<IEnumerable<TagAccountMapDBM>> GetForAccountId(int accountId)
		{
			return (await GetAll()).Where(x => x.AccountId == accountId);
		}

		public async Task DeleteWithAccountId(int accountId)
		{
			foreach (var t in await GetForAccountId(accountId))
			{
				await database.DeleteAsync(t);
			}
		}

		public async Task Write(Account account, Tag tag)
		{
			var dbObj = new TagAccountMapDBM { TagId = tag.Id.Value, AccountId = account.Id.Value };

			await database.InsertAsync(dbObj);
		}
	}
}