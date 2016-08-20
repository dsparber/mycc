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
	public class AccountStorage : AbstractStorage<AccountRepositoryDBM, AccountRepository, AccountDBM, Account>
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
			await GetDatabase().AddRepository(new AccountRepositoryDBM { Type = AccountRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY, Name = InternationalisationResources.DefaultStorage });
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