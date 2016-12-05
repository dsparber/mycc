using MyCryptos.Core.Abstract.Repositories;
using MyCryptos.Core.Currency.Database;

namespace MyCryptos.Core.Currency.Repositories
{
	public abstract class CurrencyRepository : AbstractDatabaseRepository<CurrencyDbm, Model.Currency, string>
	{
		protected CurrencyRepository(int id) : base(id, new CurrencyDatabase()) { }
	}
}

