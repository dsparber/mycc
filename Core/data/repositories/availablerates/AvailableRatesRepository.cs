using System.Collections.Generic;
using System.Threading.Tasks;
using data.repositories.exchangerate;
using data.repositories.general;
using MyCryptos.models;

namespace data.repositories.availablerates
{
	public abstract class AvailableRatesRepository : AbstractAvailabilityRepository<ExchangeRate>
	{
		protected AvailableRatesRepository(int repositoryTypeId, string name) : base(repositoryTypeId, name) { }

		public override Task<bool> FetchFast()
		{
			return Fetch();
		}

		public abstract ExchangeRateRepository ExchangeRateRepository { get; }

		public abstract ExchangeRate ExchangeRateWithCurrency(Currency currency);

		public abstract List<ExchangeRate> ExchangeRatesWithCurrency(Currency currency);
	}
}

