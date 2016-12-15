using System;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Database;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Resources;

namespace MyCryptos.Core.Account.Repositories.Implementations
{
	public class LocalAccountRepository : AccountRepository
	{
		public LocalAccountRepository(int id, string name) : base(id, name) { }
		public override int RepositoryTypeId => AccountRepositoryDbm.DB_TYPE_LOCAL_REPOSITORY;

		public override async Task<bool> FetchOnline()
		{
			LastFetch = DateTime.Now;
			return await FetchFromDatabase();
		}

		public override async Task<bool> LoadFromDatabase()
		{
			LastFastFetch = DateTime.Now;
			return await FetchOnline();
		}

		public override string Data { get { return string.Empty; } }

		public override string Description { get { return I18N.ManuallyAdded; } }

		public override string Info => Name;
	}
}

