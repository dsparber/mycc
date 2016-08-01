using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.interfaces;
using data.database.models;
using SQLite;
using Xamarin.Forms;

namespace data.database
{
	public class ExchangeRateRepositoryDatabase
	{
		readonly SQLiteAsyncConnection database;

		public ExchangeRateRepositoryDatabase()
		{
			database = DependencyService.Get<ISQLiteConnection>().GetConnection();
			database.CreateTableAsync<ExchangeRateRepositoryDBM>().RunSynchronously();
		}

		public async Task<IEnumerable<ExchangeRateRepositoryDBM>> GetAll()
		{
			return await database.Table<ExchangeRateRepositoryDBM>().ToListAsync();
		}
	}
}

