using data.database;
using data.database.models;
using data.repositories.general;
using MyCryptos.models;

namespace data.repositories.currency
{
	public abstract class CurrencyRepository : AbstractDatabaseRepository<CurrencyDBM, Currency>
	{
		protected CurrencyRepository(int repositoryId, string name) : base(repositoryId, name, new CurrencyDatabase()) { }
	}
}

