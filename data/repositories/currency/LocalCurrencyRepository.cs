using System;
using System.Threading.Tasks;

namespace data.repositories.currency
{
	public class LocalCurrencyRepository : CurrencyRepository
	{
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

