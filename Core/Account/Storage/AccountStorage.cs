using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Storage;
using MyCryptos.Core.Account.Database;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Account.Repositories.Implementations;
using MyCryptos.Core.ExchangeRate.Helpers;
using MyCryptos.Core.Resources;
using MyCryptos.Core.settings;

namespace MyCryptos.Core.Account.Storage
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

		public static List<ExchangeRate.Model.ExchangeRate> NeededRates => UsedCurrencies.Distinct()
									   .SelectMany(c => ApplicationSettings.ReferenceCurrencies.Select(cref => new ExchangeRate.Model.ExchangeRate(c, cref)))
									   .Select(e => ExchangeRateHelper.GetRate(e) ?? e)
									   .Where(r => r?.Rate == null)
									   .ToList();

		public static List<ExchangeRate.Model.ExchangeRate> NeededRatesFor(Currency.Model.Currency accountCurrency) => ApplicationSettings.ReferenceCurrencies
									   .Select(c => new ExchangeRate.Model.ExchangeRate(accountCurrency, c))
									   .Select(e => ExchangeRateHelper.GetRate(e) ?? e)
									   .Where(r => r?.Rate == null)
									   .ToList();

		public static List<ExchangeRate.Model.ExchangeRate> NeededRatesFor(FunctionalAccount account) => NeededRatesFor(account.Money.Currency);

		public static AccountRepository RepositoryOf(FunctionalAccount account) => Instance.Repositories.FirstOrDefault(r => r.Elements.Contains(account));



	}
}