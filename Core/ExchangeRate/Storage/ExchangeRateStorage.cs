using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Storage;
using MyCryptos.Core.ExchangeRate.Database;
using MyCryptos.Core.ExchangeRate.Repositories;

namespace MyCryptos.Core.ExchangeRate.Storage
{
	public class ExchangeRateStorage : AbstractDatabaseStorage<ExchangeRateRepositoryDbm, ExchangeRateRepository, ExchangeRateDbm, Model.ExchangeRate, string>
	{
		public ExchangeRateStorage() : base(new ExchangeRateRepositoryDatabase()) { }


		protected override async Task OnFirstLaunch()
		{
			await Add(new BittrexExchangeRateRepository(default(int)));
			await Add(new BtceExchangeRateRepository(default(int)));
			await Add(new LocalExchangeRateRepository(default(int)));
			await Add(new CryptonatorExchangeRateRepository(default(int)));
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