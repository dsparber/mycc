using System;
using System.Threading.Tasks;
using data.database.models;
using models;

namespace data.repositories.exchangerate
{
	public class LocalExchangeRateRepository : ExchangeRateRepository
	{
		public LocalExchangeRateRepository(string name) : base(ExchangeRateRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY, name) { }

		public override async Task Fetch()
		{
			await FetchFromDatabase();
			LastFetch = DateTime.Now;
		}

		public override async Task FetchFast()
		{
			await Fetch();
			LastFastFetch = DateTime.Now;
		}

		public override async Task FetchExchangeRate(ExchangeRate exchangeRate)
		{
			await Fetch();
			LastExchangeRateFetch = DateTime.Now;
		}

		public override async Task FetchExchangeRateFast(ExchangeRate exchangeRate)
		{
			await Fetch();
			LastExchangeRateFastFetch = DateTime.Now;
		}
	}
}

