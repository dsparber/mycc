using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Repositories;
using MyCryptos.Core.ExchangeRate.Database;

namespace MyCryptos.Core.ExchangeRate.Repositories
{
    public abstract class ExchangeRateRepository : AbstractDatabaseRepository<ExchangeRateDBM, Model.ExchangeRate, string>
    {
        protected ExchangeRateRepository(int repositoryId, string name) : base(repositoryId, name, new ExchangeRateDatabase()) { }

        public abstract Task<bool> FetchNew();
    }
}