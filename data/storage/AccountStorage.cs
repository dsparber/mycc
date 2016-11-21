using data.repositories.account;
using data.database;
using MyCryptos.models;
using data.database.models;
using MyCryptos.resources;
using System.Threading.Tasks;

namespace data.storage
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