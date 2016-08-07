using System.Threading.Tasks;
using models;

namespace data.repositories.exchangerate
{
	public abstract class OnlineExchangeRateRepository : ExchangeRateRepository
	{
		protected OnlineExchangeRateRepository(int repositoryId) : base(repositoryId) { }

		public override async Task FetchFast()
		{
			await FetchFromDatabase();
		}

		public override async Task FetchExchangeRateFast(ExchangeRate exchangeRate)
		{
			await FetchFast();
		}
	}
}

