using MyCryptos.Core.Abstract.Repositories;
using MyCryptos.Core.Currency.Database;

namespace MyCryptos.Core.Currency.Repositories
{
    public abstract class CurrencyRepository : AbstractDatabaseRepository<CurrencyDBM, Model.Currency, string>
    {
        protected CurrencyRepository(int repositoryId, string name) : base(repositoryId, name, new CurrencyDatabase()) { }
    }
}

