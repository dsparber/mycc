using System;
using System.Threading.Tasks;
using models;

namespace data.repositories.exchangerate
{
	public abstract class OnlineExchangeRateRepository : ExchangeRateRepository
	{
		protected OnlineExchangeRateRepository(int repositoryId, string name) : base(repositoryId, name) { }

		public override async Task FetchFast()
		{
			await FetchFromDatabase();
			LastFastFetch = DateTime.Now;
		}

		public override async Task FetchExchangeRateFast(ExchangeRate exchangeRate)
		{
			await FetchFast();
			LastExchangeRateFastFetch = DateTime.Now;
		}
	}
}

