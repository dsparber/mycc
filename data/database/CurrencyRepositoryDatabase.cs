using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using SQLite;
using Xamarin.Forms;

namespace data.database
{
	public class CurrencyRepositoryDatabase
	{
		SQLiteAsyncConnection database;

		public CurrencyRepositoryDatabase()
		{
			database = DependencyService.Get<ISQLiteConnection>().GetConnection();
			database.CreateTableAsync<CurrencyRepositoryDBM>().RunSynchronously();
		}

		public async Task<IEnumerable<CurrencyRepositoryDBM>> GetRepositories()
		{
			var query = database.Table<CurrencyRepositoryDBM>();
			return await query.ToListAsync();
		}
	}
}

