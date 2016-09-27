using data.repositories.account;
using data.database;
using data.factories;
using models;
using data.database.models;
using data.database.helper;
using MyCryptos.resources;
using System.Threading.Tasks;

namespace data.storage
{
	public class AccountStorage : AbstractDatabaseStorage<AccountRepositoryDBM, AccountRepository, AccountDBM, Account>
	{
		protected override AccountRepository Resolve(AccountRepositoryDBM obj)
		{
			return AccountRepositoryFactory.create(obj);
		}

		public override AbstractRepositoryDatabase<AccountRepositoryDBM> GetDatabase()
		{
			return new AccountRepositoryDatabase();
		}

		protected override async Task OnFirstLaunch()
		{
			var localRepository = new AccountRepositoryDBM(new LocalAccountRepository(InternationalisationResources.DefaultStorage));
			await GetDatabase().AddRepository(localRepository);
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