using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Storage;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Models.Implementations;
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

        public static IEnumerable<Currency.Model.Currency> UsedCurrencies => Instance.AllElements.Select(a => a?.Money?.Currency).Distinct().Where(e => e != null).ToList();
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

        public static AccountRepository RepositoryOf(FunctionalAccount account) => Instance.Repositories.Find(r => r.Id == account.ParentId);

        public static Task Update(FunctionalAccount account) => RepositoryOf(account).Update(account);

        public static IEnumerable<FunctionalAccount> EnabledAccounts => Instance.AllElements.Where(a => a.IsEnabled);

        public static IEnumerable<AddressAccountRepository> AddressRepositories => Instance.Repositories.OfType<AddressAccountRepository>();
        public static IEnumerable<BittrexAccountRepository> BittrexRepositories => Instance.Repositories.OfType<BittrexAccountRepository>();
        public static IEnumerable<LocalAccount> ManuallyAddedAccounts => Instance.AllElements.OfType<LocalAccount>();

        public static int CurrenciesForGraph => AccountsGroupedByCurrency
            .Select(e => e.Select(a =>
            {
                var rate = new ExchangeRate(e.Key, ApplicationSettings.BaseCurrency);
                rate = ExchangeRateHelper.GetRate(rate) ?? rate;

                return a.IsEnabled ? a.Money.Amount * rate.Rate ?? 0 : 0;
            }).Sum()).Count(v => v > 0);
    }
}