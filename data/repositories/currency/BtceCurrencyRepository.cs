using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using MyCryptos.models;

namespace data.repositories.currency
{
	public class BtceCurrencyRepository : OnlineCurrencyRepository
	{
		public BtceCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_BTCE_REPOSITORY, name) { }

		protected override Task<IEnumerable<Currency>> GetCurrencies()
		{
			return Task.Factory.StartNew<IEnumerable<Currency>>(() =>
		   	{
				   var currentElements = new List<Currency>();
				   currentElements.Add(Currency.EUR);
				   currentElements.Add(Currency.USD);
				   return currentElements;
			   });
		}
	}
}

