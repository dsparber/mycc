using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Storage;
using MyCC.Core.Currency.Database;
using MyCC.Core.Currency.Repositories;

namespace MyCC.Core.Currency.Storage
{
    public class CurrencyStorage : AbstractDatabaseStorage<CurrencyRepositoryDbm, CurrencyRepository, CurrencyDbm, Model.Currency, string>
    {
        private CurrencyStorage() : base(new CurrencyRepositoryDatabase())
        {
            Repositories.Add(new LocalCurrencyRepository(CurrencyRepositoryDbm.DbTypeLocalRepository));
            Repositories.Add(new BittrexCurrencyRepository(CurrencyRepositoryDbm.DbTypeBittrexRepository));
            Repositories.Add(new BtceCurrencyRepository(CurrencyRepositoryDbm.DbTypeBtceRepository));
            Repositories.Add(new CryptonatorCurrencyRepository(CurrencyRepositoryDbm.DbTypeCryptonatorRepository));
            Repositories.Add(new BlockExpertsCurrencyRepository(CurrencyRepositoryDbm.DbTypeBlockExpertsRepository));
            Repositories.Add(new CryptoIdCurrencyRepository(CurrencyRepositoryDbm.DbTypeCryptoidRepository));
            Repositories.Add(new OpenexchangeCurrencyRepository(CurrencyRepositoryDbm.DbTypeOpenExchangeRepository));

            BeforeOnlineFetching = ClearElements();
        }

        private static CurrencyStorage _instance;

        public static CurrencyStorage Instance => _instance ?? (_instance = new CurrencyStorage());

        public Model.Currency GetByString(string s)
        {
            return AllElements.Find(c => string.Equals(s, c.Code, StringComparison.OrdinalIgnoreCase) || string.Equals(s, c.Name, StringComparison.OrdinalIgnoreCase));
        }

        public override CurrencyRepository LocalRepository
        {
            get
            {
                return Repositories.Find(r => r is LocalCurrencyRepository);
            }
        }

        public static Model.Currency Find(string id) => Instance.AllElements.Find(c => string.Equals(c.Id, id));

        protected override async Task BeforeFastFetching()
        {
            await CurrencyRepositoryMapStorage.Instance.LoadFromDatabase();
            await LocalRepository.FetchOnline();
        }

        public async Task ClearElements()
        {
            await CurrencyRepositoryMapStorage.Instance.LocalRepository.RemoveAll();
            foreach (var repository in Repositories)
            {
                await repository.RemoveAll();
            }
        }

        public static IEnumerable<Model.Currency> CurrenciesOf<T>() where T : CurrencyRepository
        {

            var id = Instance.RepositoryOfType<T>().Id;
            var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e.ParentId == id).Select(e => e.Code);
            return Instance.AllElements.Where(c => codes.Any(x => x.Equals(c?.Code)));

        }
    }
}