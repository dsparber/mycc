using System;
using System.Threading.Tasks;
using models;

namespace data.repositories.exchangerate
{
	public class LocalExchangeRateRepository : ExchangeRateRepository
	{
		public LocalExchangeRateRepository(int repositoryId) : base(repositoryId) { }

		public override async Task FetchAvailableRates()
		{
			await FetchFromDatabase();
		}

		public override async Task FetchAvailableRatesFast()
		{
			await FetchAvailableRates();
		}

		public override async Task FetchExchangeRate(ExchangeRate exchangeRate)
		{
			await FetchAvailableRates();
		}

		public override async Task FetchExchangeRateFast(ExchangeRate exchangeRate)
		{
			await FetchAvailableRates();
		}
	}
}

