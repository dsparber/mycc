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
			var repos = new AccountRepositoryDatabase().GetRepositories();
			Repositories = repos.Result.Select(r => AccountRepositoryFactory.create(r)).ToList();
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