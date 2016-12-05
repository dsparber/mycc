using System;
using System.Threading.Tasks;
using MyCryptos.Core.Database;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.Currency;

namespace MyCryptos.Core.Storage
{
    public class CurrencyStorage : AbstractDatabaseStorage<CurrencyRepositoryDBM, CurrencyRepository, CurrencyDBM, Currency, string>
    {
        public CurrencyStorage() : base(new CurrencyRepositoryDatabase()) { }

        protected override async Task OnFirstLaunch()
        {
            await Add(new LocalCurrencyRepository(null));
            await Add(new BittrexCurrencyRepository(null));
            await Add(new BtceCurrencyRepository(null));
            await Add(new CryptonatorCurrencyRepository(null));
            await Add(new BlockExpertsCurrencyRepository(null));
            await Add(new CryptoIdCurrencyRepository(null));
        }

        static CurrencyStorage instance { get; set; }

        public static CurrencyStorage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CurrencyStorage();
                }
                return instance;
            }
        }

        public Currency GetByString(string s)
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
    }
}