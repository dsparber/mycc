using data.database.models;
using data.repositories.currency;

namespace data.factories
{
	public static class CurrencyRepositoryFactory
	{
		public static CurrencyRepository create(CurrencyRepositoryDBM repositoryDBM)
		{
			switch (repositoryDBM.Type) 
			{
				case CurrencyRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY: return new LocalCurrencyRepository();
				default: return null;
			}
				
		}
	}
}

