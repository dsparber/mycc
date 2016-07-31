using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.database;
using data.factories;
using data.repositories.exchangerate;
using models;

namespace data.storage
{
	public class ExchangeRateStorage
	{
		public List<ExchangeRateRepository> Repositories;

		ExchangeRateStorage()
		{
			var repos = new ExchangeRateRepositoryDatabase().GetAll();
			repos.RunSynchronously();

			Repositories = repos.Result.Select(r => ExchangeRateRepositoryFactory.create(r)).ToList();
		}

		public List<ExchangeRate> ExchangeRates
		{
			get
			{
				return Repositories.SelectMany(r => r.ExchangeRates).ToList();
			}
		}

		ExchangeRateStorage instance { get; set; }

		public ExchangeRateStorage Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ExchangeRateStorage();
				}
				return instance;
			}
		}

		public async Task FetchAvailableRates()
		{
			await Task.WhenAll(Repositories.Select(x => x.FetchAvailableRates()));
		}

		public async Task FetchAvailableRatesFast()
		{
			await Task.WhenAll(Repositories.Select(x => x.FetchAvailableRatesFast()));
		}

		public async Task FetchExchangeRate(ExchangeRate exchangeRate)
		{
			await Task.WhenAll(Repositories.Select(x => x.FetchExchangeRate(exchangeRate)));
		}

		public async Task FetchExchangeRateFast(ExchangeRate exchangeRate)
		{
			await Task.WhenAll(Repositories.Select(x => x.FetchExchangeRateFast(exchangeRate)));
		}
	}
}

