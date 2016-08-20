using System.Threading.Tasks;
using data.database.models;

namespace data.repositories.account
{
	public class LocalAccountRepository : AccountRepository
	{
		public LocalAccountRepository(string name) : base(AccountRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY, name) { }

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

