using System;
using System.Threading.Tasks;
using data.database.models;
using models;

namespace data.repositories.currency
{
	public class BtceCurrencyRepository : OnlineCurrencyRepository
	{
		public BtceCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_BTCE_REPOSITORY, name) { }

		public override async Task Fetch()
		{
			Elements.Add(Currency.BTC);
			Elements.Add(Currency.EUR);
			Elements.Add(Currency.USD);
			await WriteToDatabase();
			LastFetch = DateTime.Now;
		}
	}
}

