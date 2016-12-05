using System.Linq;
using System.Threading.Tasks;
using MyCryptos.Core.Abstract.Storage;
using MyCryptos.Core.Account.Database;
using MyCryptos.Core.Account.Models.Base;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Account.Repositories.Implementations;
using MyCryptos.Core.Resources;

namespace MyCryptos.Core.Account.Storage
{
	public class AccountStorage : AbstractDatabaseStorage<AccountRepositoryDbm, AccountRepository, AccountDbm, FunctionalAccount, int>
	{
		public AccountStorage() : base(new AccountRepositoryDatabase()) { }

		protected override async Task OnFirstLaunch()
		{
			var localRepository = new LocalAccountRepository(default(int), I18N.DefaultStorage);
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