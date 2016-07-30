using System.Collections.Generic;
using data.repositories.account;
using data.database;
using data.factories;
using models;
using System.Threading.Tasks;
using System.Linq;

namespace data.storage
{
	public class AccountStorage
	{
		public List<AccountRepository> Repositories;

		AccountStorage()
		{
			Repositories = new List<AccountRepository>();

			var db = new AccountRepositoryDatabase();

			foreach (var r in db.GetRepositories().Result)
			{
				AccountRepository accountRepository = AccountRepositoryFactory.create(r);
				Repositories.Add(accountRepository);
			}
		}

		public List<Account> Accounts
		{
			get
			{
				return Repositories.SelectMany(r => r.Accounts).ToList();
			}
		}

		AccountStorage instance { get; set; }

		public AccountStorage Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new AccountStorage();
				}
				return instance;
			}
		}

		public async Task Fetch()
		{
			await Task.WhenAll(Repositories.Select(x => x.Fetch()));
		}

		public async Task FetchFast()
		{
			await Task.WhenAll(Repositories.Select(x => x.FetchFast()));
		}
	}
}

