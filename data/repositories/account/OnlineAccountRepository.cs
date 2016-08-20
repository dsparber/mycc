using System.Threading.Tasks;

namespace data.repositories.account
{
	public abstract class OnlineAccountRepository : AccountRepository
	{
		protected OnlineAccountRepository(int reposioteyId, string name) : base(reposioteyId, name) { }

		public override async Task FetchFast()
		{
			await FetchFromDatabase();
		}
	}
}

