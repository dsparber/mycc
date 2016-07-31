using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using data.database.models;
using models;
using SQLite;
using Xamarin.Forms;
using data.database.helper;
using System;

namespace data.database
{
	public class ExchangeRateDatabase
	{

		readonly SQLiteAsyncConnection database;
		readonly CurrencyDatabase currencyDatabase;

		public ExchangeRateDatabase()
		{
			database = DependencyService.Get<ISQLiteConnection>().GetConnection();
			database.CreateTableAsync<CurrencyDBM>().RunSynchronously();

			currencyDatabase = new CurrencyDatabase();
		}

		public async Task<List<ExchangeRate>> GetAll()
		{
			var listDBM = await database.Table<ExchangeRateDBM>().ToListAsync();

			Func<ExchangeRateDBM, Task<ExchangeRate>> convert = async c => c.ToExchangeRate(await currencyDatabase.Get(c.ReferenceCurrencyId), await currencyDatabase.Get(c.SecondaryCurrencyId));

			return new List<ExchangeRate>(await Task.WhenAll(listDBM.Select(convert)));
		}

		public async Task Write(List<ExchangeRate> exchangeRates)
		{
			await Task.WhenAll(exchangeRates.Select(async e =>
			{
				var dbObj = new ExchangeRateDBM(e);
				await DatabaseHelper.InsertOrUpdate(database, dbObj);
				e.Id = dbObj.Id;
			}));
		}
	}
}

