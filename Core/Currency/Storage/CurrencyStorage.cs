using System;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Storage;
using MyCC.Core.Currency.Database;
using MyCC.Core.Currency.Repositories;

namespace MyCC.Core.Currency.Storage
{
    public class CurrencyStorage : AbstractDatabaseStorage<CurrencyRepositoryDbm, CurrencyRepository, CurrencyDbm, Model.Currency, string>
    {
        private CurrencyStorage() : base(new CurrencyRepositoryDatabase()) { }

        protected override async Task OnFirstLaunch()
        {
            await Add(new LocalCurrencyRepository(default(int)));
            await Add(new BittrexCurrencyRepository(default(int)));
            await Add(new BtceCurrencyRepository(default(int)));
            await Add(new CryptonatorCurrencyRepository(default(int)));
            await Add(new BlockExpertsCurrencyRepository(default(int)));
            await Add(new CryptoIdCurrencyRepository(default(int)));
        }

        static CurrencyStorage instance { get; set; }

        public static CurrencyStorage Instance => instance ?? (instance = new CurrencyStorage());

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

        protected override async Task beforeFastFetching()
        {
            await LocalRepository.FetchOnline();
        }

        public static Model.Currency Find(string code)
        {
            return Instance.AllElements.Find(c => Equals(code, c?.Code));
        }
    }
}