using System.Collections.Generic;
using System.Threading.Tasks;
using data.database;
using models;

namespace data.repositories.currency
{
	public abstract class CurrencyRepository
	{
		public List<Currency> Currencies;
		public string RepositoryName;

		protected CurrencyRepository()
		{
			Currencies = new List<Currency>();
		}

		public abstract Task Fetch();

		public abstract Task FetchFast();

		protected async Task FetchFromDatabase()
		{
			var db = new CurrencyDatabase();
			Currencies = new List<Currency>(await db.GetAll());
		}

		protected async Task WriteToDatabase()
		{
			var db = new CurrencyDatabase();
			await db.Write(Currencies);
		}
	}
}

