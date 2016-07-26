using System;
using System.Threading.Tasks;

namespace data.repositories.account
{
	public class LocalAccountRepository : AccountRepository
	{
		public override async Task Fetch()
		{
			await FetchFromDatabase();
		}

		public override async Task FetchFast()
		{
			await Fetch();
		}
	}
}

