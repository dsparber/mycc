using System.Threading.Tasks;
using data.repositories.exchangerate;
using data.repositories.general;
using MyCryptos.models;

namespace data.repositories.availablerates
{
	public abstract class AvailableRatesRepository : AbstractAvailabilityRepository<ExchangeRate>
	{
		protected AvailableRatesRepository(int repositoryId, string name) : base(repositoryId, name) { }

		public override Task<bool> FetchFast()
		{
			return Fetch();
		}

		public abstract ExchangeRateRepository ExchangeRateRepository { get; }

		public abstract ExchangeRate ExchangeRateWithCurrency(Currency currency);
	}
}

