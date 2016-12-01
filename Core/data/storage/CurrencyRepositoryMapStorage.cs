using data.database;
using data.repositories.currency;
using data.database.models;
using System.Threading.Tasks;

namespace data.storage
{
	public class CurrencyRepositoryMapStorage : AbstractDatabaseStorage<CurrencyRepositoryMapDBM, CurrencyRepositoryMap, CurrencyRepositoryElementDBM, CurrencyRepositoryElementDBM, string>
	{
		public CurrencyRepositoryMapStorage() : base(new CurrencyRepositoryMapDatabase()) { }

		protected override async Task OnFirstLaunch()
		{
			await Add(new CurrencyRepositoryMap());
		}

		static CurrencyRepositoryMapStorage instance { get; set; }

		public static CurrencyRepositoryMapStorage Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new CurrencyRepositoryMapStorage();
				}
				return instance;
			}
		}

		public override CurrencyRepositoryMap LocalRepository
		{
			get
			{
				return Repositories.Find(r => r is CurrencyRepositoryMap);
			}
		}
	}
}