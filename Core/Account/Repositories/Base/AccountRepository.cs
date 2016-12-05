using MyCryptos.Core.Account.Models;
using MyCryptos.Core.Database;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Repositories.Core;

namespace MyCryptos.Core.Repositories.Account
{
	public abstract class AccountRepository : AbstractDatabaseRepository<AccountDBM, FunctionalAccount, int>
	{
		protected AccountRepository(int repositoryId, string name) : base(repositoryId, name, new AccountDatabase()) { }

		public abstract string Data { get; }

		public abstract string Description { get; }
	}
}