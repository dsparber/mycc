using System;
using System.Threading.Tasks;
using MyCC.Core.Account.Database;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Resources;

namespace MyCC.Core.Account.Repositories.Implementations
{
    public class LocalAccountRepository : AccountRepository
    {
        public LocalAccountRepository(int id, string name) : base(id, name) { }
        public override int RepositoryTypeId => AccountRepositoryDbm.DbTypeLocalRepository;

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

        public override string Data => string.Empty;

        public override string Description => I18N.ManuallyAdded;
    }
}

