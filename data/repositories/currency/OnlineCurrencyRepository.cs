using System;
using System.Threading.Tasks;

namespace data.repositories.currency
{
	public abstract class OnlineCurrencyRepository : CurrencyRepository
	{
		protected OnlineCurrencyRepository(int repositoryId, string name) : base(repositoryId, name) { }

		public override async Task<bool> FetchFast()
		{
			LastFastFetch = DateTime.Now;
			return await FetchFromDatabase();
		}
	}
}

