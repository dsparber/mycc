using MyCryptos.Core.Abstract.Repositories;
using MyCryptos.Core.Account.Database;
using MyCryptos.Core.Account.Models.Base;

namespace MyCryptos.Core.Account.Repositories.Base
{
	public abstract class AccountRepository : AbstractDatabaseRepository<AccountDbm, FunctionalAccount, int>
	{
		protected AccountRepository(int id, string name) : base(id, new AccountDatabase())
		{
			Name = name;
		}

		public string Name;

		public abstract string Data { get; }

		public abstract string Description { get; }
	}
}