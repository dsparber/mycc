using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Storage;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Rates;
using MyCC.Core.Resources;
using MyCC.Core.Settings;

namespace MyCC.Core.Account.Storage
{
    public class AccountStorage : AbstractDatabaseStorage<AccountRepositoryDbm, AccountRepository, AccountDbm, FunctionalAccount, int>
    {
        private AccountStorage() : base(new AccountRepositoryDatabase()) { }

        protected override async Task OnFirstLaunch()
        {
            var localRepository = new LocalAccountRepository(default(int), I18N.LocalStorage);
            await Add(localRepository);
        }

        public override AccountRepository LocalRepository => Repositories.OfType<LocalAccountRepository>().FirstOrDefault();

        public static readonly AccountStorage Instance = new AccountStorage();

        public static List<Currency.Model.Currency> UsedCurrencies => Instance.AllElements.Select(a => a?.Money?.Currency).Distinct().Where(e => e != null).ToList();
        public static IEnumerable<IGrouping<Currency.Model.Currency, Models.Base.Account>> AccountsGroupedByCurrency => Instance.AllElements.GroupBy(a => a.Money.Currency);
        public static List<FunctionalAccount> AccountsWithCurrency(Currency.Model.Currency currency) => Instance.AllElements.Where(a => a.Money.Currency.Equals(currency)).ToList();

        public static List<ExchangeRate> NeededRates => UsedCurrencies.Distinct()
                                       .SelectMany(c => ApplicationSettings.AllReferenceCurrencies.Select(cref => new ExchangeRate(c, cref)))
                                       .Select(e => ExchangeRateHelper.GetRate(e) ?? e)
                                       .Where(r => r?.Rate == null)
                                       .ToList();

        public static List<ExchangeRate> NeededRatesFor(Currency.Model.Currency accountCurrency) => ApplicationSettings.AllReferenceCurrencies
                                       .Select(c => new ExchangeRate(accountCurrency, c))
                                       .Select(e => ExchangeRateHelper.GetRate(e) ?? e)
                                       .Where(r => r.Rate == null)
                                       .ToList();

        public static List<ExchangeRate> NeededRatesFor(FunctionalAccount account) => NeededRatesFor(account.Money.Currency);

        public static async Task<bool> AddRepository(OnlineAccountRepository repository)
        {
            var success = await repository.Test();
            if (success)
            {
                await Instance.Add(repository);
                await Instance.FetchOnline();
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}