using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Account.Models;
using MyCryptos.Core.Database;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Repositories.Account;
using MyCryptos.Core.Resources;

namespace MyCryptos.Core.Storage
{
	public class AccountStorage : AbstractDatabaseStorage<AccountRepositoryDBM, AccountRepository, AccountDBM, FunctionalAccount, int>
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
				return Repositories.OfType<LocalAccountRepository>().FirstOrDefault();
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