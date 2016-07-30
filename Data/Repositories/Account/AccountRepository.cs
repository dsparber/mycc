using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using data.database;
using models;

namespace data.repositories.account
{
	public abstract class AccountRepository
	{
		public IEnumerable<Account> Accounts;
		public string RepositoryName;
		public int RepositoryId;

		public AccountRepository()
		{
			Accounts = new List<Account>();
		}

		public abstract Task Fetch();

		public abstract Task FetchFast();

		protected async Task FetchFromDatabase()
		{
			var db = new AccountDatabase();
			Accounts = await db.GetAccounts(RepositoryId);
		}

		protected async Task WriteToDatabase()
		{
			var db = new AccountDatabase();
			await db.WriteAccounts(RepositoryId, Accounts);
		}
	}
}

