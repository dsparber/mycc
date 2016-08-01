using System.Collections.Generic;
using System.Threading.Tasks;
using data.database;
using models;

namespace data.repositories.exchangerate
{
	public abstract class ExchangeRateRepository
	{
		public List<ExchangeRate> ExchangeRates;
		public string RepositoryName;
		public int RepositoryId;

		protected ExchangeRateRepository(int repositoryId)
		{
			ExchangeRates = new List<ExchangeRate>();
			RepositoryId = repositoryId;
		}

		public abstract Task FetchAvailableRates();

		public abstract Task FetchAvailableRatesFast();

		public abstract Task FetchExchangeRate(ExchangeRate exchangeRate);

		public abstract Task FetchExchangeRateFast(ExchangeRate exchangeRate);

		protected async Task FetchFromDatabase()
		{
			var db = new ExchangeRateDatabase();
			ExchangeRates = new List<ExchangeRate>(await db.GetAll(RepositoryId));
		}

		protected async Task WriteToDatabase()
		{
			var db = new ExchangeRateDatabase();
			await db.Write(ExchangeRates, RepositoryId);
		}
	}
}

