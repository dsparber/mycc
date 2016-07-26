using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using models;

namespace data.repositories.account
{
	public abstract class AccountRepository
	{
		public List<Account> Accounts;
		public string RepositoryName;

		public AccountRepository()
		{
			Accounts = new List<Account>();
		}

		public abstract Task Fetch();

		public abstract Task FetchFast();

		protected async Task FetchFromDatabase()
		{
			// TODO Read Database
			throw new NotImplementedException();
		}

		protected async Task WriteToDatabase()
		{
			// TODO Write to Database
			throw new NotImplementedException();
		}
	}
}

