using System.Threading.Tasks;
using models;

namespace data.repositories.exchangerate
{
	public class LocalExchangeRateRepository : ExchangeRateRepository
	{
		public LocalExchangeRateRepository(int repositoryId) : base(repositoryId) { }

		public override async Task Fetch()
		{
			await FetchFromDatabase();
		}

		public override async Task FetchFast()
		{
			await Fetch();
		}

		public override async Task FetchExchangeRate(ExchangeRate exchangeRate)
		{
			await Fetch();
		}

		public override async Task FetchExchangeRateFast(ExchangeRate exchangeRate)
		{
			await Fetch();
		}
	}
}

