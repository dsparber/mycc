using System;
using System.Threading.Tasks;
using data.database;
using data.database.helper;
using data.database.models;
using data.repositories.general;
using models;

namespace data.repositories.exchangerate
{
	public abstract class ExchangeRateRepository : AbstractRepository<ExchangeRateDBM, ExchangeRate>
	{
		public DateTime LastExchangeRateFastFetch { get; protected set; }
		public DateTime LastExchangeRateFetch { get; protected set; }

		protected ExchangeRateRepository(int repositoryId, string name) : base(repositoryId, name) { }

		protected override AbstractEntityRepositoryIdDatabase<ExchangeRateDBM, ExchangeRate> GetDatabase()
		{
			return new ExchangeRateDatabase();
		}

		public abstract Task FetchExchangeRate(ExchangeRate exchangeRate);

		public abstract Task FetchExchangeRateFast(ExchangeRate exchangeRate);
	}
}

