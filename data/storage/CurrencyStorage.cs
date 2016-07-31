using models;
using data.database;
using data.factories;
using data.repositories.currency;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data.storage
{
	public class CurrencyStorage
	{
		public List<CurrencyRepository> Repositories;

		CurrencyStorage()
		{
			var repos = new CurrencyRepositoryDatabase().GetRepositories();
			Repositories = repos.Result.Select(r => CurrencyRepositoryFactory.create(r)).ToList();
		}

		public List<Currency> Currencies
		{
			get
			{
				return Repositories.SelectMany(r => r.Currencies).ToList();
			}
		}

		CurrencyStorage instance { get; set; }

		public CurrencyStorage Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new CurrencyStorage();
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