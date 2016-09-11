using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using data.repositories.exchangerate;
using data.storage;
using models;

namespace data.repositories.availablerates
{
	public class LocalAvailableRatesRepository : AvailableRatesRepository
	{
		List<ExchangeRate> Elements;

		public LocalAvailableRatesRepository(string name) : base(AvailableRatesRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY, name)
		{
			Elements = new List<ExchangeRate>();
		}

		public override bool IsAvailable(ExchangeRate element)
		{
			return Elements.Contains(element);
		}

		public override async Task Fetch()
		{
			var localExchangeRateRepository = (await ExchangeRateStorage.Instance.Repositories()).Find(r => r is LocalExchangeRateRepository);
			Elements = localExchangeRateRepository.Elements;
		}

		public override async Task FetchFast()
		{
			var localExchangeRateRepository = (await ExchangeRateStorage.Instance.Repositories()).Find(r => r is LocalExchangeRateRepository);
			Elements = localExchangeRateRepository.Elements;
		}

		public async override Task<ExchangeRateRepository> ExchangeRateRepository()
		{
			return (await ExchangeRateStorage.Instance.Repositories()).Find(r => r is LocalExchangeRateRepository);
		}

		public override ExchangeRate ExchangeRateWithCurrency(Currency currency)
		{
			foreach (var e in Elements)
			{
				if (e.Contains(currency))
				{
					return e;
				}
			}
			return null;
		}
	}
}