using System;
using System.Threading.Tasks;
using data.database.models;
using MyCryptos.resources;

namespace data.repositories.account
{
	public class LocalAccountRepository : AccountRepository
	{
		public LocalAccountRepository(string name) : base(AccountRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY, name) { }

		public override async Task<bool> Fetch()
		{
			LastFetch = DateTime.Now;
			return await FetchFromDatabase();
		}

		public override async Task<bool> FetchFast()
		{
			LastFastFetch = DateTime.Now;
			return await Fetch();
		}

		public override string Data { get { return string.Empty; } }

		public override string Description { get { return I18N.LocalStorage; } }
	}
}

