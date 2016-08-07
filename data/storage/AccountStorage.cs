using System.Collections.Generic;
using data.repositories.account;
using data.database;
using data.factories;
using models;
using System.Threading.Tasks;
using data.database.models;
using data.database.helper;
using System;

namespace data.storage
{
	public class AccountStorage : AbstractStorage<AccountRepositoryDBM, AccountRepository, AccountDBM, Account>
	{
		protected override AccountRepository Resolve(AccountRepositoryDBM obj)
		{
			return AccountRepositoryFactory.create(obj);
		}

		protected override AbstractStorage<AccountRepositoryDBM, AccountRepository, AccountDBM, Account> CreateInstance()
		{
			return new AccountStorage();
		}

		public override AbstractRepositoryDatabase<AccountRepositoryDBM> GetDatabase()
		{
			return new AccountRepositoryDatabase();
		}
	}
}