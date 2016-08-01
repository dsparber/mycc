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
		public int RepositoryId;

		protected CurrencyRepository(int repositoryId)
		{
			Currencies = new List<Currency>();
			RepositoryId = repositoryId;
		}

		public abstract Task Fetch();

		public abstract Task FetchFast();

		protected async Task FetchFromDatabase()
		{
			var db = new CurrencyDatabase();
			Currencies = new List<Currency>(await db.GetAll(RepositoryId));
		}

		protected async Task WriteToDatabase()
		{
			var db = new CurrencyDatabase();
			await db.Write(Currencies, RepositoryId);
		}
	}
}

