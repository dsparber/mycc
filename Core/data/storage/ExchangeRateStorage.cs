using System.Linq;
using System.Threading.Tasks;
using data.database;
using data.database.models;
using data.repositories.exchangerate;
using MyCryptos.models;

namespace data.storage
{
	public class ExchangeRateStorage : AbstractDatabaseStorage<ExchangeRateRepositoryDBM, ExchangeRateRepository, ExchangeRateDBM, ExchangeRate, string>
	{
		public ExchangeRateStorage() : base(new ExchangeRateRepositoryDatabase()) { }


		protected override async Task OnFirstLaunch()
		{
			await Add(new BittrexExchangeRateRepository(null));
			await Add(new BtceExchangeRateRepository(null));
			await Add(new LocalExchangeRateRepository(null));
			await Add(new CryptonatorExchangeRateRepository(null));
		}

		static ExchangeRateStorage instance { get; set; }

		public static ExchangeRateStorage Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ExchangeRateStorage();
				}
				return instance;
			}
		}

		public async Task FetchNew()
		{
			await Task.WhenAll(Repositories.Select(x => x.FetchNew()));
		}

		public override ExchangeRateRepository LocalRepository
		{
			get
			{
				return Repositories.Find(r => r is LocalExchangeRateRepository);
			}
		}
	}
}