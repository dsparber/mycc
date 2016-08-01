using System.Threading.Tasks;

namespace data.repositories.currency
{
	public abstract class OnlineCurrencyRepository : CurrencyRepository
	{
		protected OnlineCurrencyRepository(int repositoryId) : base(repositoryId) { }

		public override async Task FetchFast()
		{
			await FetchFromDatabase();
		}
	}
}

