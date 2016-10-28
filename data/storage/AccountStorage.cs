using data.repositories.account;
using data.database;
using MyCryptos.models;
using data.database.models;
using MyCryptos.resources;
using System.Threading.Tasks;

namespace data.storage
{
	public class AccountStorage : AbstractDatabaseStorage<AccountRepositoryDBM, AccountRepository, AccountDBM, Account>
	{
		public AccountStorage() : base(new AccountRepositoryDatabase()) { }

		protected override async Task OnFirstLaunch()
		{
			var localRepository = new LocalAccountRepository(InternationalisationResources.DefaultStorage);
			await Add(localRepository);
		}

		public async override Task<AccountRepository> GetLocalRepository()
		{
			return (await Repositories()).Find(r => r is LocalAccountRepository);
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