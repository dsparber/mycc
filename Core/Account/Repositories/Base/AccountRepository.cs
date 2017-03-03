using MyCC.Core.Abstract.Repositories;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Models.Base;

namespace MyCC.Core.Account.Repositories.Base
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

        public override bool Equals(object obj)
        {
            var repository = obj as AccountRepository;
            return repository != null && Id != default(int) && repository.Id == Id;
        }

        public override int GetHashCode() => 1;
    }
}