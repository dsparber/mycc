using System.Threading.Tasks;
using MyCryptos.Core.Database;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.Core;

namespace MyCryptos.Core.Repositories.ExchangeRates
{
    public abstract class ExchangeRateRepository : AbstractDatabaseRepository<ExchangeRateDBM, ExchangeRate, string>
    {
        protected ExchangeRateRepository(int repositoryId, string name) : base(repositoryId, name, new ExchangeRateDatabase()) { }

        public abstract Task<bool> FetchNew();
    }
}