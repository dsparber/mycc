using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCC.Core.Rates.Repositories.Interfaces
{
	public interface IRateRepository
	{
		int TypeId { get; }

		string Name { get; }

		bool IsAvailable(ExchangeRate rate);

		List<ExchangeRate> Rates { get; }

		RateRepositoryType RatesType { get; }
  	}
}