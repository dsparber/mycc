using System.Collections.Generic;
using System.Threading.Tasks;
using data.database.models;
using data.storage;
using MyCryptos.models;

namespace data.repositories.currency
{
	public class BtceCurrencyRepository : OnlineCurrencyRepository
	{
		public BtceCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_BTCE_REPOSITORY, name) { }

		protected async override Task<IEnumerable<Currency>> GetCurrencies()
		{
			var all = await CurrencyStorage.Instance.AllElements();
			var currentElements = new List<Currency>();
			currentElements.Add(all.Find(e => e.Equals(Currency.EUR)));
			currentElements.Add(all.Find(e => e.Equals(Currency.USD)));
			return currentElements;
		}
	}
}

