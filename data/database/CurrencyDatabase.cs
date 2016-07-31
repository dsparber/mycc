using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using data.database.models;
using models;
using SQLite;
using Xamarin.Forms;
using data.database.helper;

namespace data.database
{
	public class CurrencyDatabase
	{
		readonly SQLiteAsyncConnection database;

		public CurrencyDatabase()
		{
			database = DependencyService.Get<ISQLiteConnection>().GetConnection();
			database.CreateTableAsync<CurrencyDBM>().RunSynchronously();
		}

		public async Task<List<Currency>> GetAll()
		{
			return new List<Currency>((await database.Table<CurrencyDBM>().ToListAsync()).Select(c => c.ToCurrency()));
		}

		public async Task<Currency> Get(int id)
		{
			return (await GetAll()).Single(c => c.Id == id);
		}

		public async Task Write(List<Currency> currencies)
		{
			foreach (var c in currencies)
			{
				var dbObj = new CurrencyDBM(c);
				await DatabaseHelper.InsertOrUpdate(database, dbObj);
				c.Id = dbObj.Id;
			}
		}
	}
}

