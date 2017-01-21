using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Repositories;

namespace MyCC.Core.ExchangeRate.Repositories
{
    public abstract class AvailableRatesRepository : AbstractAvailabilityRepository<Model.ExchangeRate>
    {
        protected AvailableRatesRepository(int id) : base(id) { }

        public override Task<bool> LoadFromDatabase()
        {
            return FetchOnline();
        }

        public abstract ExchangeRateRepository ExchangeRateRepository { get; }

        public abstract Model.ExchangeRate ExchangeRateWithCurrency(Currency.Model.Currency currency);

        public abstract List<Model.ExchangeRate> ExchangeRatesWithCurrency(Currency.Model.Currency currency);
    }
}

