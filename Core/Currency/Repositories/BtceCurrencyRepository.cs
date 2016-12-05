using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Currency.Database;

namespace MyCryptos.Core.Currency.Repositories
{
    public class BtceCurrencyRepository : OnlineCurrencyRepository
    {
        public BtceCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_BTCE_REPOSITORY, name) { }

        protected override Task<IEnumerable<Model.Currency>> GetCurrencies()
        {
            return Task.Factory.StartNew<IEnumerable<Model.Currency>>(() =>
               {
                   var currentElements = new List<Model.Currency>();
                   currentElements.Add(Model.Currency.EUR);
                   currentElements.Add(Model.Currency.USD);
                   return currentElements;
               });
        }
    }
}

