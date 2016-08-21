using System;
using System.Threading.Tasks;
using data.database.models;

namespace data.repositories.account
{
	public class BittrexAccountRepository : OnlineAccountRepository
	{
		public BittrexAccountRepository(string name) : base(AccountRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY, name) { }

		public override async Task Fetch()
		{
			// TODO Implement

			await WriteToDatabase();
			LastFetch = DateTime.Now;
		}
	}
}

