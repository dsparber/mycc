using System.Threading.Tasks;

namespace data.repositories.currency
{
	public class LocalCurrencyRepository : CurrencyRepository
	{
		public LocalCurrencyRepository(int repositoryId) : base(repositoryId) {}

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

