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
		}

		public override async Task FetchExchangeRateFast(ExchangeRate exchangeRate)
		{
			await FetchFast();
		}
	}
}

