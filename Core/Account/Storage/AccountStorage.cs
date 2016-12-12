using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Storage;
using MyCryptos.Core.Account.Database;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Account.Repositories.Implementations;
using MyCryptos.Core.Resources;

namespace MyCryptos.Core.Account.Storage
{
    public class AccountStorage : AbstractDatabaseStorage<AccountRepositoryDbm, AccountRepository, AccountDbm, FunctionalAccount, int>
    {
        private AccountStorage() : base(new AccountRepositoryDatabase()) { }

        protected override async Task OnFirstLaunch()
        {
            var localRepository = new LocalAccountRepository(default(int), I18N.DefaultStorage);
            await Add(localRepository);
        }

        public override AccountRepository LocalRepository => Repositories.OfType<LocalAccountRepository>().FirstOrDefault();

        public static readonly AccountStorage Instance = new AccountStorage();

        public static List<FunctionalAccount> AccountsWithCurrency(Currency.Model.Currency currency)
        {
            return Instance.AllElements.Where(a => a.Money.Currency.Equals(currency)).ToList();
        }

        public static AccountRepository RepositoryOf(FunctionalAccount account)
        {
            return Instance.Repositories.FirstOrDefault(r => r.Elements.Contains(account));
        }

        public static IEnumerable<IGrouping<Currency.Model.Currency, Models.Base.Account>> AccountsGroupedByCurrency => Instance.AllElements.GroupBy(a => a.Money.Currency);

    }
}