using System;
using System.Threading.Tasks;
using data.database.models;

namespace data.repositories.currency
{
	public class LocalCurrencyRepository : CurrencyRepository
	{
		public LocalCurrencyRepository(string name) : base(CurrencyRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY, name) {}

		public override async Task<bool> Fetch()
		{
			LastFetch = DateTime.Now;
			return await FetchFromDatabase();
		}

		public override async Task<bool> FetchFast()
		{
			LastFastFetch = DateTime.Now;
			return await Fetch();
		}
	}
}

