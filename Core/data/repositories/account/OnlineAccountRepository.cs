using System;
using System.Threading.Tasks;

namespace data.repositories.account
{
	public abstract class OnlineAccountRepository : AccountRepository
	{
		protected OnlineAccountRepository(int reposioteyId, string name) : base(reposioteyId, name) { }

		public override async Task<bool> FetchFast()
		{
			LastFastFetch = DateTime.Now;
			return await FetchFromDatabase();
		}

		public abstract Task<bool> Test();
	}
}

