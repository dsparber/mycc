using System.Threading.Tasks;
using MyCryptos.Core.Database;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Repositories.Currency;

namespace MyCryptos.Core.Storage
{
    public class CurrencyRepositoryMapStorage : AbstractDatabaseStorage<CurrencyRepositoryMapDBM, CurrencyRepositoryMap, CurrencyRepositoryElementDBM, CurrencyRepositoryElementDBM, string>
    {
        public CurrencyRepositoryMapStorage() : base(new CurrencyRepositoryMapDatabase()) { }

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