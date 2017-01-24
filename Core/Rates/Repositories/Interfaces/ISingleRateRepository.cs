using System.Threading.Tasks;

namespace MyCC.Core.Rates.Repositories.Interfaces
{
    public interface ISingleRateRepository : IRateRepository
    {
        Task<ExchangeRate> FetchRate(ExchangeRate rate);
    }
}