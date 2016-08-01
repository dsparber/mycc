using data.database.models;
using data.repositories.exchangerate;

namespace data.factories
{
	public static class ExchangeRateRepositoryFactory
	{
		public static ExchangeRateRepository create(ExchangeRateRepositoryDBM repositoryDBM)
		{
			switch (repositoryDBM.Type) 
			{
				case ExchangeRateRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY: return new LocalExchangeRateRepository(ExchangeRateRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY);
				default: return null;
			}
				
		}
	}
}

