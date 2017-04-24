using System.Linq;
using MyCC.Core.Abstract.Storage;
using MyCC.Core.Currency.Database;
using MyCC.Core.Currency.Repositories;

namespace MyCC.Core.Currency.Storage
{
    internal class CurrencyRepositoryMapStorage : AbstractDatabaseStorage<CurrencyMapRepositoryDbm, CurrencyRepositoryMap, CurrencyMapDbm, CurrencyMapDbm, string>
    {
        private CurrencyRepositoryMapStorage() : base(new CurrencyMapRepositoryDatabase())
        {
            Repositories.Add(new CurrencyRepositoryMap());
        }

        private static CurrencyRepositoryMapStorage _instance;

        public static CurrencyRepositoryMapStorage Instance => _instance ?? (_instance = new CurrencyRepositoryMapStorage());

        public override CurrencyRepositoryMap LocalRepository => Repositories.FirstOrDefault();
    }
}