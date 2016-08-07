using models;
using data.database;
using data.factories;
using data.repositories.currency;
using data.database.models;
using data.database.helper;
using System.Threading.Tasks;

namespace data.storage
{
	public class CurrencyStorage : AbstractStorage<CurrencyRepositoryDBM, CurrencyRepository, CurrencyDBM, Currency>
	{
		protected override AbstractStorage<CurrencyRepositoryDBM, CurrencyRepository, CurrencyDBM, Currency> CreateInstance()
		{
			return new CurrencyStorage();
		}

		protected override CurrencyRepository Resolve(CurrencyRepositoryDBM obj)
		{
			return CurrencyRepositoryFactory.create(obj);
		}

		public override AbstractRepositoryDatabase<CurrencyRepositoryDBM> GetDatabase()
		{
			return new CurrencyRepositoryDatabase();
		}

		protected override async Task OnFirstLaunch()
		{
			await GetDatabase().AddRepository(new CurrencyRepositoryDBM { Type = CurrencyRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY });
			await GetDatabase().AddRepository(new CurrencyRepositoryDBM { Type = CurrencyRepositoryDBM.DB_TYPE_BTCE_REPOSITORY });
			await GetDatabase().AddRepository(new CurrencyRepositoryDBM { Type = CurrencyRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY });
		}
	}
}