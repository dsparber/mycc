using MyCryptos.Core.Database;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Repositories.Core;

namespace MyCryptos.Core.Repositories.Currency
{
    public abstract class CurrencyRepository : AbstractDatabaseRepository<CurrencyDBM, Models.Currency, string>
    {
        protected CurrencyRepository(int repositoryId, string name) : base(repositoryId, name, new CurrencyDatabase()) { }
    }
}

