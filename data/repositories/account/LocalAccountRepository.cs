using System;
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
			LastFetch = DateTime.Now;
		}

		public override async Task FetchFast()
		{
			await Fetch();
			LastFastFetch = DateTime.Now;
		}

		public override string Data { get { return string.Empty; } }
	}
}

