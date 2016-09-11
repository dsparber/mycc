using models;
using data.database;
using data.factories;
using data.database.models;
using data.database.helper;
using System.Threading.Tasks;
using data.repositories.availablerates;

namespace data.storage
{
	public class AvailableRatesStorage : AbstractStorage<AvailableRatesRepositoryDBM, AvailableRatesRepository, ExchangeRate>
	{
		public override AbstractRepositoryDatabase<AvailableRatesRepositoryDBM> GetDatabase()
		{
			return new AvailableRatesRepositoryDatabase();
		}

		protected override AvailableRatesRepository Resolve(AvailableRatesRepositoryDBM obj)
		{
			return AvailableRatesFactory.create(obj);
		}

		protected override async Task OnFirstLaunch()
		{
			await GetDatabase().AddRepository(new AvailableRatesRepositoryDBM { Type = AvailableRatesRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY });
			await GetDatabase().AddRepository(new AvailableRatesRepositoryDBM { Type = AvailableRatesRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY });
			await GetDatabase().AddRepository(new AvailableRatesRepositoryDBM { Type = AvailableRatesRepositoryDBM.DB_TYPE_BTCE_REPOSITORY });
			await GetDatabase().AddRepository(new AvailableRatesRepositoryDBM { Type = AvailableRatesRepositoryDBM.DB_TYPE_CRYPTONATOR_REPOSITORY });
		}

		static AvailableRatesStorage instance { get; set; }

		public static AvailableRatesStorage Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new AvailableRatesStorage();
				}
				return instance;
			}
		}

		public async Task<ExchangeRate> ExchangeRateWithCurrency(Currency currency)
		{
			foreach (var r in await Repositories())
			{
				var e = r.ExchangeRateWithCurrency(currency);
				if (e != null)
				{
					return e;
				}
			}
			return null;
		}

		public async Task<bool> IsAvailable(ExchangeRate exchangeRate)
		{
			foreach (var r in await Repositories())
			{
				if (r.IsAvailable(exchangeRate))
				{
					return true;
				}
			}
			return false;
		}
	}
}