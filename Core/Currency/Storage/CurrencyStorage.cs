using System;
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

        protected override async Task BeforeFastFetching()
        {
            await LocalRepository.FetchOnline();
        }

        public static Model.Currency Find(string code)
        {
            return Instance.AllElements.Find(c => Equals(code, c?.Code));
        }
    }
}