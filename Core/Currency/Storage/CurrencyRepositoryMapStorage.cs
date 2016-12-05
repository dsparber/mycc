using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Storage;
using MyCryptos.Core.Currency.Database;
using MyCryptos.Core.Currency.Repositories;

// TODO Refactor -> Remove this class or make it private
namespace MyCryptos.Core.Currency.Storage
{
    public class CurrencyRepositoryMapStorage : AbstractDatabaseStorage<CurrencyMapRepositoryDbm, CurrencyRepositoryMap, CurrencyMapDbm, CurrencyMapDbm, string>
    {
        public CurrencyRepositoryMapStorage() : base(new CurrencyMapRepositoryDatabase()) { }

        protected override async Task OnFirstLaunch()
        {
            await Add(new CurrencyRepositoryMap());
        }

        static CurrencyRepositoryMapStorage instance { get; set; }

        public static CurrencyRepositoryMapStorage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CurrencyRepositoryMapStorage();
                }
                return instance;
            }
        }

        public override CurrencyRepositoryMap LocalRepository
        {
            get
            {
                return Repositories.Find(r => r is CurrencyRepositoryMap);
            }
        }
    }
}