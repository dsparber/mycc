using System.Threading.Tasks;

namespace data.repositories.account
{
	public class LocalAccountRepository : AccountRepository
	{
		public LocalAccountRepository(int reposioteyId) : base(reposioteyId) { }

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

