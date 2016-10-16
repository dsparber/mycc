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
				case CurrencyRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY: return new LocalCurrencyRepository(repositoryDBM.Name) { Id = repositoryDBM.Id };
				case CurrencyRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY: return new BittrexCurrencyRepository(repositoryDBM.Name) { Id = repositoryDBM.Id };
				case CurrencyRepositoryDBM.DB_TYPE_BTCE_REPOSITORY: return new BtceCurrencyRepository(repositoryDBM.Name) { Id = repositoryDBM.Id };
				case CurrencyRepositoryDBM.DB_TYPE_CRYPTONATOR_REPOSITORY: return new CryptonatorCurrencyRepository(repositoryDBM.Name) { Id = repositoryDBM.Id };
				case CurrencyRepositoryDBM.DB_TYPE_BLOCK_EXPERTS_REPOSITORY: return new BlockExpertsCurrencyRepository(repositoryDBM.Name) { Id = repositoryDBM.Id };
				default: return null;
			}

		}
	}
}

