using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Abstract.Repositories;
using MyCC.Core.Currency.Database;
using MyCC.Core.Currency.Storage;

namespace MyCC.Core.Currency.Repositories
{
    public abstract class CurrencyRepository : AbstractDatabaseRepository<CurrencyDbm, Model.Currency, string>
    {
        protected CurrencyRepository(int id) : base(id, new CurrencyDatabase()) { }

        public List<Model.Currency> Currencies
        {
            get
            {
                var codes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => e?.ParentId == Id).Select(e => e.Code);
                return CurrencyStorage.Instance.AllElements.Where(c => codes.Any(x => string.Equals(x, c?.Code))).ToList();
            }
        }
    }
}

