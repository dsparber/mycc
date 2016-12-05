using System;
using System.Threading.Tasks;

namespace MyCryptos.Core.Repositories.Account
{
    public abstract class OnlineAccountRepository : AccountRepository
    {
        protected OnlineAccountRepository(int reposioteyId, string name) : base(reposioteyId, name) { }

        public override async Task<bool> LoadFromDatabase()
        {
            LastFastFetch = DateTime.Now;
            return await FetchFromDatabase();
        }

        public abstract Task<bool> Test();
    }
}

