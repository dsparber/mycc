using System;
using System.Threading.Tasks;

namespace data.repositories.currency
{
	public abstract class OnlineCurrencyRepository : CurrencyRepository
	{
		public override async Task FetchFast()
		{
			await FetchFromDatabase();
		}
	}
}

