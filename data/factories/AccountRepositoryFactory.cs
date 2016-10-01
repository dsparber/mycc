using data.database.models;
using data.repositories.account;
using Newtonsoft.Json;

namespace data.factories
{
	public static class AccountRepositoryFactory
	{
		public static AccountRepository create(AccountRepositoryDBM repositoryDBM)
		{
			switch (repositoryDBM.Type)
			{
				case AccountRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY: return new LocalAccountRepository(repositoryDBM.Name) { DatabaseId = repositoryDBM.Id};
				case AccountRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY: return new BittrexAccountRepository(repositoryDBM.Name, repositoryDBM.Data) { DatabaseId = repositoryDBM.Id };
				default: return null;
			}

		}
	}
}

