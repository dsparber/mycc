using System.Threading.Tasks;
using MyCC.Core.Abstract.Repositories;
using MyCC.Core.ExchangeRate.Database;

namespace MyCC.Core.ExchangeRate.Repositories
{
    public abstract class ExchangeRateRepository : AbstractDatabaseRepository<ExchangeRateDbm, Model.ExchangeRate, string>
    {
        protected ExchangeRateRepository(int id) : base(id, new ExchangeRateDatabase()) { }

        public abstract Task<bool> FetchNew();
    }
}