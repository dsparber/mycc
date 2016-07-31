using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.database.models;
using data.database.helper;
using models;
using SQLite;
using Xamarin.Forms;

namespace data.database
{
	public class AccountDatabase
	{
		readonly SQLiteAsyncConnection database;
		readonly TagDatabase tagDatabase;
		readonly TagAccountMapDatabase tagAccountMapDatabase;

		public AccountDatabase()
		{
			database = DependencyService.Get<ISQLiteConnection>().GetConnection();
			database.CreateTableAsync<AccountDBM>().RunSynchronously();

			tagDatabase = new TagDatabase();
			tagAccountMapDatabase = new TagAccountMapDatabase();
		}

		public async Task<IEnumerable<Account>> GetAccounts(int repositoryId)
		{
			var query = database.Table<AccountDBM>().Where(a => a.RepositoryId == repositoryId); 
			var accounts = await query.ToListAsync().ContinueWith(q => q.Result.Select(a => a.ToAccount()));

			foreach (var a in accounts)
			{
				var tagIds = (await tagAccountMapDatabase.GetForAccountId(a.Id.Value)).Select(t => t.TagId);

				a.Tags = new List<Tag>((await tagDatabase.GetAll()).Where(t => tagIds.Contains(t.Id.Value)));
			}

			return accounts;
		}

		public async Task WriteAccounts(int repositoryId, IEnumerable<Account> accounts)
		{
			foreach (var a in accounts)
			{
				var dbObj = new AccountDBM(a, repositoryId);
				await DatabaseHelper.InsertOrUpdate(database, dbObj);
				a.Id = dbObj.Id;

				await tagDatabase.Write(a.Tags);
				await tagAccountMapDatabase.DeleteWithAccountId(a.Id.Value);

				foreach (var t in a.Tags)
				{
					await tagAccountMapDatabase.Write(a, t);
				}
			}
		}
	}
}

