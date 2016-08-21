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
				case ExchangeRateRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY: return new LocalExchangeRateRepository(repositoryDBM.Name);
				case ExchangeRateRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY: return new BittrexExchangeRateRepository(repositoryDBM.Name);
				case ExchangeRateRepositoryDBM.DB_TYPE_BTCE_REPOSITORY: return new BtceExchangeRateRepository(repositoryDBM.Name);
				default: return null;
			}
				
		}
	}
}

