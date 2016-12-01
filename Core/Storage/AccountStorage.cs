using System.Threading.Tasks;
using MyCryptos.Core.Database;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Models;
using MyCryptos.Core.Repositories.Account;
using MyCryptos.Core.Resources;

namespace MyCryptos.Core.Storage
{
    public class AccountStorage : AbstractDatabaseStorage<AccountRepositoryDBM, AccountRepository, AccountDBM, Account, int>
    {
        public AccountStorage() : base(new AccountRepositoryDatabase()) { }

        protected override async Task OnFirstLaunch()
        {
            var localRepository = new LocalAccountRepository(I18N.DefaultStorage);
            await Add(localRepository);
        }

        public override AccountRepository LocalRepository
        {
            get
            {
                return Repositories.Find(r => r is LocalAccountRepository);
            }
        }

        static AccountStorage instance { get; set; }

        public static AccountStorage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountStorage();
                }
                return instance;
            }
        }
    }
}