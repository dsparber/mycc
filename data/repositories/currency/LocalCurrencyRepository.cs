using System;
using System.Threading.Tasks;
using data.database.models;

namespace data.repositories.currency
{
	public class LocalCurrencyRepository : CurrencyRepository
	{
		public LocalCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY, name) {}

		public override async Task Fetch()
		{
			await FetchFromDatabase();
			LastFetch = DateTime.Now;
		}

		public override async Task FetchFast()
		{
			await Fetch();
			LastFastFetch = DateTime.Now;
		}
	}
}

