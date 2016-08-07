using System.Threading.Tasks;

namespace data.repositories.account
{
	public abstract class OnlineAccountRepository : AccountRepository
	{
		protected OnlineAccountRepository(int reposioteyId) : base(reposioteyId) { }

		public override async Task FetchFast()
		{
			await FetchFromDatabase();
		}
	}
}

