using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCC.Core.Rates.Repositories.Interfaces
{
    public interface IMultipleRatesRepository : IRateRepository
    {
        Task<IEnumerable<ExchangeRate>> FetchRates();
    }
}