using System.Threading.Tasks;

namespace data.repositories.account
{
	public class BittrexAccountRepository : OnlineAccountRepository
	{
		public BittrexAccountRepository(int reposioteyId) : base(reposioteyId) { }

		public override async Task Fetch()
		{
			// TODO Implement

			await WriteToDatabase();
		}
	}
}

