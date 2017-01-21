using System.Collections.Generic;
using System.Threading.Tasks;
using MyCC.Core.Currency.Database;

namespace MyCC.Core.Currency.Repositories
{
    public class BtceCurrencyRepository : OnlineCurrencyRepository
    {
        public BtceCurrencyRepository(int id) : base(id) { }
        public override int RepositoryTypeId => CurrencyRepositoryDbm.DB_TYPE_BTCE_REPOSITORY;

        protected override Task<IEnumerable<Model.Currency>> GetCurrencies()
        {
            return Task.Factory.StartNew<IEnumerable<Model.Currency>>(() =>
               {
                   var currentElements = new List<Model.Currency>();
                   currentElements.Add(Model.Currency.Eur);
                   currentElements.Add(Model.Currency.Usd);
                   return currentElements;
               });
        }
    }
}

