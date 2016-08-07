using System.Threading.Tasks;
using data.database.models;

namespace data.repositories.currency
{
	public class LocalCurrencyRepository : CurrencyRepository
	{
		public LocalCurrencyRepository() : base(CurrencyRepositoryDBM.DB_TYPE_LOCAL_REPOSITORY) {}

		public override async Task Fetch()
		{
			await FetchFromDatabase();
		}

		public override async Task FetchFast()
		{
			await Fetch();
		}
	}
}

