using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Storage;
using MyCC.Core.ExchangeRate.Database;
using MyCC.Core.ExchangeRate.Repositories;

// TODO make private? 
namespace MyCC.Core.ExchangeRate.Storage
{
    public class AvailableRatesStorage : AbstractStorage<AvailableRatesRepositoryDbm, AvailableRatesRepository>
    {
        private AvailableRatesStorage() : base(new AvailableRatesRepositoryDatabase()) { }

        protected override async Task OnFirstLaunch()
        {
            await Add(new BittrexAvailableRatesRepository(default(int)));
            await Add(new BtceAvailableRatesRepository(default(int)));
            await Add(new CryptonatorAvailableRatesRepository(default(int)));
        }

        private static AvailableRatesStorage _instance;

        public static AvailableRatesStorage Instance => _instance ?? (_instance = new AvailableRatesStorage());

        public bool IsAvailable(Model.ExchangeRate exchangeRate)
        {
            return Repositories.Any(r => r.IsAvailable(exchangeRate));
        }
    }
}