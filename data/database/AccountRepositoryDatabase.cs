using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using SQLite;
using Xamarin.Forms;

namespace data.database
{
	public class AccountRepositoryDatabase
	{
		SQLiteAsyncConnection database;

		public AccountRepositoryDatabase()
		{
			database = DependencyService.Get<ISQLiteConnection>().GetConnection();
			database.CreateTableAsync<AccountRepositoryDBM>();
		}

		public async Task<IEnumerable<AccountRepositoryDBM>> GetRepositories()
		{
			var query = database.Table<AccountRepositoryDBM>();
			return await query.ToListAsync();
		}
	}
}

