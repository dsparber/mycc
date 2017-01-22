using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Storage;
using MyCC.Core.ExchangeRate.Database;
using MyCC.Core.ExchangeRate.Repositories;

namespace MyCC.Core.ExchangeRate.Storage
{
    public class ExchangeRateStorage : AbstractDatabaseStorage<ExchangeRateRepositoryDbm, ExchangeRateRepository, ExchangeRateDbm, Model.ExchangeRate, string>
    {
        private ExchangeRateStorage() : base(new ExchangeRateRepositoryDatabase()) { }


        protected override async Task OnFirstLaunch()
        {
            await Add(new BittrexExchangeRateRepository(default(int)));
            await Add(new BtceExchangeRateRepository(default(int)));
            await Add(new LocalExchangeRateRepository(default(int)));
            await Add(new CryptonatorExchangeRateRepository(default(int)));
        }

        private static ExchangeRateStorage _instance;

        public static ExchangeRateStorage Instance => _instance ?? (_instance = new ExchangeRateStorage());

        public async Task FetchNew()
        {
            await Task.WhenAll(Repositories.Select(x => x.FetchNew()));
        }

        public async Task FetchOnline(List<Model.ExchangeRate> neededRates)
        {
            await Task.WhenAll(Repositories.OfType<OnlineExchangeRateRepository>().Select(x => x.FetchOnline(neededRates)));
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