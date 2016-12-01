using data.database.models;
using data.repositories.availablerates;

namespace data.factories
{
	public static class AvailableRatesFactory
	{
		public static AvailableRatesRepository create(AvailableRatesRepositoryDBM repositoryDBM)
		{
			switch (repositoryDBM.Type)
			{
				case AvailableRatesRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY: return new LocalAvailableRatesRepository(repositoryDBM.Name);
				case AvailableRatesRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY: return new BittrexAvailableRatesRepository(repositoryDBM.Name);
				case AvailableRatesRepositoryDBM.DB_TYPE_BTCE_REPOSITORY: return new BtceAvailableRatesRepository(repositoryDBM.Name);
				case AvailableRatesRepositoryDBM.DB_TYPE_CRYPTONATOR_REPOSITORY: return new CryptonatorAvailableRatesRepository(repositoryDBM.Name);
				default: return null;
			}

		}
	}
}

