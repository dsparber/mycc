using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCC.Core.Rates.Repositories.Interfaces
{
    public interface IRateRepository
    {
        int TypeId { get; }

        Task FetchAvailableRates();

        bool IsAvailable(ExchangeRate rate);

        List<ExchangeRate> Rates { get; }

        RateRepositoryType RatesType { get; }

        Task UpdateRates();
    }
}