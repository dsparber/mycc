using models;
using data.database;
using data.factories;
using data.repositories.currency;
using data.database.models;
using data.database.helper;
using System.Threading.Tasks;
using System;

namespace data.storage
{
	public class CurrencyStorage : AbstractDatabaseStorage<CurrencyRepositoryDBM, CurrencyRepository, CurrencyDBM, Currency>
	{
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
			await GetDatabase().AddRepository(new CurrencyRepositoryDBM { Type = CurrencyRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY });
			await GetDatabase().AddRepository(new CurrencyRepositoryDBM { Type = CurrencyRepositoryDBM.DB_TYPE_BTCE_REPOSITORY });
			await GetDatabase().AddRepository(new CurrencyRepositoryDBM { Type = CurrencyRepositoryDBM.DB_TYPE_CRYPTONATOR_REPOSITORY });
			await GetDatabase().AddRepository(new CurrencyRepositoryDBM { Type = CurrencyRepositoryDBM.DB_TYPE_BLOCK_EXPERTS_REPOSITORY });
		}

		static CurrencyStorage instance { get; set; }

		public static CurrencyStorage Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new CurrencyStorage();
				}
				return instance;
			}
		}

		public async Task<Currency> GetByString(string s)
		{
			return (await AllElements()).Find(c => string.Equals(s, c.Code, StringComparison.OrdinalIgnoreCase) || string.Equals(s, c.Name, StringComparison.OrdinalIgnoreCase));
		}

		public async Task Add(Currency currency)
		{
			var local = (await Repositories()).Find(r => r is LocalCurrencyRepository);
			if (local != null)
			{
				await local.Add(currency);
			}
		}
	}
}