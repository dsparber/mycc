using System.Threading.Tasks;
using MyCryptos.Core.Database;
using MyCryptos.Core.Database.Models;
using MyCryptos.Core.Repositories.Currency;


// TODO Refactor -> Remove this class or make it private
namespace MyCryptos.Core.Storage
{
	public class CurrencyRepositoryMapStorage : AbstractDatabaseStorage<CurrencyMapRepositoryDBM, CurrencyRepositoryMap, CurrencyMapDBM, CurrencyMapDBM, string>
	{
		public CurrencyRepositoryMapStorage() : base(new CurrencyMapRepositoryDatabase()) { }

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