using System;
using System.Threading.Tasks;

namespace data.repositories.account
{
	public abstract class OnlineAccountRepository : AccountRepository
	{
		public override async Task FetchFast()
		{
			await FetchFromDatabase();
		}
	}
}

