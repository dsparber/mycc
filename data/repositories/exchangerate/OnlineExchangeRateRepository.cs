using System;
using System.Threading.Tasks;
using models;

namespace data.repositories.exchangerate
{
	public abstract class OnlineExchangeRateRepository : ExchangeRateRepository
	{
		protected OnlineExchangeRateRepository(int repositoryId) : base(repositoryId) { }

		public override async Task FetchAvailableRatesFast()
		{
			await FetchFromDatabase();
		}

		public override async Task FetchExchangeRateFast(ExchangeRate exchangeRate)
		{
			await FetchAvailableRatesFast();
		}
	}
}

