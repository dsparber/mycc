using System.Collections.Generic;
using System.Threading.Tasks;
using MyCryptos.Core.Database.Models;

namespace MyCryptos.Core.Repositories.Currency
{
    public class BtceCurrencyRepository : OnlineCurrencyRepository
    {
        public BtceCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_BTCE_REPOSITORY, name) { }

        protected override Task<IEnumerable<Models.Currency>> GetCurrencies()
        {
            return Task.Factory.StartNew<IEnumerable<Models.Currency>>(() =>
               {
                   var currentElements = new List<Models.Currency>();
                   currentElements.Add(Models.Currency.EUR);
                   currentElements.Add(Models.Currency.USD);
                   return currentElements;
               });
        }
    }
}

