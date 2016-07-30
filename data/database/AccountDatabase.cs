using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.database.models;
using models;
using SQLite;
using Xamarin.Forms;

namespace data.database
{
	public class AccountDatabase
	{
		SQLiteAsyncConnection database;

		public AccountDatabase()
		{
			database = DependencyService.Get<ISQLiteConnection>().GetConnection();
			database.CreateTableAsync<AccountDBM>().RunSynchronously();
		}

		public async Task<IEnumerable<Account>> GetAccounts(int repositoryId)
		{
			var query = database.Table<AccountDBM>().Where(a => a.RepositoryId == repositoryId); 

			return await query.ToListAsync().ContinueWith(q => q.Result.Select(a => a.ToAccount()));
		}

		public async Task WriteAccounts(int repositoryId, IEnumerable<Account> accounts)
		{
			foreach (Account a in accounts)
			{
				var rowsAffected = await database.UpdateAsync(a);
				if (rowsAffected == 0)
				{
					await database.InsertAsync(a);
				}
			}
		}
	}
}

