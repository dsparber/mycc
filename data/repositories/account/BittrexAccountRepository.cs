using System.Threading.Tasks;
using data.database.models;

namespace data.repositories.account
{
	public class BittrexAccountRepository : OnlineAccountRepository
	{
		public BittrexAccountRepository() : base(AccountRepositoryDBM.DB_TYPE_BITTREX_REPOSITORY) { }

		public override async Task Fetch()
		{
			// TODO Implement

			await WriteToDatabase();
		}
	}
}

