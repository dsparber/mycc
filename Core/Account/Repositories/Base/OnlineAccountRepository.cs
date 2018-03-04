using System.Threading.Tasks;

namespace MyCC.Core.Account.Repositories.Base
{
    public abstract class OnlineAccountRepository : AccountRepository
    {
        protected OnlineAccountRepository(int id, string name) : base(id, name) { }

        public override async Task<bool> LoadFromDatabase()
        {
            return await FetchFromDatabase();
        }

        public abstract Task<bool> Test();
    }
}

