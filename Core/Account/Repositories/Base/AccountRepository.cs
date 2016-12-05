using MyCryptos.Core.Abstract.Repositories;
using MyCryptos.Core.Account.Database;
using MyCryptos.Core.Account.Models.Base;

namespace MyCryptos.Core.Account.Repositories.Base
{
    public abstract class AccountRepository : AbstractDatabaseRepository<AccountDBM, FunctionalAccount, int>
    {
        protected AccountRepository(int repositoryId, string name) : base(repositoryId, name, new AccountDatabase()) { }

        public abstract string Data { get; }

        public abstract string Description { get; }
    }
}