using data.database;
using data.database.helper;
using data.database.models;
using data.repositories.general;
using models;

namespace data.repositories.account
{
	public abstract class AccountRepository : AbstractDatabaseRepository<AccountDBM, Account>
	{
		protected AccountRepository(int repositoryId, string name) : base(repositoryId, name) { }

		protected override AbstractEntityRepositoryIdDatabase<AccountDBM, Account> GetDatabase()
		{
			return new AccountDatabase();
		}

		public abstract string Data { get; }

		public abstract string Description { get; }
	}
}