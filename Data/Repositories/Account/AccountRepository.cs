﻿using System.Collections.Generic;
using System.Threading.Tasks;
using data.database;
using models;

namespace data.repositories.account
{
	public abstract class AccountRepository
	{
		public List<Account> Accounts;
		public string RepositoryName;
		public int RepositoryId;

		protected AccountRepository()
		{
			Accounts = new List<Account>();
		}

		public abstract Task Fetch();

		public abstract Task FetchFast();

		protected async Task FetchFromDatabase()
		{
			var db = new AccountDatabase();
			Accounts = new List<Account>(await db.GetAll(RepositoryId));
		}

		protected async Task WriteToDatabase()
		{
			var db = new AccountDatabase();
			await db.Write(Accounts, RepositoryId);
		}
	}
}

