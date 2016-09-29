using System;
using System.Diagnostics;
using System.Threading.Tasks;
using data.database.models;
using models;

namespace data.repositories.currency
{
	public class BtceCurrencyRepository : OnlineCurrencyRepository
	{
		public BtceCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_BTCE_REPOSITORY, name) { }

		public override async Task<bool> Fetch()
		{
			try
			{
				Elements.Add(Currency.EUR);
				Elements.Add(Currency.USD);
				await WriteToDatabase();
				LastFetch = DateTime.Now;
				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine(string.Format("Error Message:\n{0}\nData:\n{1}\nStack trace:\n{2}", e.Message, e.Data, e.StackTrace));
				return false;
			}
		}
	}
}

