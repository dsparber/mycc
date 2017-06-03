using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Rates.Models;

namespace MyCC.Core.Rates.Repositories
{
    public interface IRateSource
    {
        RateSourceId Id { get; }

        string Name { get; }

        bool IsAvailable(RateDescriptor rateDescriptor);

        Task<IEnumerable<ExchangeRate>> FetchRates(IEnumerable<RateDescriptor> rateDescriptors);

        RateSourceType Type { get; }
    }
}