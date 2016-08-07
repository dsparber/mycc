using System.Linq;
using System.Threading.Tasks;
using data.database;
using data.database.helper;
using data.database.models;
using data.factories;
using data.repositories.exchangerate;
using models;

namespace data.storage
{
	public class ExchangeRateStorage : AbstractStorage<ExchangeRateRepositoryDBM, ExchangeRateRepository, ExchangeRateDBM, ExchangeRate>
	{
		public async Task FetchExchangeRate(ExchangeRate exchangeRate)
		{
			await Task.WhenAll((await Repositories()).Select(x => x.FetchExchangeRate(exchangeRate)));
		}

		public async Task FetchExchangeRateFast(ExchangeRate exchangeRate)
		{
			await Task.WhenAll((await Repositories()).Select(x => x.FetchExchangeRateFast(exchangeRate)));
		}

		public override AbstractRepositoryDatabase<ExchangeRateRepositoryDBM> GetDatabase()
		{
			return new ExchangeRateRepositoryDatabase();
		}

		protected override AbstractStorage<ExchangeRateRepositoryDBM, ExchangeRateRepository, ExchangeRateDBM, ExchangeRate> CreateInstance()
		{
			return new ExchangeRateStorage();
		}

		protected override ExchangeRateRepository Resolve(ExchangeRateRepositoryDBM obj)
		{
			return ExchangeRateRepositoryFactory.create(obj);
		}
	}
}

