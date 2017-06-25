using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Storage;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Currencies.Models;

namespace MyCC.Core.Account.Storage
{
    public class AccountStorage : AbstractDatabaseStorage<AccountRepositoryDbm, AccountRepository, AccountDbm, FunctionalAccount, int>
    {
        private AccountStorage() : base(new AccountRepositoryDatabase()) { }

        protected override async Task AfterLoadingRepositories()
        {
            if (LocalRepository == null)
            {
                var localRepository = new LocalAccountRepository(default(int), string.Empty);
                await Add(localRepository);
            }
        }

        public override AccountRepository LocalRepository => Repositories.OfType<LocalAccountRepository>().FirstOrDefault();

        public static readonly AccountStorage Instance = new AccountStorage();

        public static IEnumerable<string> UsedCurrencies => Instance.AllElements.Select(a => a?.Money?.Currency.Id).Distinct().Where(e => e != null).ToList();
        public static IEnumerable<IGrouping<Currency, Models.Base.Account>> AccountsGroupedByCurrency => Instance.AllElements.GroupBy(a => a?.Money?.Currency).Where(g => g.Key != null);
        public static List<FunctionalAccount> AccountsWithCurrency(Currency currency) => Instance.AllElements.Where(a => a.Money.Currency.Equals(currency)).ToList();
        public static List<FunctionalAccount> AccountsWithCurrency(string currencyId) => Instance.AllElements.Where(a => a.Money.Currency.Id.Equals(currencyId)).ToList();

        public static string CurrencyIdOf(int accountId) => GetAccount(accountId)?.Money.Currency.Id;
        public static Models.Base.Account GetAccount(int accountId) => Instance.AllElements.Find(account => account.Id == accountId);

        public static AccountRepository RepositoryOf(int accountId) => RepositoryOf(GetAccount(accountId) as FunctionalAccount);
        public static AccountRepository RepositoryOf(FunctionalAccount account) => Instance.Repositories.Find(r => r.Id == account?.ParentId);

        public static Task Update(FunctionalAccount account) => RepositoryOf(account).Update(account);

        public static IEnumerable<FunctionalAccount> EnabledAccounts => Instance.AllElements.Where(a => a.IsEnabled);

        public static IEnumerable<AddressAccountRepository> AddressRepositories => Instance.Repositories.OfType<AddressAccountRepository>();
        public static IEnumerable<BittrexAccountRepository> BittrexRepositories => Instance.Repositories.OfType<BittrexAccountRepository>();
        public static IEnumerable<PoloniexAccountRepository> PoloniexRepositories => Instance.Repositories.OfType<PoloniexAccountRepository>();
        public static IEnumerable<LocalAccount> ManuallyAddedAccounts => Instance.AllElements.OfType<LocalAccount>();

        public static bool AlreadyExists(AccountRepository repository)
            => Instance.RepositoriesOfType(repository.GetType()).Any(r => r.Data.Equals(repository.Data));
    }
}