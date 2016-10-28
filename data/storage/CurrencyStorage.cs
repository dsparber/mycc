using MyCryptos.models;
using data.database;
using data.repositories.currency;
using data.database.models;
using System.Threading.Tasks;
using System;
using MyCryptos.data.database.helper;

namespace data.storage
{
	public class CurrencyStorage : AbstractDatabaseStorage<CurrencyRepositoryDBM, CurrencyRepository, CurrencyDBM, Currency>
	{
		public CurrencyStorage() : base(new CurrencyRepositoryDatabase()) { }

		protected override async Task OnFirstLaunch()
		{
			await Add(new BittrexCurrencyRepository(null));
			await Add(new BtceCurrencyRepository(null));
			await Add(new CryptonatorCurrencyRepository(null));
			await Add(new BlockExpertsCurrencyRepository(null));
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

		public async override Task<CurrencyRepository> GetLocalRepository()
		{
			return (await Repositories()).Find(r => r is LocalCurrencyRepository);
		}
	}
}