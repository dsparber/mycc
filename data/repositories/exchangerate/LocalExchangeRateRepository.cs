using System.Threading.Tasks;
using data.database.models;
using models;

namespace data.repositories.exchangerate
{
	public class LocalExchangeRateRepository : ExchangeRateRepository
	{
		public LocalExchangeRateRepository() : base(ExchangeRateRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY) { }

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

