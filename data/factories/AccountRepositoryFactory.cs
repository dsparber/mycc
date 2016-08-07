﻿using data.database.models;
using data.repositories.account;

namespace data.factories
{
	public static class AccountRepositoryFactory
	{
		public static AccountRepository create(AccountRepositoryDBM repositoryDBM)
		{
			switch (repositoryDBM.Type) 
			{
				case AccountRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY: return new LocalAccountRepository(repositoryDBM.Id);
				case AccountRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY: return new BittrexAccountRepository(repositoryDBM.Id);
				default: return null;
			}
				
		}
	}
}

