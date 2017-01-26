using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Abstract.Storage;
using MyCC.Core.Currency.Database;
using MyCC.Core.Currency.Repositories;

// TODO Refactor -> Remove this class or make it private
namespace MyCC.Core.Currency.Storage
{
	public class CurrencyRepositoryMapStorage : AbstractDatabaseStorage<CurrencyMapRepositoryDbm, CurrencyRepositoryMap, CurrencyMapDbm, CurrencyMapDbm, string>
	{
		private CurrencyRepositoryMapStorage() : base(new CurrencyMapRepositoryDatabase())
		{
			Repositories.Add(new CurrencyRepositoryMap());
		}

		private static CurrencyRepositoryMapStorage _instance;

		public static CurrencyRepositoryMapStorage Instance => _instance ?? (_instance = new CurrencyRepositoryMapStorage());

		public override CurrencyRepositoryMap LocalRepository => Repositories.FirstOrDefault();
	}
}