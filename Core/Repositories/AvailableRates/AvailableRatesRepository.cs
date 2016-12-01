using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.Core;
using MyCryptos.Core.Repositories.ExchangeRates;

namespace MyCryptos.Core.Repositories.AvailableRates
{
    public abstract class AvailableRatesRepository : AbstractAvailabilityRepository<ExchangeRate>
    {
        protected AvailableRatesRepository(int repositoryTypeId, string name) : base(repositoryTypeId, name) { }

        public override Task<bool> FetchFast()
        {
            return Fetch();
        }

        public abstract ExchangeRateRepository ExchangeRateRepository { get; }

        public abstract ExchangeRate ExchangeRateWithCurrency(Models.Currency currency);

        public abstract List<ExchangeRate> ExchangeRatesWithCurrency(Models.Currency currency);
    }
}

