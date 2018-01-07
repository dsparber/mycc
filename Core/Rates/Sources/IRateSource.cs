using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Rates.Models;
using MyCC.Core.Rates.Repositories.Utils;

namespace MyCC.Core.Rates.Repositories
{
    internal interface IRateSource
    {
        int Id { get; }

        string Name { get; }

        bool IsAvailable(RateDescriptor rateDescriptor);

        Task<IEnumerable<ExchangeRate>> FetchRates(IEnumerable<RateDescriptor> rateDescriptors);

        RateSourceType Type { get; }
    }
}