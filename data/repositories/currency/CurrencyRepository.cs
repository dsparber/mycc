using data.database;
using data.database.helper;
using data.database.models;
using data.repositories.general;
using models;

namespace data.repositories.currency
{
	public abstract class CurrencyRepository : AbstractRepository<CurrencyDBM, Currency>
	{
		protected CurrencyRepository(int repositoryId) : base(repositoryId) { }

		protected override AbstractEntityRepositoryIdDatabase<CurrencyDBM, Currency> GetDatabase()
		{
			return new CurrencyDatabase();
		}
	}
}

